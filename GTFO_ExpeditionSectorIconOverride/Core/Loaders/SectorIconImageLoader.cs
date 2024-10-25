using ExSeIcOv.Interfaces;
using ExSeIcOv.Models;
using UnityEngine;

namespace ExSeIcOv.Core.Loaders;

internal class SectorIconImageLoader : HiINeedDataStoredPerRundownPlease<SectorOverrideImageData>, IImageFileInspector
{
    public const string SKULL = "skull";
    public const string BG = "bg";

    public const string MAIN = "main";
    public const string SECONDARY = "secondary";
    public const string OVERLOAD = "overload";
    public const string PE = "pe";

    public const string RUNDOWN_TIER_MARKER = "rtm_";

    private static readonly SectorOverrideImageData.SectorIconOverride _baseGameSprites = new();

    public string FolderName => "SectorOverride";

    public void InspectFile(uint rundownId, ImageFileInfo file)
    {
        var dataRaw = GetOrCreate(rundownId);
        var name = file.FileNameLower;

        var data = dataRaw.Override;

        if (name.StartsWith(RUNDOWN_TIER_MARKER))
        {
            name = name.Substring(RUNDOWN_TIER_MARKER.Length);
            data = dataRaw.RundownTierMarker;
        }

        switch(name)
        {
            case $"{SKULL}_{MAIN}":
                data.Main.Skull = file.LoadAsSprite();
                break;
            case $"{SKULL}_{SECONDARY}":
                data.Extreme.Skull = file.LoadAsSprite();
                break;
            case $"{SKULL}_{OVERLOAD}":
                data.Overload.Skull = file.LoadAsSprite();
                break;
            case $"{SKULL}_{PE}":
                data.PrisonerEfficiency.Skull = file.LoadAsSprite();
                break;

            case $"{BG}_{MAIN}":
                data.Main.Background = file.LoadAsSprite();
                break;
            case $"{BG}_{SECONDARY}":
                data.Extreme.Background = file.LoadAsSprite();
                break;
            case $"{BG}_{OVERLOAD}":
                data.Overload.Background = file.LoadAsSprite();
                break;
            case $"{BG}_{PE}":
                data.PrisonerEfficiency.Background = file.LoadAsSprite();
                break;
        }
    }

    public void Finalize(uint rundownID)
    {
        if (!TryGetData(rundownID, out var data))
            return;

        if (!data.HasData)
        {
            _rundownDataDict.Remove(rundownID);
        }
    }

    public static void ApplySkull(SectorIconType type, SpriteRenderer rendererSkull, bool isRundownTierMarker)
    {
        Apply(type, rendererSkull, isRundownTierMarker, isSkull: true);
    }

    public static void ApplyBackground(SectorIconType type, SpriteRenderer rendererBG, bool isRundownTierMarker)
    {
        Apply(type, rendererBG, isRundownTierMarker, isSkull: false);
    }

    internal static bool ExpeditionDetailsWindowActive { get; set; }
    public static eRundownTier ExpeditionTier { get; internal set; }
    public static int ExpeditionIndex { get; internal set; }

    private static void Apply(SectorIconType type, SpriteRenderer renderer, bool isRundownTierMarker, bool isSkull)
    {
        TrySetBaseGameSprites(type, renderer, isSkull);

        if (!TryGetActiveRundownID(out var rundownID))
        {
            return;
        }

        var spriteData = _baseGameSprites;
        if (TryGetDataOrFallback(rundownID, out var data))
        {
            spriteData = isRundownTierMarker ? data.RundownTierMarker : data.Override;
        }

        var sprite = spriteData.Get(type, isSkull);

        if (sprite == null)
        {
            sprite = _baseGameSprites.Get(type, isSkull);
        }

        //Plugin.L.LogDebug($"Assigning Sprite: {sprite.name}");

        renderer.sprite = sprite;
    }

    private static void TrySetBaseGameSprites(SectorIconType type, SpriteRenderer renderer, bool isSkull)
    {
        if (_baseGameSprites.Get(type, isSkull) == null)
            _baseGameSprites.Set(type, isSkull, renderer.sprite);
    }
}
