using ExSeIcOv.Core.Info;
using ExSeIcOv.Core.Inspectors;
using UnityEngine;

using RundownStorage = ExSeIcOv.Core.HiINeedDataStoredPerRundownPlease<ExSeIcOv.Models.IntelImageData>;

namespace ExSeIcOv.Core.Loaders;

internal class RundownIntelImageLoader : ImageFileInspector
{
    private static RundownStorage _rundownStorage;
    
    public RundownIntelImageLoader()
    {
        _rundownStorage = new RundownStorage();
    }
    
    public override string FolderName => "Intel";

    public override void InspectFile(uint rundownId, ImageFileInfo file)
    {
        var data = _rundownStorage.GetOrCreate(rundownId);
        var name = file.FileNameLower;

        switch(name)
        {
            case "intel_top":
                data.Top = file.LoadAsSprite();
                break;
            case "intel_mid":
                data.Middle = file.LoadAsSprite();
                break;
            case "intel_bot":
                data.Bottom = file.LoadAsSprite();
                break;
            default:
                break;
        }
    }

    public override void Finalize(uint rundownID)
    {
        if (!_rundownStorage.TryGetData(rundownID, out var data))
            return;

        if (!data.HasData)
        {
            _rundownStorage.Remove(rundownID);
        }
    }

    internal static void ApplyRundownIntelImage(IntelImageType type, SpriteRenderer renderer)
    {
        if (!Utils.TryGetActiveRundownID(out var rundownID))
        {
            Plugin.L.LogError($"Could not parse {nameof(RundownManager.ActiveRundownKey)}: \"{RundownManager.ActiveRundownKey}\"");
            return;
        }

        if (!_rundownStorage.TryGetDataOrFallback(rundownID, out var data))
        {
            return;
        }

        Sprite image = null;
        switch (type)
        {
            case IntelImageType.Top:
                image = data.Top;
                break;
            case IntelImageType.Middle:
                image = data.Middle;
                break;
            case IntelImageType.Bottom:
                image = data.Bottom;
                break;
            default:
                Plugin.L.LogError($"Unsupported {nameof(IntelImageType)} passed with value: {type}");
                return;
        }

        if (image == null)
        {
            Plugin.L.LogInfo($"No Image for Rundown {rundownID}, Type {type} found, ignoring.");
            renderer.enabled = false;
            return;
        }

        renderer.sprite = image;
        renderer.enabled = true;
    }
}
