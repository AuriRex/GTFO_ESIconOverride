using ExSeIcOv.Interfaces;
using ExSeIcOv.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExSeIcOv.Core.Info;
using UnityEngine;
using static ExSeIcOv.Extensions.ExtensionMethods;

namespace ExSeIcOv.Core;

public static class ImageLoader
{
    public static Sprite LoadSprite(string filePath)
    {
        LoadImageSprite(File.ReadAllBytes(filePath), out var sprite);
        sprite.name = "sprite_" + filePath.Replace("\\", ".").Replace("/", ".");
        return sprite;
    }

    public static Texture2D LoadTex2D(string filePath)
    {
        LoadImage(File.ReadAllBytes(filePath), out var tex);
        tex.name = "tex2d_" + filePath.Replace("\\", ".").Replace("/", ".");
        return tex;
    }

    public static void LoadImage(byte[] bytes, out Texture2D tex)
    {
        tex = new Texture2D(2, 2);
        tex.LoadImage(bytes, false);

        tex.DontDestroyAndSetHideFlags();
    }

    private static void LoadImageSprite(byte[] bytes, out Sprite sprite)
    {
        LoadImage(bytes, out var tex);

        sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        sprite.DontDestroyAndSetHideFlags();
    }
}
