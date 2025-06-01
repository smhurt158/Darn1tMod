using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;

namespace Darn1tMod
{
    internal class SpeedNerf:BuffDef
    {
        public SpeedNerf():base()
        {
            buffColor = Color.blue;
            canStack = true;
            eliteDef = null;
            iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            isCooldown = false;
            isHidden = false;
            startSfx = null;
            name = "SpeedNerf";

            //Item Interactions
            ignoreGrowthNectar = false;
            isDebuff = true;

            ContentAddition.AddBuffDef(this);
        }
    }
}
