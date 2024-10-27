using System;
using System.Collections.Generic;

namespace ExSeIcOv.Core;

public class HiINeedDataStoredPerExpeditionTooPlease<T> where T : class, new()
{
    private readonly Dictionary<string, T> _data = new();
    
    public string GetExpeditionKey(uint rundownID, eRundownTier expeditionTier, int expeditionIndex)
    {
        return $"{rundownID}_{expeditionTier}_{expeditionIndex}";
    }

    public void InsertData(string key, T data)
    {
        if (!_data.TryAdd(key, data))
            throw new ArgumentException();
    }
    
    public T GetOrCreateExpeditionData(string key)
    {
        if (_data.TryGetValue(key, out var data))
            return data;
        
        data = new T();
        _data.Add(key, data);

        return data;
    }

    public bool TryGetExpeditionData(string key, out T data)
    {
        _data.TryGetValue(key, out data);
        return data != null;
    }
}