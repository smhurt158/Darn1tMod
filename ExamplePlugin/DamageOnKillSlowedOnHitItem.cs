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

            On.RoR2.GlobalEventManager.OnCharacterDeath += (On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, RoR2.GlobalEventManager self, DamageReport report) =>
            {
                GlobalEventManager_onCharacterDeath(report);
                orig(self, report);
            };

            On.RoR2.HealthComponent.TakeDamage += (On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo) =>
            {
                GlobalEventManager_preOnHitEnemy(self, damageInfo);
                orig(self, damageInfo);
            };
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;

        }

        private void GlobalEventManager_onCharacterDeath(DamageReport report)
        {
            var attackerCharacterBody = report?.attackerBody;

            if (attackerCharacterBody?.inventory)
            {
                var garbCount = attackerCharacterBody.inventory.GetItemCount(this);
                if (garbCount > 0)
                {
                    for (int i = 0; i < garbCount; i++)
                    {
                        attackerCharacterBody.AddBuff(DamageBuff);
                    }
                }
            }
        }
        private void GlobalEventManager_preOnHitEnemy(RoR2.HealthComponent healthComponent, DamageInfo damageInfo)
        {
            var victim = healthComponent?.body;

            if (victim?.inventory)
            {
                var garbCount = victim.inventory.GetItemCount(this);
                if (garbCount > 0)
                {
                    for (int i = 0; i < garbCount; i++)
                    {
                        victim.AddBuff(SpeedNerf);
                    }
                }
            }
        }

        private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffs = sender.GetBuffCount(DamageBuff);
            if (buffs > 0)
            {
                var beforeDmg = sender.baseDamage + args.baseDamageAdd + (sender.level - 1) * sender.levelDamage;
                var addedDmg = (beforeDmg) * Mathf.Pow(1.05f, (buffs)) - (beforeDmg);
                args.baseDamageAdd += addedDmg;
            }
            int debuffs = sender.GetBuffCount(SpeedNerf);
            if (debuffs > 0)
            {
                var beforeSpeed = sender.baseMoveSpeed + args.baseMoveSpeedAdd + (sender.level - 1) * sender.levelMoveSpeed;
                var addedSpeed = (beforeSpeed) * Mathf.Pow(.9f, debuffs) - (beforeSpeed);
                args.baseMoveSpeedAdd += addedSpeed;
            }
        }
    }
}
