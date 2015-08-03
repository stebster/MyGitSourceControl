namespace Stebster.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Interface;

    public class XmlRepository : IXmlRepository
    {
        #region Private Member Variables

        private const string RootNode = "Repository";
        private readonly string _directory;
        private XDocument _xDocument;

        #endregion

        #region Public Properties

        public string SavePath { get; private set; }

        #endregion

        #region Constructor

        public XmlRepository(string savePath)
        {
            SavePath = savePath;
            var di = new DirectoryInfo(savePath);
            _directory = di.FullName;
            Initialise();
        }

        #endregion

        #region IXmlRepository Members

        public void PutItems<T>(IEnumerable<T> items, string toItemsNodeName)
        {
            throw new NotImplementedException();
        }

        public IList<T> GetItems<T>(string fromItemNodeName)
        {
            throw new NotImplementedException();
        }

        public void RemoveItems<T>(IEnumerable<T> items, string fromItemNodeName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Helpers

        private void Initialise()
        {
            // Save directory should exist
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            // And the xml document should exist
            if (!File.Exists(SavePath))
            {
                ResetIt();
                SaveIt();
            }

            LoadIt();
        }

        private bool SaveIt()
        {
            try
            {
                _xDocument.Save(SavePath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        private void LoadIt()
        {
            _xDocument = XDocument.Load(SavePath);
        }

        private void ResetIt()
        {
            _xDocument = new XDocument(new XElement(RootNode));
        }

        #endregion
    }
}
