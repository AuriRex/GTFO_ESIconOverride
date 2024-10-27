using System.IO;
using Clonesoft.Json;

namespace ExSeIcOv.Core.Info;

public class ConfigFileInfo : GenericFileInfo
{
    public ConfigFileInfo(string filePath) : base(filePath)
    {
    }

    public ConfigFileInfo(GenericFileInfo fileInfo) : base(fileInfo.FilePath)
    {
    }

    public T LoadAsJSONConfig<T>()
    {
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
    }
}