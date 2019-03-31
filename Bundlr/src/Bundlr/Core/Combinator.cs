using System;
using System.Linq;

namespace Bundlr.Core
{
    public static class Combinator
    {
        internal static byte[] Combine(params byte[][] arrays)
        {
            var buffer = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
                offset += data.Length;
            }
            return buffer;
        }
    }
}