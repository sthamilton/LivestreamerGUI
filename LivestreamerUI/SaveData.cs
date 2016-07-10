using System.IO;
using System.Xml.Serialization;

namespace LivestreamerUI
{
    public class SaveData
    {
        public static void Save(object obj, string filename)
        {
            XmlSerializer sr = new XmlSerializer(obj.GetType());
            TextWriter writer = new StreamWriter(filename);
            sr.Serialize(writer, obj);
            writer.Close();
        }
    }
}
