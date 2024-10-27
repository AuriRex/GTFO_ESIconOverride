using System.Collections.Generic;
using System.Linq;
using ExSeIcOv.Core.Info;
using ExSeIcOv.Interfaces;

namespace ExSeIcOv.Core.Inspectors;

public abstract class ConfigFileInspector : IFileInspector
{
    private static readonly HashSet<string> _validExtensions = new()
    {
        ".json",
        ".jsonc",
    };
    
    public abstract string FolderName { get; }
    
    public virtual void Init(uint rundownID, string path)
    {
        
    }
    
    public void InspectFile(uint rundownId, GenericFileInfo genericFile)
    {
        if (!_validExtensions.Any(ext => genericFile.FilePath.ToLower().EndsWith(ext)))
        {
            return;
        }
        
        var info = new ConfigFileInfo(genericFile);
        
        InspectFile(rundownId, info);
    }

    public abstract void InspectFile(uint rundownId, ConfigFileInfo file);
    
    public virtual void Finalize(uint rundownID)
    {
        
    }
}