using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using GlobalEventManager = On.RoR2.GlobalEventManager;
using HealthComponent = On.RoR2.HealthComponent;

namespace ExamplePlugin
{
    // This attribute specifies that we have a dependency on a given BepInEx Plugin,
    // We need the R2API ItemAPI dependency because we are using for adding our item to the game.
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]

    // This one is because we use a .language file for language tokens
    // More info in https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
    [BepInDependency(LanguageAPI.PluginGUID)]

    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    // BaseUnityPlugin itself inherits from MonoBehaviour,
    // so you can use this as a reference for what you can declare and use in your plugin class
    // More information in the Unity Docs: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class ExamplePlugin : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Darn1t";
        public const string PluginName = "ExamplePlugin";
        public const string PluginVersion = "1.0.0";

        private static readonly DamageOnKillSlowedOnHitItem damageOnKillSlowedOnHit = new();
        private static readonly PermanentDamageArtifact permanentDamageArtifact = new();

        public void Awake()
        {
            Log.Init(Logger);

            GlobalEventManager.OnCharacterDeath += (GlobalEventManager.orig_OnCharacterDeath orig, RoR2.GlobalEventManager self, DamageReport report) =>
            {
                GlobalEventManager_onCharacterDeath(report);
                orig(self, report);
            };
            
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
            HealthComponent.TakeDamage += (HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo) =>
            {
                GlobalEventManager_onHitEnemy(self , damageInfo);
                orig(self, damageInfo);

            };
        }

        private void GlobalEventManager_onCharacterDeath(DamageReport report)
        {
            var attackerCharacterBody = report?.attackerBody;

            if (attackerCharacterBody?.inventory)
            {
                var garbCount = attackerCharacterBody.inventory.GetItemCount(damageOnKillSlowedOnHit);
                if (garbCount > 0)
                {
                    for(int i = 0; i < garbCount; i++)
                    {
                        attackerCharacterBody.AddBuff(damageOnKillSlowedOnHit.DamageBuff);
                    }
                }
            }
        }

        private void GlobalEventManager_onHitEnemy(RoR2.HealthComponent healthComponent, DamageInfo damageInfo)
        {
            var victim = healthComponent?.body;

            if (victim?.inventory)
            {
                var garbCount = victim.inventory.GetItemCount(damageOnKillSlowedOnHit);
                if (garbCount > 0)
                {
                    for (int i = 0; i < garbCount; i++)
                    {
                        victim.AddBuff(damageOnKillSlowedOnHit.SpeedNerf);
                    }
                }
            }

            if(victim && victim.isPlayerControlled && RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(permanentDamageArtifact))
            {
                victim.inventory.GiveItem(permanentDamageArtifact.PermanentMaxHealthDecreaseItem);
                if(victim.baseMaxHealth <= 1)
                {
                    victim.healthComponent.health = 0;
                }
                damageInfo.damage = 0;
            }
        }

        private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffs = sender.GetBuffCount(damageOnKillSlowedOnHit.DamageBuff);
            if(buffs > 0)
            {
                var beforeDmg = sender.baseDamage + args.baseDamageAdd + (sender.level - 1) * sender.levelDamage;
                var addedDmg = (beforeDmg) * Mathf.Pow(1.05f, (buffs)) - (beforeDmg);
                args.baseDamageAdd += addedDmg;
            }
            int debuffs = sender.GetBuffCount(damageOnKillSlowedOnHit.SpeedNerf);
            if(debuffs > 0)
            {
                var beforeSpeed = sender.baseMoveSpeed + args.baseMoveSpeedAdd + (sender.level - 1) * sender.levelMoveSpeed;
                var addedSpeed = (beforeSpeed) * Mathf.Pow(.9f, debuffs) - (beforeSpeed);
                args.baseMoveSpeedAdd += addedSpeed;
            }

            int permDebuffs = sender.inventory.GetItemCount(permanentDamageArtifact.PermanentMaxHealthDecreaseItem);

            if (permDebuffs > 0)
            {
                args.baseHealthAdd -= Mathf.Min(5 * permDebuffs, sender.baseMaxHealth - 1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(damageOnKillSlowedOnHit.itemIndex), transform.position, transform.forward * 20f);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(damageOnKillSlowedOnHit.SpeedNerf);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(damageOnKillSlowedOnHit.DamageBuff);
            }
        }
    }
}
