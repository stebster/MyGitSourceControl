namespace Stebster.Core.Extension
{
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static class StebsterExtentionMethods
    {
        /// <summary>
        ///     Converts this object to a serialised XElement for use in an XDocument (xml document)
        /// </summary>
        /// <typeparam name="T">Class type of the passed object</typeparam>
        /// <param name="obj">The object iteself</param>
        /// <returns>The XElement that the object serialises to</returns>
        public static XElement ToXElement<T>(this object obj)
        {
            XElement element;

            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    element = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }

            return element;
        }

        /// <summary>
        ///     Converts this XNode into an object of the passed Type T if possible
        /// </summary>
        /// <typeparam name="T">Class type of the object we want to create</typeparam>
        /// <param name="xElement">The XNode we want to deserialise to an object</param>
        /// <returns>The object or null if failed</returns>
        public static T ToObjectOfType<T>(this XNode xElement) where T : class
        {
            T obj = null;

            using (var xmlReader = xElement.CreateReader())
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                if (xmlSerializer.CanDeserialize(xmlReader))
                {
                    obj = xmlSerializer.Deserialize(xmlReader) as T;
                }
            }
           
            return obj;
        }

    }
}
