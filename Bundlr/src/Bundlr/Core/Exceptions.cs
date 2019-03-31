using System;

namespace Bundlr.Core
{
    public class DecodeException : Exception
    {
        public DecodeException(string message)
           : base(message)
        {
            
        }
    }

    public class EncodeException : Exception
    {
        public EncodeException(string message)
            : base(message)
        {
            
        }
    }
}