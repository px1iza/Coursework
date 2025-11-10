using System;
using System.Collections.Generic;

public interface IDataProvider<T>
{
    void Save(List<T> items);
    List<T> Load();
}