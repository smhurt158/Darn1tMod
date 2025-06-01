using R2API;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Darn1tMod
{
    internal class PermanentMaxHealthDecreaseItem:ItemDef
    {

        public PermanentDamageDisplayBuff HealthDisplayBuff = new();
        public PermanentMaxHealthDecreaseItem() : base()
        {
            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            name = "PERMANENT_MAX_HEALTH_DECREASE";
            nameToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_NAME";
            pickupToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_PICKUP";
            descriptionToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_DESC";
            loreToken = "DAMAGE_ON_KILL_SLOWED_ON_HIT_LORE";


            #pragma warning disable CS0618
            deprecatedTier = ItemTier.NoTier;
            #pragma warning restore CS0618

            pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            canRemove = false;

            hidden = false;

            // https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/
            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(this, displayRules));
        }
    }
}
