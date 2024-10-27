using ExSeIcOv.Core;
using ExSeIcOv.Core.Info;

namespace ExSeIcOv.Interfaces;

public interface IFileInspector
{
    /// <summary>
    /// The sub-folder to iterate through
    /// </summary>
    string FolderName { get; }
    
    void Init(uint rundownID, string path);
    
    void InspectFile(uint rundownId, GenericFileInfo genericFile);

    /// <summary>
    /// Called once all files in the per-rundown folder have been inspected
    /// </summary>
    /// <param name="rundownID"></param>
    void Finalize(uint rundownID);

}