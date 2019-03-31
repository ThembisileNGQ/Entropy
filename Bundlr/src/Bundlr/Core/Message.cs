using System;
using System.Collections.Generic;
using System.Linq;

namespace Bundlr.Core
{
    public class Message
    {
        public Dictionary<string, string> Headers { get; }
        public byte[] Payload { get; }

        public Message(
            Dictionary<string, string> headers,
            byte[] payload)
        {
            if(headers == null)
                throw new ArgumentException(nameof(headers));
            if(payload == null)
                throw new ArgumentException(nameof(headers));

            Payload = payload;
            Headers = headers;
        }
        
        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return other.Payload.SequenceEqual(Payload) &&
                   Headers.Count == other.Headers.Count &&
                   !Headers.Except(other.Headers).Any();
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Message) obj);
        }
        
        public static bool operator ==(Message left, Message right)
        {
            return Equals(left, right);
        }
        
        public static bool operator !=(Message left, Message right)
        {
            return !Equals(left, right);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Headers != null ? Headers.GetHashCode() : 0) * 397) ^ (Payload != null ? Payload.GetHashCode() : 0);
            }
        }
    }
}