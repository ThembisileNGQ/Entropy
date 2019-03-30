using System;
using System.Collections.Generic;
using System.Linq;
using Bundlr.Codecs;
using Bundlr.Core;

namespace Bundlr.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var payload = Guid.NewGuid().ToByteArray();
            //payload = payload.Concat(Guid.NewGuid().ToByteArray()).ToArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();
            var value = RandomString(30);
            var key = RandomString(4);
            
            Console.WriteLine(new Guid(payload));
            Console.WriteLine(key);
            Console.WriteLine(value);

            var message = new Message(new Dictionary<string, string>
            {
                {key, value}
            }, payload);

            var codec = new NaiveCodec();

            var data = codec.Encode(message);
            var msg = codec.Decode(data);
            
            Console.WriteLine(msg == message);
        }
        
        private static Random random = new Random();
        public static string RandomString(int len)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÅÖabcdefghijklmnopqrstuvwxyzäåö0123456789";
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}