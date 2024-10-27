using System;
using System.Collections.Generic;
using System.IO;
using Clonesoft.Json;
using ExSeIcOv.Core;
using ExSeIcOv.Core.Info;
using UnityEngine;

namespace ExSeIcOv.Models;

public class SectorSpecialOverrideConfig
{
    public Dictionary<char, TierEntry> ExpeditionTiers { get; set; } = new();

    [JsonIgnore]
    public IEnumerable<(eRundownTier Tier, TierEntry Entry)> Tiers
    {
        get
        {
            foreach (var tiers in ExpeditionTiers)
            {
                var tier = (eRundownTier) (tiers.Key - 64);
                yield return (tier, tiers.Value);
            }
        }
    }

    public bool TryGetData(eRundownTier tier, int expeditionIndex, out TierEntry.Overrides data)
    {
        if ((int)tier >= 27 || (int)tier <= 0)
            throw new ArgumentException("Invalid RundownTier", nameof(tier));
        
        var key = (char)(tier + 64);
        
        if (ExpeditionTiers.TryGetValue(key, out var value))
        {
            return value.TryGetData(expeditionIndex, out data);
        }

        data = null;
        return false;
    }
    
    public class TierEntry
    {
        public Dictionary<int, Overrides> ExpeditionOverrides { get; set; } = new();

        public bool TryGetData(int expeditionIndex, out Overrides value)
        {
            return ExpeditionOverrides.TryGetValue(expeditionIndex, out value);
        }

        public class Overrides
        {
            public LayerEntry Main { get; set; } = new();
            public LayerEntry Extreme { get; set; } = new();
            public LayerEntry Overload { get; set; } = new();
            public LayerEntry PrisonerEfficiency { get; set; } = new();
            
            public class LayerEntry
            {
                public string Skull { get; set; } = string.Empty;
                public string Background { get; set; } = string.Empty;
            }

            public LayerEntry GetLayer(SectorIconType layer)
            {
                return layer switch
                {
                    SectorIconType.None => null,
                    SectorIconType.Main => Main,
                    SectorIconType.Extreme => Extreme,
                    SectorIconType.Overload => Overload,
                    SectorIconType.PrisonerEfficiency => PrisonerEfficiency,
                    _ => throw new ArgumentException("Invalid Layer", nameof(layer))
                };
            }
            
            public void LoadSpritesInto(SectorIconOverride sectorIconOverride, string basePath)
            {
                for (int i = 1; i < Enum.GetNames<SectorIconType>().Length; i++)
                {
                    var layer = (SectorIconType)i;
                    
                    var entry = GetLayer(layer);

                    if (TryLoadImage(basePath, entry.Skull, out var skullSprite))
                    {
                        sectorIconOverride.SetSkull(layer, skullSprite);
                    }
                    
                    if (TryLoadImage(basePath, entry.Background, out var backgroundSprite))
                    {
                        sectorIconOverride.SetBackground(layer, backgroundSprite);
                    }
                }
            }

            private static bool TryLoadImage(string basePath, string fileName, out Sprite sprite)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    sprite = null;
                    return false;
                }
                
                var path = Path.Combine(basePath, fileName);
                
                if (!File.Exists(path))
                {
                    Plugin.L.LogError($"File at path does not exist: {path}");
                    sprite = null;
                    return false;
                }

                sprite = ImageLoader.LoadSprite(path);
                return true;
            }
        }
    }
}