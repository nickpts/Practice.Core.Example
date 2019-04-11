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
using System.Threading;

using Practice.Core.Examples.Concurrency;

namespace Practice.Core.Examples
{
    public class Serialization
    {
        #region DataContractSerializer

        /// <summary>
        /// Explicit serialization/deserialization
        /// </summary>
        public static void DataContractSerializer()
        {
            var serializedPerson = new Person {
                Name = "Nick",
                Age = 35,
                MassiveArray = File.ReadAllLines(CParallel.millionLineFilePath).ToArray() };

            var ds = new DataContractSerializer(typeof(Person));

            Console.WriteLine($"Serializing person: { serializedPerson.Name }, { serializedPerson.Age }");

            using (Stream s = File.Create("person.xml"))
            {
                ds.WriteObject(s, serializedPerson);
            }

            Thread.Sleep(1000);
            Console.WriteLine($"Person serialized, file size is: { new FileInfo("person.xml").Length } ");

            var deserializedPerson = new Person();

            Thread.Sleep(1000);
            Console.WriteLine($"Attempting deserialization");

            using (Stream s = File.OpenRead("person.xml"))
            {
                deserializedPerson = (Person)ds.ReadObject(s);
            }

            Console.WriteLine($"Deserialized person: { deserializedPerson.Name }, { deserializedPerson.Age }");
            Console.ReadLine();
        }

        /// <summary>
        /// Explicit serialization/deserialization with binary formatter.
        /// The documentation says that binary formatter should be reduce the size if the type contains 
        /// large arrays, this does not seem to be the case.
        /// </summary>
        public static void DataContractSerializerBinaryFormatted()
        {
            var serializedPerson = new Person {
                Name = "Nick",
                Age = 35,
                MassiveArray = File.ReadAllLines(CParallel.millionLineFilePath).ToArray()
            };

            var ds = new DataContractSerializer(typeof(Person));

            Console.WriteLine($"Serializing person: { serializedPerson.Name }, { serializedPerson.Age }");

            var s = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(s))
            using (Stream fileStream = File.Create("personBinaryFormatted"))
            {
                ds.WriteObject(s, serializedPerson);
                s.WriteTo(fileStream);
            }         

            Thread.Sleep(1000);
            Console.WriteLine($"Person serialized, stream size is: { new FileInfo("personBinaryFormatted").Length } ");

            var s2 = new MemoryStream(s.ToArray());
            var deserializedPerson = new Person();

            Thread.Sleep(1000);
            Console.WriteLine($"Attempting deserialization");

            using (var reader = XmlDictionaryReader.CreateBinaryReader(s2, XmlDictionaryReaderQuotas.Max))
            {
                deserializedPerson = (Person)ds.ReadObject(s2);
            }

            Console.WriteLine($"Deserialized person: { deserializedPerson.Name }, { deserializedPerson.Age }");
            Console.ReadLine();
        }

        #endregion

        #region NetDataContractSerializer

        /// <summary>
        /// Differs from DataContractSerializer in that it writes CLR type information in the serialized XML
        /// Can only be used if both the serializing and deserializing ends share the CLR types
        /// </summary>
        public static void NetDataContractSerializer()
        {
            var serializedPerson = new Person
            {
                Name = "Nick",
                Age = 35,
            };

            Console.WriteLine($"Serializing person: { serializedPerson.Name }, { serializedPerson.Age }");

            var ds = new NetDataContractSerializer();

            using (Stream s = File.Create("person.xml"))
            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(s))
            {
                ds.WriteObject(writer, serializedPerson);
            }

            Thread.Sleep(1000);
            Console.WriteLine($"Person serialized, file size is: { new FileInfo("person.xml").Length } ");
            Console.WriteLine(File.ReadAllText("person.xml"));

            Thread.Sleep(1000);
            Console.WriteLine($"Attempting deserialization");

            var deserializedPerson = new Person();

            using (FileStream s = new FileStream("person.xml", FileMode.Open))
            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(s, new XmlDictionaryReaderQuotas()))
            {
                deserializedPerson = (Person)ds.ReadObject(reader, true);
            }

            Console.WriteLine($"Deserialized person: { deserializedPerson.Name }, { deserializedPerson.Age }");
            Console.ReadLine();
        }

        #endregion

        #region BinarySerializer

        /// <summary>
        /// The binary serializer supports two formatters, BinaryFormatter
        /// and SoapFormatter, the first one is displayed below
        /// </summary>
        public static void BinarySerialization()
        {
            XPerson nick = new XPerson { Name = "Nick", Age = 33, MassiveArray = File.ReadAllLines(CParallel.millionLineFilePath).ToArray() };
            IFormatter formatter = new BinaryFormatter();

            using (Stream s = File.Create(@"C:\Users\Nick\Downloads\ser.bin"))
            {
                formatter.Serialize(s, nick);
            }

            Console.WriteLine($"Serialized file size: { new FileInfo(@"C:\Users\Nick\Downloads\ser.bin").Length }");

            using (Stream s = File.OpenRead(@"C:\Users\Nick\Downloads\ser.bin"))
            {
                XPerson dsNick = (XPerson)formatter.Deserialize(s);
            }

            Console.ReadLine();
        }

        #endregion

        #region XmlSerializer

        public static void XmlSerializer()
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

        }

        #endregion
    }

    [DataContract, KnownType(typeof(Student)), KnownType(typeof(Teacher))]
    public class Person
    {
        [XmlElement("TestName")]
        public string Name;

        [DataMember (EmitDefaultValue = false)]
        public int Age;

        [DataMember]
        public string[] MassiveArray;
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

        public string[] MassiveArray;

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
