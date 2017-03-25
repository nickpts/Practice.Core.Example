using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Practice.Core.Examples
{
    public class Serialization
    {

        public static void Test()
        {
            XPerson nick = new XPerson { Name = "Nick", Age = 33 };
            IFormatter formatter = new BinaryFormatter();
            
            using (Stream s = File.Create(@"C:\Users\Nick\Downloads\ser.bin"))
            {
                formatter.Serialize(s, nick);
            }

            using (Stream s = File.OpenRead(@"C:\Users\Nick\Downloads\ser.bin"))
            {
                XPerson dsNick = (XPerson)formatter.Deserialize(s);
            }
        }

        public void TestBf()
        {
            Person nick = new Person { Name = "Nick", Age = 33 };

            XmlSerializer xs = new XmlSerializer(typeof(Person));

            using (Stream writer = File.Create("person.xml"))
            {
                xs.Serialize(writer, nick);
            }

            using (Stream reader = File.OpenRead("person.xml"))
            {
                Person p2 = (Person)xs.Deserialize(reader);
            }


            var ds = new DataContractSerializer(typeof(Person));

            var s = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(s))
            {
                ds.WriteObject(s, nick);
            }

            var s2 = new MemoryStream(s.ToArray());
            Person dsNick;

            using (var reader = XmlDictionaryReader.CreateBinaryReader(s2, XmlDictionaryReaderQuotas.Max))
            {
                dsNick = (Person)ds.ReadObject(s2);
            }

            
        }
    }

    [DataContract, KnownType(typeof(Student)), KnownType(typeof(Teacher))]
    public class Person
    {
        [XmlElement("TestName")]
        public string Name;

        [DataMember (EmitDefaultValue = false)]
        public int Age;
    }

    [DataContract]
    public class Student: Person
    {

    }

    [DataContract]
    public class Teacher : Person
    {

    }

    [Serializable]
    public sealed class XPerson
    {
        public string Name;
        [OptionalField (VersionAdded = 2)]
        public DateTime DateOfBirth;

        [NonSerialized]
        public int Age;
        [NonSerialized]
        public bool Valid = true;

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            Valid = true;
        }

        public interface ISerializable
        {
            void GetObjectData(SerializationInfo info, StreamingContext context);
        }
    }

}
