using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExSeIcOv.Components;
using ExSeIcOv.Core;
using ExSeIcOv.Core.Loaders;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion(ExSeIcOv.Plugin.VERSION)]
[assembly: AssemblyFileVersion(ExSeIcOv.Plugin.VERSION)]
[assembly: AssemblyInformationalVersion(ExSeIcOv.Plugin.VERSION)]

namespace ExSeIcOv;

[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BasePlugin
{
    public const string GUID = "dev.aurirex.gtfo.exseicov";
    public const string NAME = "ExSeIcOv";
    public const string NAME_FULL = "ExpeditionSectorIconOverride";
    public const string VERSION = "0.0.1";

    internal static ManualLogSource L;

    private static Harmony _harmony;

    public override void Load()
    {
        L = Log;
        Log.LogMessage($"Loading {NAME_FULL}");

        ClassInjector.RegisterTypeInIl2Cpp<IntelImageSetter>();
        ClassInjector.RegisterTypeInIl2Cpp<SectorIconSetter>();

        /*var monoBehaviourTypes = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly()).Where(t => t.IsAssignableTo(typeof(MonoBehaviour))).ToArray();
        Log.LogDebug($"Found {monoBehaviourTypes.Length} custom {nameof(MonoBehaviour)}s. Registering ...");
        foreach(var type in monoBehaviourTypes)
        {
            ClassInjector.RegisterTypeInIl2Cpp(type);
        }*/

        _harmony = new Harmony(GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    internal static void Init()
    {
        L.LogInfo("Loading Image assets ...");
        ImageLoader.Register<RundownIntelImageLoader>();
        ImageLoader.Register<SectorIconImageLoader>();
        ImageLoader.Init();
        L.LogInfo("Image asset loading complete!");
    }
}
