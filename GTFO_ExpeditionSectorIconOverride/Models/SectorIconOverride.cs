using ExSeIcOv.Core;
using UnityEngine;

namespace ExSeIcOv.Models;

public class SectorIconOverride
{
    public SectorIconOverride()
    {
        
    }

    public SectorIconOverride(SectorSpecialOverrideConfig.TierEntry.Overrides configOverrides, string basePath)
    {
        configOverrides.LoadSpritesInto(this, basePath);
    }
    
    public Layer Main { get; set; } = new();
    public Layer Extreme { get; set; } = new();
    public Layer Overload { get; set; } = new();
    public Layer PrisonerEfficiency { get; set; } = new();

    public bool HasData => Main.HasData || Extreme.HasData || Overload.HasData || PrisonerEfficiency.HasData;

    public Sprite Get(SectorIconType type, bool skull)
    {
        if (!TryGetLayer(type, out var layer))
            return null;

        return skull ? layer.Skull : layer.Background;
    }

    public void SetSkull(SectorIconType type, Sprite sprite) => Set(type, skull: true, sprite);
    
    public void SetBackground(SectorIconType type, Sprite sprite) => Set(type, skull: false, sprite);
    
    public void Set(SectorIconType type, bool skull, Sprite sprite)
    {
        if (!TryGetLayer(type, out var layer))
            return;

        if (skull)
        {
            layer.Skull = sprite;
            return;
        }
            
        layer.Background = sprite;
    }

    private bool TryGetLayer(SectorIconType type, out Layer layer)
    {
        switch (type)
        {
            default:
            case SectorIconType.None:
                layer = null;
                return false;
            case SectorIconType.Main:
                layer = Main;
                break;
            case SectorIconType.Extreme:
                layer = Extreme;
                break;
            case SectorIconType.Overload:
                layer = Overload;
                break;
            case SectorIconType.PrisonerEfficiency:
                layer = PrisonerEfficiency;
                break;
        }
        return true;
    }

    public class Layer
    {
        public Sprite Skull { get; set; }
        public Sprite Background { get; set; }

        public bool HasData => Skull != null || Background != null;
    }
}