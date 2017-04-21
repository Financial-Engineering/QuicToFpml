using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Quic.Config
{
    /// <summary>
    /// Collection of utilities for serialization
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Removes extranious spaces between CSV fields (but not inside of strings)
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static string PackCsv(string csv)
        {
            return String.Join(",", csv.Split(',').Select(str => str.Trim()));
        }

        public static string RemoveControlChars(string str)
        {
            return new string(str.Where(c => !Char.IsControl(c)).ToArray()).Trim();
        }

        /// <summary>
        /// Creates a deep copy of an object using serialization
        /// </summary>
        /// <typeparam name="T">Any serializable type</typeparam>
        /// <param name="obj">Object to clone</param>
        /// <returns>Deep copy of object</returns>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms,obj);
                ms.Position = 0;
                return (T) formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Serialize an object to XML stripping out declaration, namespace and removing UTF-8 byte order mark
        /// </summary>
        /// <typeparam name="T">Any serializable type</typeparam>
        /// <param name="item">Object to serialize</param>
        /// <returns>Serialized object as an UTF-8 XML string</returns>
        public static string SerializeFragment<T>(T item)
        {
            var memStream = new MemoryStream();

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                Encoding = new UTF8Encoding(false) // remove BOM
            };

            using (var textWriter = XmlWriter.Create(memStream, settings))
            {
                var serializer = new XmlSerializer(typeof (T));

                var xns = new XmlSerializerNamespaces();
                xns.Add(String.Empty,String.Empty);

                serializer.Serialize(textWriter, item, xns);
            }

            return Encoding.UTF8.GetString(memStream.ToArray());
        }

        /// <summary>
        /// Serialize an object to XML
        /// </summary>
        /// <typeparam name="T">Any serializable type</typeparam>
        /// <param name="item">Object to serialize</param>
        /// <returns>Serialized object as XML</returns>
        public static string Serialize<T>(T item)
        {
            var memStream = new MemoryStream();

            using (var textWriter = new XmlTextWriter(memStream, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            return memStream != null ? Encoding.UTF8.GetString(memStream.ToArray()) : null;
        }

        /// <summary>
        /// Creates a memory stream from an UTF8 encoded string
        /// </summary>
        /// <param name="value">UTF8 encoded string</param>
        /// <returns>converted memory stream</returns>
        private static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        /// <summary>
        /// Deserialize an UTF-8 encoded XML document into objects
        /// </summary>
        /// <typeparam name="T">Object type to deserialize</typeparam>
        /// <param name="xmlString">UTF-8 encoded document</param>
        /// <returns>Object of type defined by T</returns>
        public static T Deserialize<T>(string xmlString)
        {
            if (String.IsNullOrWhiteSpace(xmlString))
                return default(T);

            using (var reader = new XmlTextReader(GenerateStreamFromString(xmlString)))
            {
                reader.Normalization = true;
                var serializer = new XmlSerializer(typeof (T));
                return (T) serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Return a list of files of a given set of extensions
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="extensions">Parameter array containing list of extension</param>
        /// <returns>Enumerated list of files as FileInfo objects</returns>
        /// <example>
        /// Return a list of files in directory c:\temp with the extensions txt and csv
        /// <code>
        /// var fileInfoList = GetFiles("C:\temp",".txt",".csv");
        /// </code>
        /// </example>
        public static IEnumerable<FileInfo> GetFiles(string path, params string[] extensions)
        {
            var list = new List<FileInfo>();
            foreach (var extention in extensions)
            {
                list.AddRange(new DirectoryInfo(path)
                    .GetFiles("*" + extention)
                    .Where(p => p.Extension.Equals(extention, StringComparison.CurrentCultureIgnoreCase)).ToArray());
            }
            return list;
        }

        /// <summary>
        /// Delete files of a given set of extensions
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="extentions">Parameter array containing list of file extensions</param>
        /// <example>
        /// Delete all files ending in txt and csv in directory C:\temp
        /// <code>
        /// DeleteFiles("C:\temp",".txt",".csv");
        /// </code>
        /// </example>
        public static void DeleteFiles(string path, params string[] extentions)
        {
            var files = GetFiles(path, extentions);

            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
                File.Delete(file.FullName);
            }
        }

        private static List<Type> UpCastApprovedTypes
        {
            get
            {
                var approvedTypes = new List<Type>
                {
                    typeof (int),
                    typeof (int),
                    typeof (Int32),
                    typeof (Int64),
                    typeof (string),
                    typeof (DateTime),
                    typeof (double),
                    typeof (decimal),
                    typeof (float),
                    typeof (List<>),
                    typeof (Dictionary<string, List<string>>),
                    typeof (bool),
                    typeof (Boolean),
                    typeof (int?),
                    typeof (Int32?),
                    typeof (Int64?),
                    typeof (DateTime?),
                    typeof (double?),
                    typeof (decimal?),
                    typeof (float?),
                    typeof (bool?),
                    typeof (Boolean?)
                };

                return approvedTypes;
            }
        }

        /// <summary>
        /// Upcasts a base class to a derived class using reflection on properties
        /// </summary>
        /// <typeparam name="T">Derived Class</typeparam>
        /// <param name="baseObject"></param>
        /// <param name="excludeProps">List of properties to exclude</param>
        /// <returns>Base class object copied into derived class</returns>
        /// <example>
        /// <code>
        /// var derivedClassObject = baseClassObject.UpCast&lt;DerivedClass&gt;();
        /// </code>
        /// </example>
        public static T UpCast<T>(this object baseObject, params string[] excludeProps)
        {
            var sourceType = baseObject.GetType();
            var derivedType = typeof(T);

            var result = Activator.CreateInstance<T>();

            foreach (var sourceProperty in sourceType.GetProperties())
            {
                //Skip if in the exclude list
                if (excludeProps.Contains(sourceProperty.Name)) continue;

                var sourcePropertyValue = sourceProperty.GetValue(baseObject, null);

                if (sourcePropertyValue == null) continue;

                var destinationProperty = derivedType.GetProperty(sourceProperty.Name);

                if (destinationProperty == null) continue;

                try
                {
                    destinationProperty.SetValue(result, sourcePropertyValue, null);
                }
                catch
                {
                    
                }
            }

            return result;
        }
    }
}