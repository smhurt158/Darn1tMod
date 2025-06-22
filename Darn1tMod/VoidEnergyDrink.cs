using R2API;
using RoR2;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.ExpansionManagement;
using System.Linq;

namespace Darn1tMod
{
    internal class VoidEnergyDrink:ItemDef
    {
        VoidEnergyDrinkSpeedBuff speedBuff;
        public VoidEnergyDrink() : base()
        {
            speedBuff = new VoidEnergyDrinkSpeedBuff();
            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            name = "VOID_ENERGY_DRINK_NAME";
            nameToken = "VOID_ENERGY_DRINK_NAME";
            pickupToken = "VOID_ENERGY_DRINK_PICKUP";
            descriptionToken = "VOID_ENERGY_DRINK_DESC";
            loreToken = "VOID_ENERGY_DRINK_LORE";


            #pragma warning disable CS0618
            deprecatedTier = ItemTier.VoidTier1;
            #pragma warning restore CS0618

            //requiredExpansion = ExpansionCatalog.expansionDefs.First(def => def.nameToken == "DLC1_NAME");
            ItemRelationshipProvider itemRelationshipProvider = new ItemRelationshipProvider();
            itemRelationshipProvider.relationshipType = Addressables.LoadAssetAsync<ItemRelationshipType>("RoR2/DLC1/Common/ContagiousItem.asset").WaitForCompletion();


            var energyDrink = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/SprintBonus/SprintBonus.asset").WaitForCompletion();
            itemRelationshipProvider.relationships = [
                new ItemDef.Pair
                {
                    itemDef1 = energyDrink,
                    itemDef2 = this,
                }
            ];

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

            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;

            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients;

        }

        private void GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var buffCount = sender.GetBuffCount(speedBuff);
            var itemCount = sender.inventory.GetItemCount(this);


            var beforeSprintSpeed = sender.sprintingSpeedMultiplier + args.sprintSpeedAdd;
            var addedSprintSpeed = beforeSprintSpeed * (1 + buffCount * .03f * itemCount) - (beforeSprintSpeed);
            Debug.Log("SEAN HURT ADDING SPRINT SPEED " + addedSprintSpeed);

            args.sprintSpeedAdd += addedSprintSpeed;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            
            var count = body.inventory.GetItemCount(this);
            VoidEnergyDrinkBehavior voidEnergyDrinkBehavior = body.GetComponent<VoidEnergyDrinkBehavior>();
            if (count > 0 && !voidEnergyDrinkBehavior)
            {
                Debug.Log("SEAN HURT Got ITEM");
                voidEnergyDrinkBehavior = body.gameObject.AddComponent<VoidEnergyDrinkBehavior>();
                voidEnergyDrinkBehavior.CharacterBody = body;
                voidEnergyDrinkBehavior.speedBuff = speedBuff;
            }

            if (voidEnergyDrinkBehavior)
            {
                Debug.Log("SEAN HURT SETTING STACKS " + count);
                voidEnergyDrinkBehavior.itemStacks = count;
            }
        }

        public class VoidEnergyDrinkBehavior : MonoBehaviour
        {
            public VoidEnergyDrinkSpeedBuff speedBuff;
            public CharacterBody CharacterBody;
            public int itemStacks = 0;

            private const float timerMax = 1;
            private float timer = 0;

            private const int maxBuffsPerItem = 5;

            private bool wasSprinting = false;
            public void FixedUpdate()
            {
                if (CharacterBody.isSprinting)
                {
                    wasSprinting = true;
                    timer += Time.fixedDeltaTime;
                    if(timer >= timerMax)
                    {
                        Debug.Log("SEAN HURT Adding buff");
                        CharacterBody.AddBuff(speedBuff);
                        timer = 0;
                    }
                }
                else if(wasSprinting)
                {
                    Debug.Log("SEAN HURT Removing buff");

                    timer = 0;
                    CharacterBody.SetBuffCount(speedBuff.buffIndex, 0);
                    wasSprinting = false;
                }
            }
        }

        public class VoidEnergyDrinkSpeedBuff : BuffDef
        {
            public VoidEnergyDrinkSpeedBuff() : base()
            {
                canStack = true;
                eliteDef = null;
                isCooldown = false;
                isHidden = true;
                startSfx = null;
                name = "VoidEnergyDrinkSpeedBuff";

                //Item Interactions
                ignoreGrowthNectar = true;
                isDebuff = false;
                flags = Flags.ExcludeFromNoxiousThorns;

                ContentAddition.AddBuffDef(this);
            }
        }
    }
}
