using System.Collections.Generic;
using System.Linq;
using ExSeIcOv.Core.Info;
using ExSeIcOv.Interfaces;

namespace ExSeIcOv.Core.Inspectors;

public abstract class ImageFileInspector : IFileInspector
{
    private static readonly HashSet<string> _validImageFileExtensions = new()
    {
        ".png",
        ".jpg",
        ".exr"
    };
    
    public abstract string FolderName { get; }

    public virtual void Init(uint rundownID, string path)
    {
        
    }

    public void InspectFile(uint rundownId, GenericFileInfo genericFile)
    {
        if (!_validImageFileExtensions.Any(ext => genericFile.FilePath.ToLower().EndsWith(ext)))
        {
            return;
        }
        
        var info = new ImageFileInfo(genericFile);
        
        InspectFile(rundownId, info);
    }

    public abstract void InspectFile(uint rundownId, ImageFileInfo file);
    
    public virtual void Finalize(uint rundownID)
    {
        
    }
}