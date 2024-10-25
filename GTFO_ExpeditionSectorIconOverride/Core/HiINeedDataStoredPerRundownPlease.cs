using System.Collections.Generic;
using static CellMenu.CM_PageRundown_New;

namespace ExSeIcOv.Core;

public abstract class HiINeedDataStoredPerRundownPlease<T> where T : class, new()
{
    protected static readonly Dictionary<uint, T> _rundownDataDict = new();

    protected static T GetOrCreate(uint rundownId)
    {
        if (!_rundownDataDict.TryGetValue(rundownId, out T data))
        {
            data = new T();
            _rundownDataDict.Add(rundownId, data);
        }

        return data;
    }

    public static bool TryGetData(uint rundownId, out T data)
    {
        _rundownDataDict.TryGetValue(rundownId, out data);
        return data != null;
    }

    public static bool TryGetDataOrFallback(uint rundownId, out T data)
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

    public static bool TryGetActiveRundownID(out uint rundownID)
    {
        return uint.TryParse(RundownManager.ActiveRundownKey.Replace("Local_", string.Empty), out rundownID);
    }
}
