using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Pathed
{
    public static class BinaryFormatterExtensions
    {
        public static T Deserialize<T>(this BinaryFormatter binaryFormatter, Stream stream)
        {
            return (T)binaryFormatter.Deserialize(stream);
        }

        public static void Serialize<T>(this BinaryFormatter binaryFormatter, Stream stream, T value)
        {
            binaryFormatter.Serialize(stream, value);
        }
    }
}
