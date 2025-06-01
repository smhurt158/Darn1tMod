using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;

namespace Darn1tMod
{
    internal class DamageBuff:BuffDef
    {
        public DamageBuff() : base()
        {
            buffColor = Color.white;
            canStack = true;
            eliteDef = null;
            iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            isCooldown = false;
            isHidden = false;
            startSfx = null;
            name = "DamageBuff";

            //Item Interactions
            ignoreGrowthNectar = false;
            isDebuff = false;
            flags = Flags.ExcludeFromNoxiousThorns;

            ContentAddition.AddBuffDef(this);
        }
    }
}
