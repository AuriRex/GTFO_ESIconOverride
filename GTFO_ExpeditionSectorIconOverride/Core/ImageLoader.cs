using ExSeIcOv.Interfaces;
using ExSeIcOv.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static ExSeIcOv.Extensions.ExtensionMethods;

namespace ExSeIcOv.Core;

public static class ImageLoader
{
    public static void Register<T>()
    {
        if (!typeof(T).IsAssignableTo(typeof(IImageFileInspector)))
        {
            throw new ArgumentException($"Type \"{typeof(T).FullName}\" is invalid.");
        }

        if (_imageProcessors.Any(p => p.GetType() == typeof(T)))
        {
            throw new ArgumentException($"Type \"{typeof(T).FullName}\" is already registered.");
        }

        _imageProcessors.Add(Activator.CreateInstance(typeof(T)) as IImageFileInspector);
    }

    private static readonly HashSet<IImageFileInspector> _imageProcessors = new();

    private static readonly HashSet<string> _validImageFileExtensions = new()
    {
        ".png",
        ".jpg",
        ".exr"
    };

    private static string _assetsPath;
    public static string AssetsPath => _assetsPath ??= Path.Combine(BepInEx.Paths.BepInExRootPath, "Assets", Plugin.NAME);

    private static string _rundownIntelPath;
    //public static string RundownIntelPath => _rundownIntelPath ??= Path.Combine(AssetsPath, "RundownIntel/");
    public static string TLRundownPath => _rundownIntelPath ??= Path.Combine(AssetsPath, "Rundowns/");

    internal static void Init()
    {
        if (!Directory.Exists(TLRundownPath))
        {
            Directory.CreateDirectory(TLRundownPath);
        }

        IterateRundownFolders();
    }

    private static void IterateRundownFolders()
    {
        foreach (var dir in Directory.EnumerateDirectories(TLRundownPath))
        {
            var name = Path.GetFileName(dir);

            Plugin.L.LogInfo($"Inspecting path ({name}): {dir}");

            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!uint.TryParse(name, out var rundownID))
                continue;

            LoadImagesFor(rundownID);
        }
    }

    private static void LoadImagesFor(uint rundownID)
    {
        var pathRundownFolder = Path.Combine(TLRundownPath, $"{rundownID}/");

        if (!Directory.Exists(pathRundownFolder))
            return;

        foreach(var processor in _imageProcessors)
        {
            var path = pathRundownFolder;
            var customFolderName = processor.FolderName;

            if (!string.IsNullOrWhiteSpace(customFolderName))
            {
                path = Path.Combine(pathRundownFolder, $"{customFolderName}/");
            }

            if (!Directory.Exists(path))
                continue;

            foreach (var filePath in Directory.EnumerateFiles(path))
            {
                if (!_validImageFileExtensions.Any(ext => filePath.ToLower().EndsWith(ext)))
                {
                    continue;
                }

                var info = new ImageFileInfo(filePath);

                processor.InspectFile(rundownID, info);
            }

            processor.Finalize(rundownID);
        }
    }

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
