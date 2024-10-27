using ExSeIcOv.Core.Info;
using ExSeIcOv.Core.Inspectors;
using ExSeIcOv.Models;
using RundownStorage = ExSeIcOv.Core.HiINeedDataStoredPerRundownPlease<ExSeIcOv.Models.SectorSpecialOverrideConfig>;

namespace ExSeIcOv.Core.Loaders;

public class SectorIconConfigLoader : ConfigFileInspector
{
    public const string CONFIG_FILE_NAME = "sectoriconconfig";
    
    private static RundownStorage _rundownStorage;
    
    public SectorIconConfigLoader()
    {
        _rundownStorage = new RundownStorage();
    }
    
    public override string FolderName => SectorIconImageLoader.SECTOR_OVERRIDE_FOLDER;
    
    public override void InspectFile(uint rundownId, ConfigFileInfo file)
    {
        var name = file.FileNameLower;

        if (name != CONFIG_FILE_NAME)
            return;
        
        Plugin.L.LogInfo($"Loading config file '{file.FileName}' for rundown '{rundownId}'");
        
        _rundownStorage.InsertData(rundownId, file.LoadAsJSONConfig<SectorSpecialOverrideConfig>());
    }

    public static bool TryGetConfig(uint rundownId, out SectorSpecialOverrideConfig config)
    {
        return _rundownStorage.TryGetData(rundownId, out config);
    }
}