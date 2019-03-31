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
            var payloadSize = (uint)message.Payload.Length;
            var headerSize = (byte)message.Headers.Count;

            var buffer = new List<byte>().ToArray();
            
            // 1.    Hydrate the start of the message with information about
            //       the data to be encoded. header, payload, and checksum sizes
            //       are stored here
            
            buffer = Combine(buffer, new [] {headerSize});
            buffer = Combine(buffer, BitConverter.GetBytes(payloadSize));
            buffer = Combine(buffer, BitConverter.GetBytes(Constants.ChecksumSizeMax));

            var header = message.Headers;
            
            //2.    For every key value in the header, encode the header item into 
            //      the binary message by storing the header item size and the actual
            //      header item into the binary string.
            
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

            //3.    Add the payload to the binary message
            
            buffer = Combine(buffer, message.Payload);
            
            //4.    Append the checksum hash (in this case MD5)
            //      To the end of the binary message
            
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(buffer);
                buffer = Combine(buffer, hashBytes);
                return buffer;
            }
        }

        
        public Message Decode(Span<byte> data)
        {
            //1.    Check that the message to be decoded is free from
            //      errors that might have been introduced from transmission or storage
            if (!IsValidChecksum(data))
              throw new DecodeException("the error code checks do not match");  
            
            //2.    Retreive the header size that describes how many key values
            //      are in the binary message. this will be crucial in extracting the 
            //      header from the binary message.
            
            var headerSize = data[0];
            
            //3.    The header always starts at a fixed place according to the schema
            //      so we can start decoding our header from this position.
            
            var headerStart = Constants.HeaderSizeDescriptorByteLength + Constants.PayloadSizeDescriptorByteLength + Constants.ChecksumSizeDescriptorByteLength;

            //4.    Retreive the header and payload from the binary message
            
            var headers = GetHeaders(data, headerSize, headerStart);
            var payload = GetPayload(data, headers.position);
            
            return new Message(headers.headers, payload);
        }

        internal static byte[] GetPayload(Span<byte> data, int headersEndingPosition)
        {
            //4.6    Get the payload by slicing the window from where the headers end
            //       to where the checksum begins
            
            return data.Slice(headersEndingPosition, data.Length - headersEndingPosition - Constants.ChecksumSizeMax).ToArray();
        }

        internal static (Dictionary<string, string> headers, int position) GetHeaders(
            Span<byte> data, 
            byte headerSize, 
            int headerStart)
        {
            var headers = new Dictionary<string,string>();
            var headerPosition = headerStart;
            
            // -> this would be easier on the eyes with c# 8s Index/Range core types
            
            for (byte i = 0; i < headerSize; i++)
            {
                //4.1    Retreive the length in bytes of the header key
                //       and then use that information to extract the exact
                //       utf-8 encoded string that presides in that range
                
                var keyLength = BitConverter.ToInt32(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var key = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, keyLength));

                //4.2    Set the header read position to the next header value
                
                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + keyLength;
                
                
                //4.3    Retreive the length in bytes of the header value
                //       and then use that information to extract the exact
                //       utf-8 encoded string that presides in that range
                
                var valueLength = BitConverter.ToInt32(data.Slice(headerPosition, Constants.HeaderItemDescriptorByteLength));
                var value = Encoding.UTF8.GetString(data.Slice(headerPosition + Constants.HeaderItemDescriptorByteLength, valueLength));
                
                //4.4    Set the header read position to the next header key
                
                headerPosition = headerPosition + Constants.HeaderItemDescriptorByteLength + valueLength;

                //4.5    Set the dictionary to have the key and values that have
                //       been decoded.
                
                headers[key] = value;
            }
            
            return (headers, headerPosition);
        }
        
        internal static bool IsValidChecksum(Span<byte> data)
        {
            //    Checksum works by extracting the checksum at the tail end
            //    of the binary message, and compares it with the hash of the
            //    rest of the binary string. It is basically just like HMAC.
            
            using (MD5 md5 = MD5.Create())
            {
                var fromCompute = md5.ComputeHash(data.Slice(0,data.Length - Constants.ChecksumSizeMax).ToArray());
                var fromTailer = data.Slice(data.Length - Constants.ChecksumSizeMax, Constants.ChecksumSizeMax);
                return fromCompute.SequenceEqual(fromTailer.ToArray());
            }
        }

    }
}