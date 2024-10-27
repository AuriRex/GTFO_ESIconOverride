using System.IO;

namespace ExSeIcOv.Core.Info;

public class GenericFileInfo
{
    public string FileName { get; init; }
    public string FileNameLower => FileName.ToLower();
    public string FilePath { get; init; }

    public GenericFileInfo(string filePath)
    {
        FileName = Path.GetFileNameWithoutExtension(filePath);
        FilePath = filePath;
    }
}