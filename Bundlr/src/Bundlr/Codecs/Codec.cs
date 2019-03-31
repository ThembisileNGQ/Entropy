using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bundlr.Core;

namespace Bundlr.Codecs
{
    public class NaiveCodec : ICodec
    {
        public byte[] Encode(Message message)
        {
            var payloadSize = (uint)message.Payload.Length;
            var headerSize = (byte)message.Headers.Count;

            var buffer = new List<byte>().ToArray();
            
            buffer = Combine(buffer, new [] {headerSize});
            buffer = Combine(buffer, BitConverter.GetBytes(payloadSize));
            buffer = Combine(buffer, BitConverter.GetBytes(Constants.ChecksumSizeMax));

            var header = message.Headers;
            
            foreach (var key in header.Keys)
            {
                var keyInBytes = Encoding.UTF8.GetBytes(key);
                var keyLengthInBytes = BitConverter.GetBytes(keyInBytes.Length);
                
                if(Specification.IsHeaderItemSizeLimited(keyLengthInBytes))
                    throw new EncodeException("header key exceeds specification");
                
                var valueInBytes = Encoding.UTF8.GetBytes(header[key]);
                var valueLengthInBytes = BitConverter.GetBytes(valueInBytes.Length);
                
                if(Specification.IsHeaderItemSizeLimited(valueInBytes))
                    throw new EncodeException("header value exceeds specification");

                buffer = Combine(buffer, keyLengthInBytes, keyInBytes, valueLengthInBytes, valueInBytes);
            }

            buffer = Combine(buffer, message.Payload);
            
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(buffer);
                Console.WriteLine(hashBytes.Length);
                buffer = Combine(buffer, hashBytes);
                return buffer;
            }
        }

        public Message Decode(Span<byte> data)
        {

            if (!IsValidChecksum(data))
              throw new DecodeException("the error code checks do not match");  
            
            var headerSize = data[0];
            var headerStart = Constants.HeaderSizeDescriptorByteLength + Constants.PayloadSizeDescriptorByteLength + Constants.ChecksumSizeDescriptorByteLength;

            var headers = GetHeaders(data, headerSize, headerStart);
            var payload = GetPayload(data, headers.position);
            
            return new Message(headers.headers, payload);
        }



        internal static byte[] GetPayload(Span<byte> data, int headerPosition)
        {
            return data.Slice(headerPosition, data.Length - headerPosition - Constants.ChecksumSizeMax).ToArray();
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
                var keyLength = BitConverter.ToInt32(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var key = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, keyLength));

                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + keyLength;
                
                var valueLength = BitConverter.ToInt32(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var value = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, valueLength));
                
                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + valueLength;

                headers[key] = value;
            }


            return (headers, headerPosition);
        }

        internal static bool IsValidChecksum(Span<byte> data)
        {
            using (MD5 md5 = MD5.Create())
            {
                var fromCompute = md5.ComputeHash(data.Slice(0,data.Length - Constants.ChecksumSizeMax).ToArray());
                var fromTailer = data.Slice(data.Length - Constants.ChecksumSizeMax, Constants.ChecksumSizeMax);
                return fromCompute.SequenceEqual(fromTailer.ToArray());
            }
        }
        
        internal static byte[] Combine(
            byte[] first,
            byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        internal static byte[] Combine(
            byte[] first,
            byte[] second,
            byte[] third,
            byte[] fourth,
            byte[] fifth)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,third.Length);
            Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length,fourth.Length);
            Buffer.BlockCopy(fifth, 0, ret, first.Length + second.Length + third.Length + fourth.Length,fifth.Length);
            return ret;
        }

    }
}