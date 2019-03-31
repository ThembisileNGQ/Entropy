using System;

namespace Bundlr.Core
{
    public interface ICodec
    {
        byte[] Encode(Message message);
        Message Decode(Span<byte> data);
    }
}