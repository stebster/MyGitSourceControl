namespace Stebster.Core.Repository.Interface
{
    using System.Collections.Generic;

    public interface IXmlRepository
    {
        string SavePath { get; }

        void PutItems<T>(IEnumerable<T> items, string toItemsNodeName); 

        IList<T> GetItems<T>(string fromItemNodeName);

        void RemoveItems<T>(IEnumerable<T> items, string fromItemNodeName);
    }
}
