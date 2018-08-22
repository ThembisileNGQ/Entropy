using System;
using Newtonsoft.Json.Serialization;

namespace Akka.Websockets.Manager.Common.Json
{
    public class JsonBinderWithoutAssembly : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            typeName = serializedType.FullName;
            assemblyName = null;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}