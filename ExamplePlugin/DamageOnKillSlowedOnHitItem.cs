using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace ExamplePlugin
{
    internal class DamageOnKillSlowedOnHitItem:ItemDef
    {
        public readonly DamageBuff DamageBuff = new();
        public readonly SpeedNerf SpeedNerf = new();
        public DamageOnKillSlowedOnHitItem() : base()
        {
            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            name = "DAMAGE_ON_KILL_SLOWED_ON_HIT_NAME";
            nameToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_NAME";
            pickupToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_PICKUP";
            descriptionToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_DESC";
            loreToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_LORE";


            #pragma warning disable CS0618
            deprecatedTier = ItemTier.Lunar;
            #pragma warning restore CS0618

            pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            canRemove = true;

            hidden = false;

            // https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/
            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(this, displayRules));
        }
    }
}
