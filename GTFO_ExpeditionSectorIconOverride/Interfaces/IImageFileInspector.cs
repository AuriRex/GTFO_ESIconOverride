using ExSeIcOv.Core;

namespace ExSeIcOv.Interfaces;

public interface IImageFileInspector
{
    /// <summary>
    /// The sub-folder to iterate through
    /// </summary>
    string FolderName { get; }

    void InspectFile(uint rundownId, ImageFileInfo file);

    /// <summary>
    /// Called once all files in the per-rundown folder have been inspected
    /// </summary>
    /// <param name="rundownID"></param>
    void Finalize(uint rundownID);
}
