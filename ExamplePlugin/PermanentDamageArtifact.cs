using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using static RoR2.RoR2Content;
using JetBrains.Annotations;
using System;

namespace ExamplePlugin
{
    internal class PermanentDamageArtifact : ArtifactDef
    {
        public PermanentMaxHealthDecreaseItem PermanentMaxHealthDecreaseItem = new();
        public PermanentDamageArtifact() 
        {
            cachedName = "RiskyArtifactOfArrogance";
            nameToken = "RISKYARTIFACTS_ARROGANCE_NAME";
            descriptionToken = "RISKYARTIFACTS_ARROGANCE_DESC";
            smallIconDeselectedSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            smallIconSelectedSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();

            ContentAddition.AddArtifactDef(this);

            On.RoR2.HealthComponent.TakeDamage += (On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo) =>
            {
                GlobalEventManager_preOnHitEnemy(self, damageInfo);
                orig(self, damageInfo);
                GlobalEventManager_postOnHitEnemy(self, damageInfo);
            };

            CharacterBody.onBodyStartGlobal += (characterBody) =>
            {
                if (characterBody.isPlayerControlled && RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(this))
                {
                    ItemDef aegius = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/BarrierOnOverHeal/BarrierOnOverHeal.asset").WaitForCompletion();
                    characterBody.inventory.GiveItem(aegius);
                }
                characterBody.SetBuffCount(PermanentMaxHealthDecreaseItem.HealthDisplayBuff.buffIndex, characterBody.inventory.GetItemCount(PermanentMaxHealthDecreaseItem));
            };
        }
        private void GlobalEventManager_preOnHitEnemy(RoR2.HealthComponent healthComponent, DamageInfo damageInfo)
        {
            var victim = healthComponent?.body;

            if (victim &&
                victim.isPlayerControlled &&
                RunArtifactManager.instance &&
                RunArtifactManager.instance.IsArtifactEnabled(this))
            {
                healthComponent.barrier += 1;
            }
        }

        private void GlobalEventManager_postOnHitEnemy(RoR2.HealthComponent healthComponent, DamageInfo damageInfo)
        {
            bool artifactEnabled = RunArtifactManager.instance?.IsArtifactEnabled(this) ?? false;
            if (!artifactEnabled) return;

            var victim = healthComponent?.body;

            if (!(victim?.isPlayerControlled ?? false)) return;

            var tookDamage = healthComponent.barrier <= 0;

            // damage info is rejected when it is blocked by tougher times or similar effects
            if (damageInfo.rejected) healthComponent.barrier = Math.Max(0, healthComponent.barrier - 1);

            healthComponent.health = victim.maxHealth;
            if (tookDamage && !damageInfo.rejected)
            {
                bool isFallDamage = (damageInfo.damageType.damageType & DamageType.FallDamage) == DamageType.FallDamage;

                var permanentDamageTaken = Mathf.CeilToInt(isFallDamage ? damageInfo.damage : 5 * damageInfo.procCoefficient);

                victim.inventory.GiveItem(PermanentMaxHealthDecreaseItem, permanentDamageTaken);
                victim.SetBuffCount(PermanentMaxHealthDecreaseItem.HealthDisplayBuff.buffIndex, victim.inventory.GetItemCount(PermanentMaxHealthDecreaseItem));
                if (victim.healthComponent.health <= victim.inventory.GetItemCount(PermanentMaxHealthDecreaseItem))
                {
                    victim.healthComponent.health = 0;
                }
            }

        }
    }
}
