using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Scripts.Other
{
    public static class Converter
    {
        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(System.Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        public static System.Object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                System.Object obj = (System.Object)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}