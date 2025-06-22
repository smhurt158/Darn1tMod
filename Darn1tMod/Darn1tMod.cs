using BepInEx;
using R2API;
using RoR2;
using UnityEngine;

namespace Darn1tMod
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]

    // This one is because we use a .language file for language tokens
    // More info in https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
    [BepInDependency(LanguageAPI.PluginGUID)]

    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    // BaseUnityPlugin itself inherits from MonoBehaviour https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Darn1tMod : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public static PluginInfo PInfo { get; private set; }

        public const string PluginAuthor = "Darn1t";
        public const string PluginName = "Darn1tMod";
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginVersion = "0.1.0";

        private static DamageOnKillSlowedOnHitItem damageOnKillSlowedOnHit;
        private static VoidEnergyDrink voidEnergyDrink;
        private static PermanentDamageArtifact permanentDamageArtifact;

        public void Awake()
        {
            Log.Init(Logger);

            PInfo = Info;
            Asset.Init();
            Log.Info($"SEAN HURT LOG INIT");
            damageOnKillSlowedOnHit = new DamageOnKillSlowedOnHitItem();
            permanentDamageArtifact = new PermanentDamageArtifact();
            voidEnergyDrink = new VoidEnergyDrink();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //Instantiate(Asset.mainBundle.LoadAsset<GameObject>("cube.prefab"), transform.position + new Vector3(0, -1, 0), Quaternion.identity);
                Log.Info($"SEAN HURT Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(damageOnKillSlowedOnHit.itemIndex), transform.position, transform.forward * 20f);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                Log.Info($"SEAN HURT Player pressed F3. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(voidEnergyDrink.itemIndex), transform.position, transform.forward * 20f);
            }
        }
    }
}
