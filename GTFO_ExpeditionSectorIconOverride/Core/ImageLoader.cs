using System.IO;
using UnityEngine;
using static ExSeIcOv.Extensions.ExtensionMethods;

namespace ExSeIcOv.Core;

public static class ImageLoader
{
    private static readonly Cache<Texture2D> _textureCache = new();
    private static readonly Cache<Sprite> _spriteCache = new();
    
    public static Sprite LoadSprite(string filePath, bool useCache = true)
    {
        if (useCache && _spriteCache.TryGetCached(filePath, out var cachedSprite))
            return cachedSprite;
        
        LoadNewImageSprite(File.ReadAllBytes(filePath), out var sprite);
        sprite.name = "sprite_" + filePath.Replace("\\", ".").Replace("/", ".");
        _spriteCache.DoCache(filePath, sprite);
        Plugin.L.LogInfo($"Loaded sprite {sprite.name}");
        return sprite;
    }

    public static Texture2D LoadTex2D(string filePath, bool useCache = true)
    {
        if (useCache && _textureCache.TryGetCached(filePath, out var cachedTexture))
            return cachedTexture;
        
        LoadNewImage(File.ReadAllBytes(filePath), out var tex);
        tex.name = "tex2d_" + filePath.Replace("\\", ".").Replace("/", ".");
        _textureCache.DoCache(filePath, tex);
        Plugin.L.LogInfo($"Loaded texture {tex.name}");
        return tex;
    }

    public static void LoadNewImage(byte[] bytes, out Texture2D tex)
    {
        tex = new Texture2D(2, 2);
        tex.LoadImage(bytes, false);

        tex.DontDestroyAndSetHideFlags();
    }

    public static void LoadNewImageSprite(byte[] bytes, out Sprite sprite)
    {
        LoadNewImage(bytes, out var tex);

        sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        sprite.DontDestroyAndSetHideFlags();
    }
}
