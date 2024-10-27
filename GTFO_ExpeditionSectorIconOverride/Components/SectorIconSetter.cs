using ExSeIcOv.Core;
using ExSeIcOv.Core.Loaders;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using UnityEngine;

namespace ExSeIcOv.Components;

internal class SectorIconSetter : MonoBehaviour
{
    [HideFromIl2Cpp]
    public void Setup(SectorIconType type, SpriteRenderer skull, SpriteRenderer background, bool onRundownScreen = false)
    {
        Type = type;

        RendererSkull = skull;
        RendererBG = background;

        IsRundownTierMarker = onRundownScreen;

        //Plugin.L.LogWarning($"Setup: {Type}, isRTM:{IsRundownTierMarker}");
    }

    [HideFromIl2Cpp]
    public SectorIconType Type
    {
        get => (SectorIconType)_typeIL2CPP.Get();
        set => _typeIL2CPP.Set((int)value);
    }

    [HideFromIl2Cpp]
    private bool IsRundownTierMarker
    {
        get => _isRundownTierMarkerIL2CPP.Get();
        set => _isRundownTierMarkerIL2CPP.Set(value);
    }

    [HideFromIl2Cpp]
    private SpriteRenderer RendererBG
    {
        get => _rendererBGIL2CPP.Get();
        set => _rendererBGIL2CPP.Set(value);
    }

    [HideFromIl2Cpp]
    private SpriteRenderer RendererSkull
    {
        get => _rendererSkullIL2CPP.Get();
        set => _rendererSkullIL2CPP.Set(value);
    }

    public Il2CppValueField<int> _typeIL2CPP = null!;
    public Il2CppValueField<bool> _isRundownTierMarkerIL2CPP = null!;
    public Il2CppReferenceField<SpriteRenderer> _rendererBGIL2CPP = null!;
    public Il2CppReferenceField<SpriteRenderer> _rendererSkullIL2CPP = null!;

    //private SectorIconType _type;
    //private SpriteRenderer _rendererBG;
    //private SpriteRenderer _rendererSkull;
    //private bool _isRundownTierMarker;

    public void Awake()
    {
        AssignSprites();
    }

    public void AssignSprites()
    {
        if (Type == SectorIconType.None)
            return;

        if (name.StartsWith(Plugin.PREVENT_OVERRIDE_NAME_PREFIX))
        {
            Destroy(this);
            return;
        }
        
        //Plugin.L.LogDebug($"DoThing: {Type}, isRTM:{IsRundownTierMarker}");

        if (RendererSkull != null)
            SectorIconImageLoader.ApplySkull(Type, RendererSkull, IsRundownTierMarker);

        if (RendererBG != null)
            SectorIconImageLoader.ApplyBackground(Type, RendererBG, IsRundownTierMarker);
    }
}
