using System;
using System.Collections.Generic;
using static CellMenu.CM_PageRundown_New;

namespace ExSeIcOv.Core;

public class HiINeedDataStoredPerRundownPlease<T> where T : class, new()
{
    private readonly Dictionary<uint, T> _rundownDataDict = new();

    public T GetOrCreate(uint rundownId)
    {
        if (_rundownDataDict.TryGetValue(rundownId, out var data))
            return data;
        
        data = new T();
        _rundownDataDict.Add(rundownId, data);

        return data;
    }

    public void InsertData(uint rundownId, T data)
    {
        if (!_rundownDataDict.TryAdd(rundownId, data))
            throw new ArgumentException();
    }
    
    public bool TryGetData(uint rundownId, out T data)
    {
        _rundownDataDict.TryGetValue(rundownId, out data);
        return data != null;
    }

    public bool TryGetDataOrFallback(uint rundownId, out T data)
    {
        if (TryGetData(rundownId, out data))
        {
            return true;
        }

        if (TryGetData(0, out data))
        {
            return true;
        }

        return false;
    }

    public void Remove(uint rundownID)
    {
        _rundownDataDict.Remove(rundownID);
    }
}
