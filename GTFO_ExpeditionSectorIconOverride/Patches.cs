using System;
using System.Linq;
using CellMenu;
using ExSeIcOv.Components;
using ExSeIcOv.Core;
using ExSeIcOv.Core.Loaders;
using ExSeIcOv.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ExSeIcOv;

[HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Awake))]
public static class StartMainGame__Awake__Patch
{
    public static void Postfix()
    {
        Plugin.Init();
    }
}

[HarmonyPatch(typeof(GlobalPopupMessageManager), nameof(GlobalPopupMessageManager.Setup))]
public class GlobalPopupMessageManager__Setup__Patch
{
    public static void Postfix()
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
            var type = IntelImageType.None;
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

            if (type == IntelImageType.None)
                continue;
            
            var setter = contentChild.gameObject.GetOrAddComponent<IntelImageSetter>();
            setter.SetType(type);
        }
    }
}

[HarmonyPatch(typeof(GameStateManager), nameof(GameStateManager.ChangeState))]
public class GameStateManager__ChangeState__Patch
{
    public static void Postfix(eGameStateName nextState)
    {
        switch (nextState)
        {
            case eGameStateName.Generating:
                SectorIconImageLoader.IsInExpedition = true;
                // Local_1_2_0
                // Local_1 <- Rundown, 1 <- Tier, 0 <- Index
                var data = RundownManager.ActiveExpeditionUniqueKey.Split("_").Skip(2).ToArray();
                
                if (Enum.TryParse(data[0], out eRundownTier tier))
                {
                    SectorIconImageLoader.ExpeditionTier = tier;
                }

                if (int.TryParse(data[1], out var expeditionIndex))
                {
                    SectorIconImageLoader.ExpeditionIndex = expeditionIndex;
                }
                break;
            case eGameStateName.Lobby:
                SectorIconImageLoader.IsInExpedition = false;
                break;
            default:
                break;
        }

    }
}

[HarmonyPatch(typeof(CM_ExpeditionSectorIcon), nameof(CM_ExpeditionSectorIcon.Setup))]
public class CM_ExpeditionSectorIcon__Setup__Patch
{
    // Called for Main, Extreme and Overload
    public static void Postfix(CM_ExpeditionSectorIcon __instance, LevelGeneration.LG_LayerType type)
    {
        var sectorIconType = (SectorIconType)((int)type + 1);

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
public class CM_ExpeditionSectorIcon__SetupAsFinishedAll__Patch
{
    // Called PE exclusively
    public static void Postfix(CM_ExpeditionSectorIcon __instance)
    {
        var iconType = SectorIconType.PrisonerEfficiency;
        var skull = __instance.m_iconFinishedAllSkull;
        var bg = __instance.m_iconFinishedAllBG;

        CM_ExpeditionSectorIcon__Setup__Patch.AddSectorSetterComponent(__instance, iconType, skull, bg);
    }
}

//public void SetExpeditionInfo(string title, string rundownKey, ExpeditionInTierData data, eRundownTier tier, int expIndex, int expListID, eExpeditionIconStatus iconStatus)
// [HarmonyPatch(typeof(CM_ExpeditionWindow), nameof(CM_ExpeditionWindow.SetExpeditionInfo))]
// public class CM_ExpeditionWindow_SetExpeditionInfo_Patch
// {
//     public static void Prefix(CM_ExpeditionWindow __instance)
//     {
//
//     }
// }

[HarmonyPatch(typeof(CM_ExpeditionWindow), nameof(CM_ExpeditionWindow.SetVisible))]
public class CM_ExpeditionWindow__SetVisible__Patch
{
    public static void Prefix(CM_ExpeditionWindow __instance, bool visible, bool inMenuBar)
    {
        if (inMenuBar)
            return;

        SectorIconImageLoader.IsExpeditionDetailsWindowActive = visible;
        SectorIconImageLoader.ExpeditionTier = __instance.m_tier;
        SectorIconImageLoader.ExpeditionIndex = __instance.m_expIndex;
    }
}
