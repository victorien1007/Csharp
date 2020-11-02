using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;

namespace StrM
{
    [Serializable]
    public class StrMessage
    {
        private string toMessage;
        private string toTopic;
        private string toName;

        public string ToMessage { get => toMessage; set => toMessage = value; }
        public string ToTopic { get => toTopic; set => toTopic = value; }
        public string ToName { get => toName; set => toName = value; }

        public StrMessage() { }
    }
    public class SerObject
    {
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;
            //Memory instance
            //内存实例
            MemoryStream ms = new MemoryStream();
            //Create a serialized instance
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);//Serialize the object, write into the ms stream 序列化对象，写入ms流中  
            ms.Position = 0;
            //byte[] bytes = new byte[ms.Length];//这个有错误
            byte[] bytes = ms.GetBuffer();
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }

        /// <summary>  
        ///Deserialize byte arrays into objects 把字节数组反序列化成对象  
        /// </summary>  
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            //Use the incoming byte [] to create a memory stream
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);//Inverse memory stream into objects 把内存流反序列成对象  
            ms.Close();
            return obj;
        }

    }

}
