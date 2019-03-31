# Bundlr

The simple binary message codec written for .NET Core applications.

Bundlr is a binary message format that encodes a POCO that looks like this.

```csharp
public class Message
{
    public Dictionary<string, string> Headers { get; }
    public byte[] Payload { get; }
}
```

# Motives
Sometimes if you are transmitting data over the wire with a fixed standard schema, it may be better to use a fixed schema binary formatter that can be faster than the other general purpose no schema binary message formatters like [Hyperion](https://github.com/akkadotnet/Hyperion)
The message schema chosen is one that is simple to understand. The message schema should be simple enough to implement on other platforms with relative ease.

# Usage

```csharp

//Create encoder
var codec = new Codec();
var headers = new Dictionary<string,string>
{
    {"Bundlr","The Simple Binary Message Codec."},
    {"Sinch","Bringing Enriched Engagement To The Masses."},
}
var payload = //some byte array
var object = new Message(headers,payload)

//Encode your message into the specified binary message format
var data = codec.Encode(object);

//decode a binary formatted message into the typed Message
var message = codec.Decode(data);
```

Also look at the [unit tests](https://github.com/Lutando/Entropy/Bundlr/test/Bundlr.Tests/UnitTests.cs) to see how it works.

# Binary Message Format
The binary message format at a high level can be described as some a series of encoded metadata, followed by an encoded array of key values, followed by a string of bytes that is the payload, and finally followed by the string of bytes that represent the checksum

The first byte describes how many key value pairs are contained in the message. The next 4 bytes represent how big the message payload is. The next 2 bytes is the message checksum.

It can be aptly described by the message diagram below:
```
+--------------+--------------+---------------+
|  Header Size | Payload Size | Checksum Size |
+---------------------+-----------------------+
|  Header Key Length 1  |    Header Key 1     |
+-----------------------+---------------------+
| Header Value Length 1 |    Header Value 1   |
+-----------------------+---------------------+
|  Header Key Length 2  |    Header Key 2     |
+-----------------------+---------------------+
| Header Value Length 2 |    Header Value 2   |
+-----------------------+---------------------+
.                       .                     .
.                       .                     .
.                       .                     .
+-----------------------+---------------------+
|  Header Key Length n  |    Header Key n     |
+-----------------------+---------------------+
| Header Value Length n |    Header Value n   |
+-----------------------+---------------------+
|              Message Payload                |
+---------------------+-----------------------+
|              Message CheckSum               |
+---------------------+-----------------------+
//|n| <= 63

Header Key Length = Describes the byte length of the header key.
Header Key = Holds the utf-8 encoded header key.
Header Value Length = Describes the byte length of the header value.
Header Value = Holds the utf-8 encoded header value.
Message Payload = Holds the payload.
Message Checksum = Is the error checking checsum

```
> This diagram may be misleading, the header byte blocks described above ie `Header Size`, `Payload Size`, `Checksum Size`, and `Header Key Length` etc are not proportionally size in relation to one another. eg The header size is only 1 byte, and the header keys can be variable in size just as long as they do not exceed 1023 bytes. This diagram is there to show the sequence of bytes and where they are placed in the encoded binary message.

# Areas for Improvement
**Benchmarking** - It is not substantial enough to just implement a binary message format without benchmarking it against others. Bundlr would need to be profiled for its computational resource usage. Some metrics to test for could be:

* Encoded message size
* Encode/decode speed
* Memory allocations during encoding/decoding
* Processor utilization during encoding/decoding

The benchmarks above should be a good first attempt to profile the codec. It would be beneficial to pit this codec against other binary message formatters to see how good or bad it is.

**Versioning** - If Bundlr were to be used in production, it would probably need a message schema version. As the message format is likely to evolve, it might be beneficial to add a schema version to the binary message format in the message header.


