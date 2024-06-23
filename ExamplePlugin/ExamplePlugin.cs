using BepInEx;
using R2API;
using RoR2;
using UnityEngine;
using GlobalEventManager = On.RoR2.GlobalEventManager;

using UnityEngine.AddressableAssets;

namespace ExamplePlugin
{

    // This is an example plugin that can be put in
    // BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    // It's a small plugin that adds a relatively simple item to the game,
    // and gives you that item whenever you press F2.

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
        private float test = 0f;
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "AuthorName";
        public const string PluginName = "ExamplePlugin";
        public const string PluginVersion = "1.0.0";

        // We need our item definition to persist through our functions, and therefore make it a class field.
        private static ItemDef myItemDef;
        private static BuffDef myBuffDef;
        private static BuffDef myDebuffDef;

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {


            Log.Init(Logger);

            myBuffDef = new BuffDef();

            myBuffDef.buffColor = Color.white;
            myBuffDef.canStack = true;
            myBuffDef.eliteDef = null;
            myBuffDef.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            myBuffDef.isCooldown = false;
            myBuffDef.isDebuff = false;
            myBuffDef.isHidden = false;
            myBuffDef.startSfx = null;
            myBuffDef.name = "test";

            ContentAddition.AddBuffDef(myBuffDef);

            myDebuffDef = new BuffDef();

            myDebuffDef.buffColor = Color.white;
            myDebuffDef.canStack = true;
            myDebuffDef.eliteDef = null;
            myDebuffDef.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            myDebuffDef.isCooldown = false;
            myDebuffDef.isDebuff = true;
            myDebuffDef.isHidden = false;
            myDebuffDef.startSfx = null;
            myDebuffDef.name = "test2";

            ContentAddition.AddBuffDef(myDebuffDef);

            myItemDef = ScriptableObject.CreateInstance<ItemDef>();

            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            myItemDef.name = "EXAMPLE_CLOAKONKILL_NAME";
            myItemDef.nameToken = "EXAMPLE_CLOAKONKILL_NAME";
            myItemDef.pickupToken = "EXAMPLE_CLOAKONKILL_PICKUP";
            myItemDef.descriptionToken = "EXAMPLE_CLOAKONKILL_DESC";
            myItemDef.loreToken = "EXAMPLE_CLOAKONKILL_LORE";


            // Tier1=white, Tier2=green, Tier3=red, Lunar=Lunar, Boss=yellow,
            // and finally NoTier is generally used for helper items, like the tonic affliction
            #pragma warning disable Publicizer001
            myItemDef.deprecatedTier = ItemTier.Lunar;
            #pragma warning restore Publicizer001
            

            myItemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            myItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            // Can remove determines if a shrine of order, or a printer can take this item, generally true, except for NoTier items.
            myItemDef.canRemove = true;

            // Hidden means that there will be no pickup notification, and it won't appear in the inventory at the top of the screen.
            myItemDef.hidden = false;

            // https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/
            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(myItemDef, displayRules));

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
                var garbCount = attackerCharacterBody.inventory.GetItemCount(myItemDef.itemIndex);
                if (garbCount > 0)
                {
                    for(int i = 0; i < garbCount; i++)
                    {
                        attackerCharacterBody.AddBuff(myBuffDef);
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
                var garbCount = victim.inventory.GetItemCount(myItemDef);
                if (garbCount > 0)
                {
                    for (int i = 0; i < garbCount; i++)
                    {
                        victim.AddBuff(myDebuffDef);
                    }
                }
            }

            var attacker = damageInfo?.attacker?.GetComponent<CharacterBody>();
            Log.Info($"attacker {attacker}");
            if (attacker)
            {
                var buffs = attacker.GetBuffCount(myBuffDef);
                var debuffs = attacker.GetBuffCount(myDebuffDef);
                bool roll = Util.CheckRoll(2 * damageInfo.procCoefficient + test, attacker.master);
                Log.Info($"proc co {damageInfo.procCoefficient}, roll {roll}, debuffs {debuffs}, buffs {buffs}");
                if (debuffs > 0 && roll)
                {
                    attacker.RemoveBuff(myDebuffDef);
                    if(buffs > 0)
                    {
                        attacker.RemoveBuff(myBuffDef);
                    }
                }
            }
        }

        private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffs = sender.GetBuffCount(myBuffDef);
            int debuffs = sender.GetBuffCount(myDebuffDef);
            if(buffs - debuffs != 0)
            {
                var beforeDmg = sender.baseDamage + args.baseDamageAdd + (sender.level - 1) * sender.levelDamage;
                var addedDmg = (beforeDmg) * Mathf.Pow(1.05f, (buffs - debuffs)) - (beforeDmg);
                Log.Info(addedDmg);
                args.baseDamageAdd += addedDmg;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(myItemDef.itemIndex), transform.position, transform.forward * 20f);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(myDebuffDef);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                PlayerCharacterMasterController.instances[0].master.GetBody().AddBuff(myBuffDef);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                test += 1f;
                Log.Info($"test: {test}");

            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                test -= 1f;
                Log.Info($"test: {test}");

            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                test *= 2f;
                Log.Info($"test: {test}");

            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                test /= 2f;
                Log.Info($"test: {test}");

            }
        }
    }
}
