namespace Bundlr.Core
{
    public interface ICodec
    {
        byte[] Encode(Message message);
        Message Decode(byte[] data);
    }
}