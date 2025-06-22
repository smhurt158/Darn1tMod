using R2API;
using RoR2;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Darn1tMod
{
    internal class DamageOnKillSlowedOnHitItem:ItemDef
    {
        const int MAX_BUFFS_PER_STACK = 30;
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

            try
            {
                pickupModelPrefab = Asset.mainBundle.LoadAsset<GameObject>("cube.prefab");
            }
            catch (Exception)
            {
                pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
            }

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
                var itemCount = attackerCharacterBody.inventory.GetItemCount(this);
                if (itemCount > 0)
                {
                    int buffs = attackerCharacterBody.GetBuffCount(DamageBuff);

                    attackerCharacterBody.SetBuffCount(DamageBuff.buffIndex, Math.Min(buffs + itemCount, itemCount * MAX_BUFFS_PER_STACK));
                }
            }
        }
        private void GlobalEventManager_preOnHitEnemy(RoR2.HealthComponent healthComponent, DamageInfo damageInfo)
        {
            var victim = healthComponent?.body;

            if (victim?.inventory)
            {
                var itemCount = victim.inventory.GetItemCount(this);
                if (itemCount > 0)
                {
                    int debuffs = victim.GetBuffCount(SpeedNerf);

                    victim.SetBuffCount(SpeedNerf.buffIndex, Math.Min(debuffs + itemCount, itemCount * MAX_BUFFS_PER_STACK));
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
