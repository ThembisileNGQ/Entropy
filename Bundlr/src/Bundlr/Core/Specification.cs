using System.Collections.Generic;

namespace Bundlr.Core
{
    public static class Specification
    {
        public static bool IsHeaderSizeLimited(Dictionary<string, string> headers)
        {
            return headers.Count <= Constants.HeaderSizeMax;
        }
        public static bool IsPayloadSizeLimited(byte[] payload)
        {
            return payload.Length <= Constants.PayloadSizeMax;
        }

        public static bool IsHeaderItemSizeLimited(byte[] headerItem)
        {
            return headerItem.Length <= Constants.HeaderItemByteLengthMax;
        }
    }
}