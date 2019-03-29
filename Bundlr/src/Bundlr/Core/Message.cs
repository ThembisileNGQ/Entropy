using System;
using System.Collections.Generic;

namespace Bundlr.Core
{
    public class Message {
        public Dictionary<String, String> Headers { get; }
        public byte[] Payload { get; }
    }

    public class Constants
    {
        internal const ushort HEADER_SIZE_MAX = 63;
        internal const ushort HEADER_ITEM_BYTE_LENGTH_MAX = 1023;
        internal const uint PAYLOAD_SIZE_MAX = 256 * 1000;
    }
}