using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bundlr.Core;

using static Bundlr.Core.Combinator;

namespace Bundlr.Codecs
{
    public class Codec : ICodec
    {
        public byte[] Encode(Message message)
        {
            if (message == null)
                throw new EncodeException("argument for encoding is null.");

            var buffer = WriteMetadata(message);

            buffer = WriteHeaders(buffer, message);

            buffer = WritePayload(buffer, message);

            buffer = WriteChecksum(buffer);

            return buffer;
        }

        private byte[] WriteMetadata(Message message)
        {
            var buffer = new List<byte>().ToArray();

            var payloadSize = (uint)message.Payload.Length;
            var headerSize = (byte)message.Headers.Count;

            buffer = Combine(buffer, new [] {headerSize});
            buffer = Combine(buffer, BitConverter.GetBytes(payloadSize));
            buffer = Combine(buffer, BitConverter.GetBytes(Constants.ChecksumSizeMax));

            return buffer;
        }

        private byte[] WriteHeaders(byte[] buffer, Message message)
        {
            var header = message.Headers;

            foreach (var key in header.Keys)
            {
                var keyInBytes = Encoding.UTF8.GetBytes(key);
                var keyLengthInBytes = BitConverter.GetBytes((short)keyInBytes.Length);

                if(!Specification.IsHeaderItemSizeLimited(keyInBytes))
                    throw new EncodeException("header key does not meet specification.");

                var valueInBytes = Encoding.UTF8.GetBytes(header[key]);
                var valueLengthInBytes = BitConverter.GetBytes((short)valueInBytes.Length);

                if(!Specification.IsHeaderItemSizeLimited(valueInBytes))
                    throw new EncodeException("header value does not meet specification.");

                buffer = Combine(buffer, keyLengthInBytes, keyInBytes, valueLengthInBytes, valueInBytes);
            }

            return buffer;
        }

        private byte[] WritePayload(byte[] buffer, Message message)
        {
            buffer = Combine(buffer, message.Payload);

            return buffer;
        }
        
        private byte[] WriteChecksum(byte[] buffer)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(buffer);
                buffer = Combine(buffer, hashBytes);
                return buffer;
            }
        }

        public Message Decode(Span<byte> data)
        {
            if(!IsViableBinaryMessage(data))
                throw new DecodeException("binary message is not viable.");
            if (!IsValidChecksum(data))
              throw new DecodeException("the error code checks do not match.");
            var headerSize = data[0];

            var headerStart =
                Constants.HeaderSizeDescriptorByteLength +
                Constants.PayloadSizeDescriptorByteLength +
                Constants.ChecksumSizeDescriptorByteLength;

            var headers = GetHeaders(data, headerSize, headerStart);
            var payload = GetPayload(data, headers.position);

            return new Message(headers.headers, payload);
        }

        internal static (Dictionary<string, string> headers, int position) GetHeaders(
            Span<byte> data,
            byte headerSize,
            int headerStart)
        {
            var headers = new Dictionary<string,string>();
            var headerPosition = headerStart;

            for (byte i = 0; i < headerSize; i++)
            {
                var keyLength = BitConverter.ToUInt16(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var key = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, keyLength));

                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + keyLength;
                var valueLength = BitConverter.ToUInt16(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var value = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, valueLength));

                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + valueLength;

                headers[key] = value;
            }

            return (headers, headerPosition);
        }

        internal static byte[] GetPayload(Span<byte> data, int headersEndingPosition)
        {
            return data.Slice(headersEndingPosition, data.Length - headersEndingPosition - Constants.ChecksumSizeMax).ToArray();
        }

        internal static bool IsViableBinaryMessage(Span<byte> data)
        {
            return data.Length > Constants.HeaderSizeDescriptorByteLength + Constants.PayloadSizeDescriptorByteLength + Constants.ChecksumSizeDescriptorByteLength;
        }
        internal static bool IsValidChecksum(Span<byte> data)
        {
            //    Checksum works by extracting the checksum at the tail end
            //    of the binary message, and compares it with the hash of the
            //    rest of the binary string. It is similar to HMAC.

            using (MD5 md5 = MD5.Create())
            {
                var fromCompute = md5.ComputeHash(data.Slice(0,data.Length - Constants.ChecksumSizeMax).ToArray());
                var fromTailer = data.Slice(data.Length - Constants.ChecksumSizeMax, Constants.ChecksumSizeMax);
                return fromCompute.SequenceEqual(fromTailer.ToArray());
            }
        }

    }
}