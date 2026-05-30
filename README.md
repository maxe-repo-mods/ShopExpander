# ShopExpander

Customize shop item variety by configuring max items per category and price multipliers.

R.E.P.O. BepInEx mod. Host-side only.

## Features

- Configure maximum number of each item type in the shop (guns, melee, grenades, mines, drones, health packs, upgrades, trackers)
- Apply global and per-upgrade price multipliers
- Remove per-item spawn limits to allow more variety to appear
- Optionally remove purchase limits (allow buying multiple of normally single-purchase items)
- Block specific items by name from appearing in the shop

## Installation

Requires [BepInEx 5.x](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/).

Place `ShopExpander.dll` in `BepInEx/plugins/`.

Configuration file is generated at `BepInEx/config/maxenterme.ShopExpander.cfg` on first launch.

## Configuration

Edit `BepInEx/config/maxenterme.ShopExpander.cfg` to customize item counts and prices.

### Item Counts

| Section | Key | Default | Range | Description |
|---------|-----|---------|-------|-------------|
| ItemCounts | MaxGuns | 5 | 0-20 | Maximum guns in shop |
| ItemCounts | MaxMelee | 3 | 0-20 | Maximum melee weapons in shop |
| ItemCounts | MaxGrenades | 5 | 0-20 | Maximum grenades in shop |
| ItemCounts | MaxMines | 3 | 0-20 | Maximum mines in shop |
| ItemCounts | MaxDrones | 3 | 0-20 | Maximum drones in shop |
| ItemCounts | MaxHealthPacks | 5 | 0-20 | Maximum health packs in shop |
| ItemCounts | MaxUpgrades | 10 | 0-30 | Maximum upgrades in shop |
| ItemCounts | MaxTrackers | 3 | 0-20 | Maximum trackers in shop |

### Pricing

| Section | Key | Default | Range | Description |
|---------|-----|---------|-------|-------------|
| Price | PriceMultiplier | 100 | 0-500 | Item price multiplier (100 = 100%, 50 = 50% off, 200 = double price) |
| Price | UpgradePriceMultiplier | 100 | 0-500 | Upgrade price multiplier (100 = 100%, 50 = 50% off, 200 = double price) |

### Spawn and Purchase Limits

| Section | Key | Default | Description |
|---------|-----|---------|-------------|
| General | RemoveSpawnLimits | true | Remove per-item spawn limits (maxAmountInShop) to allow more variety |
| General | RemovePurchaseLimits | false | Remove per-item purchase limits (allow buying multiples of single-purchase items) |
| General | BlockedItems | (empty) | Comma-separated list of item names to exclude from shop (e.g., `Item Gun Tranq, Item Mine Shockwave`) |

## Build

```bash
dotnet build -c Release
```

The compiled DLL will be available at:
```
bin/Release/netstandard2.1/ShopExpander.dll
```


## AI Disclosure

This mod was developed with the assistance of AI (Claude by Anthropic). All code has been reviewed and tested by the developer.

## License

MIT
