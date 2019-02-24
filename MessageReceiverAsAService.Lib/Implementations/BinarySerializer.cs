﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessageReceiverAsAService.Lib.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class BinarySerializer : IBinarySerializer
    {
        public byte[] Serialize<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            using (var writer = new BsonDataWriter(memoryStream))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, value);

                return memoryStream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data) where T : class
        {
            if (data == null)
            {
                return default(T);
            }

            var memoryStream = new MemoryStream(data);
            using (var reader = new BsonDataReader(memoryStream))
            {
                if (typeof(T).GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    reader.ReadRootValueAsArray = true;
                }

                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
