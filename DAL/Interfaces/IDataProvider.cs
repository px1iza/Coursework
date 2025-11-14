using System;
using System.Collections.Generic;
namespace DAL.Interfaces
{
    public interface IDataProvider<T>
    {
        void Save(List<T> items);
        List<T> Load();
    }
}