using RebuildSharedData.ClientTypes;
using RebuildSharedData.Enum;
using RoBotClient.Bot.State;

namespace RoBotClient.Bot.Behavior;

/// <summary>
/// Decides which inventory items a bot may auto-sell. Conservative by design: keeps anything equipped,
/// usable (potions/consumables), a card, ammo, refined/carded/slotted (i.e. potentially valuable) gear,
/// zero sell-value, on the keep-list, or a configured healing item. Sells junk (Etc loot) and plain
/// common weapons/equipment (unrefined, uncarded, unslotted, normal rank).
/// </summary>
public static class ShopPolicy
{
    public static bool ShouldSell(ItemData? def, InventoryItemView inv, bool equipped, BotBehaviorConfig cfg)
    {
        if (def == null || equipped) return false;
        if (def.SellPrice <= 0) return false;
        if (cfg.KeepItemIds.Contains(def.Id) || cfg.HealingItemIds.Contains(def.Id)) return false;

        switch (def.ItemClass)
        {
            case ItemClass.Etc:
                return true; // junk loot (Jellopy, Clover, ...) — always sell
            case ItemClass.Weapon:
            case ItemClass.Equipment:
                var carded = inv.Cards != null && Array.Exists(inv.Cards, c => c > 0);
                return inv.Refine == 0 && !carded && def.Slots == 0 && def.ItemRank <= 0;
            default:
                return false; // Useable, Card, Ammo, None — keep
        }
    }
}
