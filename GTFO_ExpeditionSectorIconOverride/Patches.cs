using CellMenu;
using ExSeIcOv.Components;
using ExSeIcOv.Core;
using ExSeIcOv.Core.Loaders;
using ExSeIcOv.Extensions;
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;

namespace ExSeIcOv;

[HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Awake))]
public static class StartMainGame_Awake_Patch
{
    public static void Postfix()
    {
        Plugin.Init();
    }
}

[HarmonyPatch(typeof(GlobalPopupMessageManager), nameof(GlobalPopupMessageManager.Setup))]
public class GlobalPopupMessageManager_Setup_Patch
{
    public static void Postfix(GlobalPopupMessageManager __instance)
    {
        

        var prefabMap = GlobalPopupMessageManager.m_popupTypeToPrefabMap;

        if (prefabMap == null)
        {
            Plugin.L.LogError($"Popup Prefab Map is null?!??!!");
            return;
        }

        var prefab = prefabMap[PopupType.RundownInfo];

        if (prefab == null)
        {
            Plugin.L.LogError($"{nameof(PopupType.RundownInfo)} Popup Prefab could not be found?!??!!");
            return;
        }

        var child = prefab.transform.FindChild("ContentGroup");

        foreach (var contentChild in child.Children())
        {
            IntelImageType type = IntelImageType.None;
            switch (contentChild.name)
            {
                case "IntelPicture_Muted":
                    type = IntelImageType.Top;
                    break;
                case "IntelPicture_Bold":
                    type = IntelImageType.Middle;
                    break;
                case "IntelPicture_Aggressive":
                    type = IntelImageType.Bottom;
                    break;
                default:
                    break;
            }

            if (type != IntelImageType.None)
            {
                var setter = contentChild.gameObject.GetOrAddComponent<IntelImageSetter>();
                setter.SetType(type);
            }
        }
    }
}

//CM_ExpeditionWindow
//public void Setup(CM_PageRundown_New basePage)
[HarmonyPatch(typeof(CM_ExpeditionWindow), nameof(CM_ExpeditionWindow.Setup))]
public class CM_ExpeditionWindow_Setup_Patch
{
    public static bool InMethod { get; private set; }

    public static void Prefix()
    {
        InMethod = true;
        SectorIconImageLoader.ExpeditionDetailsWindowActive = true;
    }

    public static void Postfix()
    {
        InMethod = false;
        SectorIconImageLoader.ExpeditionDetailsWindowActive = false;
    }
}

//CM_ExpeditionSectorIcon
//public void Setup(LG_LayerType type, Transform root, bool visible, bool cleared, bool titleVisible = true, float alphaMaxBG = 0.5f, float alphaMaxSkull = 0.8f)
[HarmonyPatch(typeof(CM_ExpeditionSectorIcon), nameof(CM_ExpeditionSectorIcon.Setup))]
public class CM_ExpeditionSectorIcon_Setup_Patch
{
    // Called for Main, Extreme and Overload
    public static void Postfix(CM_ExpeditionSectorIcon __instance, LevelGeneration.LG_LayerType type)
    {
#warning TODO

        var sectorIconType = (SectorIconType)((int)type + 1);

        /*
         * Set Icon based off of json file per expedition
         */
        SpriteRenderer skull;
        SpriteRenderer bg;
        switch (sectorIconType)
        {
            default:
            case SectorIconType.Main:
                skull = __instance.m_iconMainSkull;
                bg = __instance.m_iconMainBG;
                break;
            case SectorIconType.Extreme:
                skull = __instance.m_iconSecondarySkull;
                bg = __instance.m_iconSecondaryBG;
                break;
            case SectorIconType.Overload:
                skull = __instance.m_iconThirdSkull;
                bg = __instance.m_iconThirdBG;
                break;
        }

        AddSectorSetterComponent(__instance, sectorIconType, skull, bg);
    }

    internal static void AddSectorSetterComponent(CM_ExpeditionSectorIcon __instance, SectorIconType sectorIconType, SpriteRenderer skull, SpriteRenderer bg)
    {
        var isRundownTierMarker = null != __instance.gameObject.GetComponentInParents<CM_RundownTierMarker>();

        var iconSetter = __instance.gameObject.GetOrAddComponent<SectorIconSetter>();
        iconSetter.Setup(sectorIconType, skull, bg, onRundownScreen: isRundownTierMarker);
        iconSetter.AssignSprites();
    }
}

[HarmonyPatch(typeof(CM_ExpeditionSectorIcon), nameof(CM_ExpeditionSectorIcon.SetupAsFinishedAll))]
public class CM_ExpeditionSectorIcon_SetupAsFinishedAll_Patch
{
    // Called PE exclusively
    public static void Postfix(CM_ExpeditionSectorIcon __instance)
    {
        var iconType = SectorIconType.PrisonerEfficiency;
        var skull = __instance.m_iconFinishedAllSkull;
        var bg = __instance.m_iconFinishedAllBG;

        CM_ExpeditionSectorIcon_Setup_Patch.AddSectorSetterComponent(__instance, iconType, skull, bg);
    }
}

//public void SetExpeditionInfo(string title, string rundownKey, ExpeditionInTierData data, eRundownTier tier, int expIndex, int expListID, eExpeditionIconStatus iconStatus)
[HarmonyPatch(typeof(CM_ExpeditionWindow), nameof(CM_ExpeditionWindow.SetExpeditionInfo))]
public class CM_ExpeditionWindow_SetExpeditionInfo_Patch
{
    public static void Prefix(CM_ExpeditionWindow __instance)
    {

    }
}

[HarmonyPatch(typeof(CM_ExpeditionWindow), nameof(CM_ExpeditionWindow.SetVisible))]
public class CM_ExpeditionWindow_SetVisible_Patch
{
    public static void Postfix(CM_ExpeditionWindow __instance, bool visible, bool inMenuBar)
    {
        if (inMenuBar)
            return;

        SectorIconImageLoader.ExpeditionDetailsWindowActive = visible;
        SectorIconImageLoader.ExpeditionTier = __instance.m_tier;
        SectorIconImageLoader.ExpeditionIndex = __instance.m_expIndex;

        var expInTier = __instance.m_data;
        var index = __instance.m_expIndex;
        var tier = __instance.m_tier;
    }
}
