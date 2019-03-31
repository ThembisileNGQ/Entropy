using System;
using System.Collections.Generic;
using System.Linq;
using Bundlr.Core;

namespace Bundlr.Tests
{
    public class TestHelper
    {
        private static Random Random = new Random();
        private const string CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÅÖabcdefghijklmnopqrstuvwxyzäåö0123456789";
        private static string GetRandomString(int maxLength = 1000)
        {
            var length = Random.Next(maxLength);
            
            var characters = Enumerable
                .Repeat(CharacterSet, length)
                .Select(s => s[Random.Next(s.Length)])
                .ToArray();
            
            return new string(characters);
        }

        
        // Helper class to create a random message with randomly generated data
        internal static Message CreateMessage()
        {
            var headerSize = Random.Next(Constants.HeaderSizeMax);
            var payloadSize = Random.Next((int)Constants.PayloadSizeMax);

            var headers = Enumerable
                .Range(0, headerSize)
                .Select(x => new KeyValuePair<string, string>(GetRandomString(), GetRandomString()))
                .ToDictionary(x => x.Key, x => x.Value);
            
            
            var payload = new byte[payloadSize];
            Random.NextBytes(payload);
            
            return new Message(headers,payload);
        }
    }
}