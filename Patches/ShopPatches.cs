using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ShopExpander.Patches;

[HarmonyPatch]
public static class ShopPatches
{
    [HarmonyPatch(typeof(ShopManager), "ShopInitialize")]
    [HarmonyPrefix]
    private static void ShopInitialize_Prefix()
    {
        try
        {
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

            AdjustItemLimits();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"ShopExpander ShopInitialize Prefix error: {e}");
        }
    }

    /// <summary>
    /// Postfix on GetAllItemsFromStatsManager (private method):
    /// - Fix itemConsumablesAmount (game overwrites it with Random.Range(4,6))
    /// - Fix itemSpawnTargetAmount for general items
    /// - Remove blocked items from potentialItems list as a safety net
    /// </summary>
    [HarmonyPatch(typeof(ShopManager), "GetAllItemsFromStatsManager")]
    [HarmonyPostfix]
    private static void GetAllItemsFromStatsManager_Postfix(ShopManager __instance)
    {
        try
        {
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

            // Game variable mapping (from decompiled source):
            //   potentialItems        = general items (guns, melee, grenades, mines, drones, trackers)
            //   potentialItemConsumables = power crystals ONLY
            //   itemSpawnTargetAmount = max count for potentialItems
            //   itemConsumablesAmount = max count for power crystals (game sets Random.Range(4,6))
            //   itemUpgradesAmount    = max count for upgrades
            //   itemHealthPacksAmount = max count for health packs

            int generalItems = Plugin.MaxGuns.Value + Plugin.MaxMelee.Value +
                               Plugin.MaxGrenades.Value + Plugin.MaxMines.Value +
                               Plugin.MaxDrones.Value + Plugin.MaxTrackers.Value;

            __instance.itemSpawnTargetAmount = generalItems;
            __instance.itemUpgradesAmount = Plugin.MaxUpgrades.Value;
            __instance.itemHealthPacksAmount = Plugin.MaxHealthPacks.Value;
            // itemConsumablesAmount controls power crystals — leave at game default

            // Safety net: remove items of blocked categories or block list from potentialItems
            var blocked = GetBlockedItemNames();
            int removed = __instance.potentialItems.RemoveAll(item =>
            {
                if (blocked.Contains(((UnityEngine.Object)item).name)) return true;
                int? limit = GetCategoryLimit(item.itemType);
                return limit.HasValue && limit.Value <= 0;
            });

            Plugin.Logger.LogInfo($"ShopExpander GetAllItems(post): " +
                $"generalTarget={generalItems}, upgrades={Plugin.MaxUpgrades.Value}, " +
                $"healthPacks={Plugin.MaxHealthPacks.Value}, " +
                $"potentialItems={__instance.potentialItems.Count}, removed={removed}");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"ShopExpander GetAllItems Postfix error: {e}");
        }
    }

    private static HashSet<string>? _cachedBlockedItems;
    private static string? _cachedBlockedRaw;

    private static HashSet<string> GetBlockedItemNames()
    {
        var raw = Plugin.BlockedItems.Value;
        if (_cachedBlockedItems != null && raw == _cachedBlockedRaw)
            return _cachedBlockedItems;

        _cachedBlockedRaw = raw;
        _cachedBlockedItems = string.IsNullOrWhiteSpace(raw)
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(
                raw.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0),
                StringComparer.OrdinalIgnoreCase);
        return _cachedBlockedItems;
    }

    private static void AdjustItemLimits()
    {
        if (StatsManager.instance?.itemDictionary == null) return;

        var blocked = GetBlockedItemNames();

        foreach (var kvp in StatsManager.instance.itemDictionary)
        {
            var item = kvp.Value;
            if (item == null) continue;

            // Per-item block list
            if (blocked.Contains(kvp.Key))
            {
                item.maxAmountInShop = 0;
                Plugin.Logger.LogInfo($"ShopExpander: blocked '{kvp.Key}'");
                continue;
            }

            if (Plugin.RemovePurchaseLimits.Value)
                item.maxPurchase = false;

            int? limit = GetCategoryLimit(item.itemType);
            if (limit.HasValue)
            {
                if (limit.Value <= 0)
                {
                    item.maxAmountInShop = 0;
                }
                else if (Plugin.RemoveSpawnLimits.Value)
                {
                    item.maxAmountInShop = 999;
                    item.maxAmount = 999;
                }
                else
                {
                    item.maxAmountInShop = Mathf.Max(item.maxAmountInShop, limit.Value);
                    item.maxAmount = Mathf.Max(item.maxAmount, limit.Value);
                }
            }
            else if (Plugin.RemoveSpawnLimits.Value)
            {
                item.maxAmountInShop = 999;
                item.maxAmount = 999;
            }
        }

        if (blocked.Count > 0)
            Plugin.Logger.LogInfo($"ShopExpander: item limits adjusted. Blocked: {string.Join(", ", blocked)}");
        else
            Plugin.Logger.LogInfo("ShopExpander: item limits adjusted.");
    }

    private static int? GetCategoryLimit(SemiFunc.itemType type)
    {
        return type switch
        {
            SemiFunc.itemType.gun => Plugin.MaxGuns.Value,
            SemiFunc.itemType.melee => Plugin.MaxMelee.Value,
            SemiFunc.itemType.grenade => Plugin.MaxGrenades.Value,
            SemiFunc.itemType.mine => Plugin.MaxMines.Value,
            SemiFunc.itemType.drone => Plugin.MaxDrones.Value,
            SemiFunc.itemType.healthPack => Plugin.MaxHealthPacks.Value,
            SemiFunc.itemType.item_upgrade => Plugin.MaxUpgrades.Value,
            SemiFunc.itemType.tracker => Plugin.MaxTrackers.Value,
            _ => null
        };
    }

    [HarmonyPatch(typeof(ItemAttributes), "GetValue")]
    [HarmonyPostfix]
    private static void GetValue_Postfix(ItemAttributes __instance)
    {
        try
        {
            if (Plugin.PriceMultiplier.Value == 100 && Plugin.UpgradePriceMultiplier.Value == 100) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;

            float multiplier = (__instance.item != null && __instance.item.itemType == SemiFunc.itemType.item_upgrade)
                ? Plugin.UpgradePriceMultiplier.Value / 100f
                : Plugin.PriceMultiplier.Value / 100f;

            __instance.value = Mathf.Max(1, Mathf.RoundToInt(__instance.value * multiplier));
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"ShopExpander GetValue error: {e}");
        }
    }
}
