using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using GlobalEventManager = On.RoR2.GlobalEventManager;

using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{
    // This attribute specifies that we have a dependency on a given BepInEx Plugin,
    // We need the R2API ItemAPI dependency because we are using for adding our item to the game.
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(ItemAPI.PluginGUID)]

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
        public const string PluginAuthor = "SeanH";
        public const string PluginName = "ExamplePlugin";
        public const string PluginVersion = "1.0.0";

        private static ItemDef damageOnKillSlowedOnHit;

        private static DamageBuff damageBuff;
        private static SpeedNerf speedNerf;

        public void Awake()
        {


            Log.Init(Logger);

            damageBuff = new DamageBuff();

            speedNerf = new SpeedNerf();

            ContentAddition.AddBuffDef(damageBuff);

            ContentAddition.AddBuffDef(speedNerf);

            damageOnKillSlowedOnHit = ScriptableObject.CreateInstance<ItemDef>();

            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            damageOnKillSlowedOnHit.name = "DAMAGE_ON_KILL_SLOWED_ON_HIT_NAME";
            damageOnKillSlowedOnHit.nameToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_NAME";
            damageOnKillSlowedOnHit.pickupToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_PICKUP";
            damageOnKillSlowedOnHit.descriptionToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_DESC";
            damageOnKillSlowedOnHit.loreToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_LORE";


            #pragma warning disable Publicizer001
            #pragma warning disable CS0618
            damageOnKillSlowedOnHit.deprecatedTier = ItemTier.Lunar;
            #pragma warning restore CS0618
            #pragma warning restore Publicizer001

            damageOnKillSlowedOnHit.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            damageOnKillSlowedOnHit.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            damageOnKillSlowedOnHit.canRemove = true;

            damageOnKillSlowedOnHit.hidden = false;

            // https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/
            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(damageOnKillSlowedOnHit, displayRules));

            GlobalEventManager.OnCharacterDeath += (GlobalEventManager.orig_OnCharacterDeath orig, RoR2.GlobalEventManager self, DamageReport report) =>
            {
                GlobalEventManager_onCharacterDeath(report);
                orig(self, report);
            };
            GlobalEventManager.OnHitEnemy += (GlobalEventManager.orig_OnHitEnemy orig, RoR2.GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject) =>
            {
                GlobalEventManager_onHitAll(damageInfo, hitObject);
                orig(self, damageInfo, hitObject);
            };
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;
        }

        private void GlobalEventManager_onCharacterDeath(DamageReport report)
        {
            // If a character was killed by the world, we shouldn't do anything.
            if (!report.attacker || !report.attackerBody)
            {
                return;
            }

            var attackerCharacterBody = report.attackerBody;

            if (attackerCharacterBody.inventory)
            {
                var garbCount = attackerCharacterBody.inventory.GetItemCount(damageOnKillSlowedOnHit);
                if (garbCount > 0)
                {
                    for(int i = 0; i < garbCount; i++)
                    {
                        attackerCharacterBody.AddBuff(damageBuff);
                    }
                }
            }
        }

        private void GlobalEventManager_onHitAll(DamageInfo damageInfo, GameObject hitObject)
        {
            // If a character was killed by the world, we shouldn't do anything.

            var victim = hitObject?.GetComponent<CharacterBody>();

            if (victim?.inventory)
            {
                var garbCount = victim.inventory.GetItemCount(damageOnKillSlowedOnHit);
                if (garbCount > 0)
                {
                    for (int i = 0; i < garbCount; i++)
                    {
                        victim.AddBuff(speedNerf);
                    }
                }
            }

            var attacker = damageInfo?.attacker?.GetComponent<CharacterBody>();
            if (attacker)
            {
                var buffs = attacker.GetBuffCount(damageBuff);
                var debuffs = attacker.GetBuffCount(speedNerf);
                bool roll = Util.CheckRoll(2 * damageInfo.procCoefficient, attacker.master);
                Log.Info($"proc co {damageInfo.procCoefficient}, roll {roll}, debuffs {debuffs}, buffs {buffs}");
                if (debuffs > 0 && roll)
                {
                    attacker.RemoveBuff(speedNerf);
                    if(buffs > 0)
                    {
                        attacker.RemoveBuff(damageBuff);
                    }
                }
            }
        }

        private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffs = sender.GetBuffCount(damageBuff);
            if(buffs > 0)
            {
                var beforeDmg = sender.baseDamage + args.baseDamageAdd + (sender.level - 1) * sender.levelDamage;
                var addedDmg = (beforeDmg) * Mathf.Pow(1.05f, (buffs)) - (beforeDmg);
                args.baseDamageAdd += addedDmg;
            }
            int debuffs = sender.GetBuffCount(speedNerf);
            if(debuffs > 0)
            {
                var beforeSpeed = sender.baseMoveSpeed + args.baseMoveSpeedAdd + (sender.level - 1) * sender.levelMoveSpeed;
                var addedSpeed = (beforeSpeed) * Mathf.Pow(.9f, debuffs) - (beforeSpeed);
                args.baseMoveSpeedAdd += addedSpeed;
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
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(speedNerf);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(damageBuff);
            }
        }
    }
}
