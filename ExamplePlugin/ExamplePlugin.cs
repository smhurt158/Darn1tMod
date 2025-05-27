using BepInEx;
using BepInEx.Logging;
using R2API;
using RoR2;
using UnityEngine;

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
        public static PluginInfo PInfo { get; private set; }

        private float test = 0;
        private float test2 = 0;
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Darn1t";
        public const string PluginName = "ExamplePlugin";
        public const string PluginVersion = "1.0.0";

        private static DamageOnKillSlowedOnHitItem damageOnKillSlowedOnHit;
        private static PermanentDamageArtifact permanentDamageArtifact;

        public void Awake()
        {
            PInfo = Info;
            Log.Init(Logger);
            Log.Info($"SEAN HURT LOG INIT");
            damageOnKillSlowedOnHit = new DamageOnKillSlowedOnHitItem();
            permanentDamageArtifact = new PermanentDamageArtifact();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                Log.Info($"SEAN HURT Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(damageOnKillSlowedOnHit.itemIndex), transform.position, transform.forward * 20f);
            }
        }
    }
}
