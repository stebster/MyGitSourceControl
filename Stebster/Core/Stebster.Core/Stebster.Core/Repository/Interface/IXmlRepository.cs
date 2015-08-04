namespace Stebster.Core.Repository.Interface
{
    using System;
    using System.Collections.Generic;

    public interface IXmlRepository
    {
        bool SaveItem<T>(T item) where T : class, IEquatable<T>;

        IList<T> ReadItems<T>() where T : class, IEquatable<T>; 

        bool DeleteItem<T>(T item) where T : class, IEquatable<T>;

        T GetItem<T>(T item) where T : class, IEquatable<T>;
    }
}
