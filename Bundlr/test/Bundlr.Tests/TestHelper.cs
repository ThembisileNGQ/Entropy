using System;
using System.Linq;

namespace Bundlr.Tests
{
    public class TestHelper
    {
        
        private static Random random = new Random();
        private static string GetRandomString(int maxLength)
        {
            var length = random.Next(maxLength);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÅÖabcdefghijklmnopqrstuvwxyzäåö0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}