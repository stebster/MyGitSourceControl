namespace Stebster.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Extension;
    using Interface;

    public class XmlRepository : IXmlRepository
    {
        #region Private Member Variables

        private const string RootNode = "XmlRepository";
        private XDocument _xDocument;

        #endregion

        #region Public Properties

        public string RepositoryFilePath { get; private set; }

        #endregion

        #region Constructor

        public XmlRepository(string repositoryPath)
        {
            RepositoryFilePath = repositoryPath;
            Initialise();
        }

        #endregion

        #region IXmlRepository Members

        public bool SaveItem<T>(T item) where T : class, IEquatable<T>
        {
            var saved = false;

            if (FindItem(item) == null)
            {
                var itemsCollectionNode = GetNodeUnderRoot(typeof(T).Name, true);

                if (itemsCollectionNode != null)
                {
                    var element = item.ToXElement<T>();

                    if (element != null)
                    {
                        itemsCollectionNode.Add(element);
                        saved = SaveXmlRepository();
                    }
                }
            }
                
            return saved;
        }

        public IList<T> ReadItems<T>() where T : class, IEquatable<T>
        {
            var found = new List<T>();

            var itemsCollectionNode = GetNodeUnderRoot(typeof(T).Name);

            if (itemsCollectionNode != null)
            {
                found.AddRange(itemsCollectionNode.Elements().Select(element => element.ToObjectOfType<T>()).Where(x => x != null));
            }

            return found;
        }

        public bool DeleteItem<T>(T item) where T : class, IEquatable<T>
        {
            var deleted = false;

            var found = FindItem(item);

            if (found != null)
            {
                found.Remove();
                deleted = SaveXmlRepository();
            }
                
            return deleted;
        }

        public T GetItem<T>(T item) where T : class, IEquatable<T>
        {
            T foundItem = null;

            var found = FindItem(item);

            if (found != null)
            {
                foundItem = found.ToObjectOfType<T>();
            }

            return foundItem;
        }

        #endregion

        #region Private Helpers

        private void Initialise()
        {
            var fi = new FileInfo(RepositoryFilePath);

            if (fi.DirectoryName != null && !Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            if (!File.Exists(RepositoryFilePath))
            {
                ResetXmlRepository();
                SaveXmlRepository();
            }

            if (!LoadXmlRepository())
            {
                throw new Exception("Repository Failed To Initialise");
            }
        }

        private bool SaveXmlRepository()
        {
            bool saved;

            try
            {
                _xDocument.Save(RepositoryFilePath);
                saved = true;
            }
            catch (Exception)
            {
                saved = false;
            }

            return saved;

        }

        private bool LoadXmlRepository()
        {
            bool loaded;

            try
            {
                _xDocument = XDocument.Load(RepositoryFilePath);
                loaded = true;
            }
            catch (Exception)
            {
                loaded = false;
            }

            return loaded;
        }

        private void ResetXmlRepository()
        {
            _xDocument = new XDocument(new XElement(RootNode));
        }

        private XElement GetNodeUnderRoot(string nodeName, bool createIfNotFound = false)
        {
            XElement element = null;

            var root = _xDocument.Root;

            if (root != null)
            {
                element = root.Element(nodeName);

                if (element == null && createIfNotFound)
                {
                    element = new XElement(nodeName);
                    root.Add(element);
                }
            }

            return element;
        }

        private XElement FindItem<T>(T item) where T : class, IEquatable<T>
        {
            XElement found = null;

            var itemsCollectionNode = GetNodeUnderRoot(typeof(T).Name);

            if (itemsCollectionNode != null)
            {
                foreach (var element in itemsCollectionNode.Elements())
                {
                    var elementObject = element.ToObjectOfType<T>();

                    if (elementObject != null && item.Equals(elementObject))
                    {
                        found = element;
                        break;
                    }
                }
            }

            return found;
        }

        #endregion
    }
}
