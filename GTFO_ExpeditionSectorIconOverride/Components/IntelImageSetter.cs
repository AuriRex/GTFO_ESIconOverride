using ExSeIcOv.Core;
using ExSeIcOv.Core.Loaders;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using UnityEngine;

namespace ExSeIcOv.Components;

internal class IntelImageSetter : MonoBehaviour
{
    [HideFromIl2Cpp]
    public void SetType(IntelImageType type)
    {
        Type = type;
        typeAsInt.Set((int)type);
    }

    [HideFromIl2Cpp]
    public IntelImageType Type { get; private set; }

    public Il2CppValueField<int> typeAsInt = null!;

    private SpriteRenderer _renderer;

    public void Awake()
    {
        Type = (IntelImageType)typeAsInt.Get();

        _renderer = GetComponent<SpriteRenderer>();

        RundownIntelImageLoader.ApplyRundownIntelImage(Type, _renderer);
    }
}
