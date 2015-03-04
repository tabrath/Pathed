using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pathed
{
    public static class BinaryFormatterExtensions
    {
        public static T Deserialize<T>(this BinaryFormatter binaryFormatter, Stream stream)
        {
            if (binaryFormatter == null)
                throw new ArgumentNullException("binaryFormatter");

            if (stream == null)
                throw new ArgumentNullException("stream");

            return (T)binaryFormatter.Deserialize(stream);
        }

        public static void Serialize<T>(this BinaryFormatter binaryFormatter, Stream stream, T value)
        {
            if (binaryFormatter == null)
                throw new ArgumentNullException("binaryFormatter");

            if (stream == null)
                throw new ArgumentNullException("stream");

            binaryFormatter.Serialize(stream, value);
        }
    }
}
