using System.Collections.Generic;
using Bundlr.Codecs;
using Bundlr.Core;
using FluentAssertions;
using Xunit;

namespace Bundlr.Tests
{
    public class UnitTests
    {
        [Fact]
        public void SerializingAndDeserializng_Message_ShouldBeEqual()
        {
            var codec = new Codec();
            var obj = TestHelper.CreateMessage();

            var data = codec.Encode(obj);

            codec.Decode(data).Should().Be(obj);
        }
        
        [Fact]
        public void SerializingAndDeserializng_MessageWithEmptyMembers_ShouldBeEqual()
        {
            var codec = new Codec();
            var obj = new Message(new Dictionary<string, string>(), new List<byte>().ToArray());

            var data = codec.Encode(obj);

            codec.Decode(data).Should().Be(obj);
        }
    }
}