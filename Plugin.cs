using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ShopExpander;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string PluginGuid = "maxenterme.ShopExpander";
    private const string PluginName = "ShopExpander";
    private const string PluginVersion = "1.1.2";

    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;

    // Item count per category
    internal static ConfigEntry<int> MaxGuns = null!;
    internal static ConfigEntry<int> MaxMelee = null!;
    internal static ConfigEntry<int> MaxGrenades = null!;
    internal static ConfigEntry<int> MaxMines = null!;
    internal static ConfigEntry<int> MaxDrones = null!;
    internal static ConfigEntry<int> MaxHealthPacks = null!;
    internal static ConfigEntry<int> MaxUpgrades = null!;
    internal static ConfigEntry<int> MaxTrackers = null!;

    // Price
    internal static ConfigEntry<int> PriceMultiplier = null!;
    internal static ConfigEntry<int> UpgradePriceMultiplier = null!;

    // Spawn limits
    internal static ConfigEntry<bool> RemoveSpawnLimits = null!;
    internal static ConfigEntry<bool> RemovePurchaseLimits = null!;

    // Block list
    internal static ConfigEntry<string> BlockedItems = null!;

    private void Awake()
    {
        Instance = this;

        MaxGuns = Config.Bind("ItemCounts", "MaxGuns", 5,
            new ConfigDescription("Max guns in shop", new AcceptableValueRange<int>(0, 20)));
        MaxMelee = Config.Bind("ItemCounts", "MaxMelee", 3,
            new ConfigDescription("Max melee weapons in shop", new AcceptableValueRange<int>(0, 20)));
        MaxGrenades = Config.Bind("ItemCounts", "MaxGrenades", 5,
            new ConfigDescription("Max grenades in shop", new AcceptableValueRange<int>(0, 20)));
        MaxMines = Config.Bind("ItemCounts", "MaxMines", 3,
            new ConfigDescription("Max mines in shop", new AcceptableValueRange<int>(0, 20)));
        MaxDrones = Config.Bind("ItemCounts", "MaxDrones", 3,
            new ConfigDescription("Max drones in shop", new AcceptableValueRange<int>(0, 20)));
        MaxHealthPacks = Config.Bind("ItemCounts", "MaxHealthPacks", 5,
            new ConfigDescription("Max health packs in shop", new AcceptableValueRange<int>(0, 20)));
        MaxUpgrades = Config.Bind("ItemCounts", "MaxUpgrades", 10,
            new ConfigDescription("Max upgrades in shop", new AcceptableValueRange<int>(0, 30)));
        MaxTrackers = Config.Bind("ItemCounts", "MaxTrackers", 3,
            new ConfigDescription("Max trackers in shop", new AcceptableValueRange<int>(0, 20)));

        PriceMultiplier = Config.Bind("Price", "PriceMultiplier", 100,
            new ConfigDescription("Item price multiplier (100 = 100%, 50 = 50%, 200 = 200%)", new AcceptableValueRange<int>(0, 500)));
        UpgradePriceMultiplier = Config.Bind("Price", "UpgradePriceMultiplier", 100,
            new ConfigDescription("Upgrade price multiplier (100 = 100%, 50 = 50%, 200 = 200%)", new AcceptableValueRange<int>(0, 500)));

        RemoveSpawnLimits = Config.Bind("General", "RemoveSpawnLimits", true,
            "Remove per-item spawn limits (maxAmountInShop) so more variety appears.");
        RemovePurchaseLimits = Config.Bind("General", "RemovePurchaseLimits", false,
            "Remove per-item purchase limits. Allows buying items that are normally limited to 1.");

        BlockedItems = Config.Bind("General", "BlockedItems", "",
            "Comma-separated list of item names to exclude from the shop. " +
            "Example: Item Gun Tranq, Item Mine Shockwave");

        new Harmony(PluginGuid).PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded!");
    }
}
