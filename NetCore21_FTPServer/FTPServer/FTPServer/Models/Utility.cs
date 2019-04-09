using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace FTPServer.Models
{
    public class Utility
    {
        public static T Deserialize<T>(string input) where T : class
        {
            var ser = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T objectToSerialize)
        {
            var xmlSerializer = new XmlSerializer(objectToSerialize.GetType());

            using (var textWriter = new Utf8StringWriter())
            {
                xmlSerializer.Serialize(textWriter, objectToSerialize);
                return textWriter.ToString();
            }

        }

        public static long ConvertMegaBytesToBytes(double megabytes)
        {
            var bytes = megabytes * (1024f * 1024f);
            return Convert.ToInt64(bytes);
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}