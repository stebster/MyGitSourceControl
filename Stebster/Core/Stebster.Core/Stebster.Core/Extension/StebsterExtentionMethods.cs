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
        /// <param name="prefix">Optional namespace prefix</param>
        /// <param name="ns">Optional namespace</param>
        /// <returns>The XElement that the object serialises to</returns>
        public static XElement ToXElement<T>(this object obj, string prefix="", string ns="")
        {
            XElement element;

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    // Create list of namespaces for serialiser and add the optional passed ones
                    var serialiserNamespaces = new XmlSerializerNamespaces();
                    serialiserNamespaces.Add(prefix, ns);

                    // Serialise passed object to memory stream
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(streamWriter, obj, serialiserNamespaces);

                    // Get the string from the memory stream and parse as an Xelement
                    var xmlString = Encoding.UTF8.GetString(stream.ToArray());
                    element = XElement.Parse(xmlString);
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
                var serializer = new XmlSerializer(typeof(T));

                if (serializer.CanDeserialize(xmlReader))
                {
                    obj = serializer.Deserialize(xmlReader) as T;
                }
            }
           
            return obj;
        }

    }
}
