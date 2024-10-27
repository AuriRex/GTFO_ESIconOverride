using ExSeIcOv.Core.Info;
using ExSeIcOv.Core.Inspectors;
using ExSeIcOv.Interfaces;
using ExSeIcOv.Models;
using JetBrains.Annotations;
using UnityEngine;

using ExpeditionStorage = ExSeIcOv.Core.HiINeedDataStoredPerExpeditionTooPlease<ExSeIcOv.Models.SectorIconOverride>;
using RundownStorage = ExSeIcOv.Core.HiINeedDataStoredPerRundownPlease<ExSeIcOv.Models.SectorOverrideImageData>;

namespace ExSeIcOv.Core.Loaders;

internal class SectorIconImageLoader : ImageFileInspector
{
    public const string SECTOR_OVERRIDE_FOLDER = "SectorOverride";
    
    public const string SKULL = "skull";
    public const string BG = "bg";

    public const string MAIN = "main";
    public const string SECONDARY = "secondary";
    public const string OVERLOAD = "overload";
    public const string PE = "pe";

    public const string RUNDOWN_TIER_MARKER = "rtm_";

    private static readonly SectorIconOverride _baseGameSprites = new();

    private static RundownStorage _rundownStorage;
    private static ExpeditionStorage _expeditionStorage;
    
    public SectorIconImageLoader()
    {
        _rundownStorage = new RundownStorage();
        _expeditionStorage = new ExpeditionStorage();
    }
    
    public override string FolderName => SECTOR_OVERRIDE_FOLDER;

    private bool HasConfig => _config != null;
    private SectorSpecialOverrideConfig _config;
    private string _basePath;

    public override void Init(uint rundownID, string path)
    {
        SectorIconConfigLoader.TryGetConfig(rundownID, out _config);
        _basePath = path;
    }

    public override void InspectFile(uint rundownId, ImageFileInfo file)
    {
        var dataWrapper = _rundownStorage.GetOrCreate(rundownId);
        var data = dataWrapper.Override;
        
        var name = file.FileNameLower;
        
        if (name.StartsWith(RUNDOWN_TIER_MARKER))
        {
            name = name.Substring(RUNDOWN_TIER_MARKER.Length);
            data = dataWrapper.RundownTierMarker;
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

        if (HasConfig)
        {
            DoConfigThingies(rundownId);
        }
    }

    private void DoConfigThingies(uint rundownId)
    {
        foreach (var (expeditionTier, entry) in _config.Tiers)
        {
            foreach (var (expeditionIndex, expeditionConfigOverride) in entry.ExpeditionOverrides)
            {
                var key = _expeditionStorage.GetExpeditionKey(rundownId, expeditionTier, expeditionIndex);

                var data = _expeditionStorage.GetOrCreateExpeditionData(key);
                
                expeditionConfigOverride.LoadSpritesInto(data, _basePath);
            }
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

    public static void ApplySkull(SectorIconType type, SpriteRenderer rendererSkull, bool isRundownTierMarker)
    {
        Apply(type, rendererSkull, isRundownTierMarker, isSkull: true);
    }

    public static void ApplyBackground(SectorIconType type, SpriteRenderer rendererBG, bool isRundownTierMarker)
    {
        Apply(type, rendererBG, isRundownTierMarker, isSkull: false);
    }

    internal static bool IsExpeditionDetailsWindowActive { get; set; }
    internal static bool IsInExpedition { get; set; }
    internal static bool UseExpeditionSprites => IsInExpedition || IsExpeditionDetailsWindowActive;
    public static eRundownTier ExpeditionTier { get; internal set; }
    public static int ExpeditionIndex { get; internal set; }

    private static void Apply(SectorIconType type, SpriteRenderer renderer, bool isRundownTierMarker, bool isSkull)
    {
        TrySetBaseGameSprites(type, renderer, isSkull);

        if (!Utils.TryGetActiveRundownID(out var rundownID))
        {
            return;
        }

        Sprite sprite = null;

        if (UseExpeditionSprites)
        {
            var key = _expeditionStorage.GetExpeditionKey(rundownID, ExpeditionTier, ExpeditionIndex);

            if (_expeditionStorage.TryGetExpeditionData(key, out var spritesForExpedition))
            {
                sprite = spritesForExpedition.Get(type, isSkull);
            }
        }
        
        if (sprite == null)
        {
            var spriteData = _baseGameSprites;
            if (_rundownStorage.TryGetDataOrFallback(rundownID, out var data))
            {
                spriteData = isRundownTierMarker ? data.RundownTierMarker : data.Override;
            }

            sprite = spriteData.Get(type, isSkull);
        }

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
