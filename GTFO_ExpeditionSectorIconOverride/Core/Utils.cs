namespace ExSeIcOv.Core;

public static class Utils
{
    public static bool TryGetActiveRundownID(out uint rundownID)
    {
        return uint.TryParse(RundownManager.ActiveRundownKey.Replace("Local_", string.Empty), out rundownID);
    }
}