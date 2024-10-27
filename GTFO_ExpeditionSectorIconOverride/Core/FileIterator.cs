using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExSeIcOv.Core.Info;
using ExSeIcOv.Interfaces;

namespace ExSeIcOv.Core;

public static class FileIterator
{
    public static T Register<T>() where T : class, new()
    {
        var instance = Activator.CreateInstance(typeof(T));
        Register(instance);
        return instance as T;
    }

    public static void Register(object instance)
    {
        var type = instance.GetType();
        if (!type.IsAssignableTo(typeof(IFileInspector)))
        {
            throw new ArgumentException($"Type \"{type.FullName}\" is invalid.");
        }

        if (_fileInspectors.Any(p => p.GetType() == type))
        {
            throw new ArgumentException($"Type \"{type.FullName}\" is already registered.");
        }
        
        _fileInspectors.Add(instance as IFileInspector);
    }

    private static readonly List<IFileInspector> _fileInspectors = new();
    

    private static string _assetsPath;
    public static string AssetsPath => _assetsPath ??= Path.Combine(BepInEx.Paths.BepInExRootPath, "Assets", Plugin.NAME);

    private static string _rundownFoldersPath;
    public static string RundownRootPath => _rundownFoldersPath ??= Path.Combine(AssetsPath, "Rundowns/");

    internal static void Init()
    {
        if (!Directory.Exists(RundownRootPath))
        {
            Directory.CreateDirectory(RundownRootPath);
        }

        IterateRundownRootFolder();
    }

    private static void IterateRundownRootFolder()
    {
        foreach (var dir in Directory.EnumerateDirectories(RundownRootPath))
        {
            var name = Path.GetFileName(dir);

            Plugin.L.LogInfo($"Inspecting path ({name}): {dir}");

            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!uint.TryParse(name, out var rundownID))
                continue;

            IterateRundownFolder(rundownID);
        }
    }

    private static void IterateRundownFolder(uint rundownID)
    {
        var pathRundownFolder = Path.Combine(RundownRootPath, $"{rundownID}/");

        if (!Directory.Exists(pathRundownFolder))
            return;

        foreach(var inspector in _fileInspectors)
        {
            var path = pathRundownFolder;
            var customFolderName = inspector.FolderName;

            if (!string.IsNullOrWhiteSpace(customFolderName))
            {
                path = Path.Combine(pathRundownFolder, $"{customFolderName}/");
            }

            if (!Directory.Exists(path))
                continue;

            try
            {
                inspector.Init(rundownID, path);
            }
            catch (Exception ex)
            {
                LogError($"{inspector.GetType().FullName}.{nameof(IFileInspector.Init)}", ex);
            }

            foreach (var filePath in Directory.EnumerateFiles(path))
            {
                var info = new GenericFileInfo(filePath);

                try
                {
                    inspector.InspectFile(rundownID, info);
                }
                catch (Exception ex)
                {
                    LogError($"{inspector.GetType().FullName}.{nameof(IFileInspector.InspectFile)}", ex);
                }
            }

            try
            {
                inspector.Finalize(rundownID);
            }
            catch (Exception ex)
            {
                LogError($"{inspector.GetType().FullName}.{nameof(IFileInspector.Finalize)}", ex);
            }
        }
    }

    private static void LogError(string method, Exception exception)
    {
        Plugin.L.LogError($"{exception.GetType().Name} has occured in {method}.");
        Plugin.L.LogError(exception.Message);
        Plugin.L.LogWarning(exception.StackTrace);
    }
}