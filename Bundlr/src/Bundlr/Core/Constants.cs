namespace Bundlr.Core
{
    public class Constants
    {
        public const ushort ChecksumSizeMax = 16;
        public const ushort HeaderSizeMax = 63;
        public const uint PayloadSizeMax = 256 * 1000;
        public const ushort HeaderItemByteLengthMax = 1023;
        public const ushort HeaderItemDescriptorByteLength = sizeof(int);
        public const ushort HeaderSizeDescriptorByteLength = sizeof(byte);
        public const ushort PayloadSizeDescriptorByteLength = sizeof(uint);
        public const ushort ChecksumSizeDescriptorByteLength = sizeof(ushort);
    }
}