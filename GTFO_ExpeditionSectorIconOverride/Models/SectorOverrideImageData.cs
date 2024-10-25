using ExSeIcOv.Core;
using System;
using UnityEngine;

namespace ExSeIcOv.Models;

public class SectorOverrideImageData
{
    public SectorIconOverride Override = new();
    public SectorIconOverride RundownTierMarker = new();

    public bool HasData => Override.HasData || RundownTierMarker.HasData;

    public class SectorIconOverride
    {
        public Layer Main = new();
        public Layer Extreme = new();
        public Layer Overload = new();
        public Layer PrisonerEfficiency = new();

        public bool HasData => Main.HasData || Extreme.HasData || Overload.HasData || PrisonerEfficiency.HasData;

        public Sprite Get(SectorIconType type, bool skull)
        {
            if (!TryGetLayer(type, out var layer))
                return null;

            return skull ? layer.Skull : layer.Background;
        }

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
            public Sprite Skull;
            public Sprite Background;

            public bool HasData => Skull != null || Background != null;
        }
    }
}
