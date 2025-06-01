using R2API;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Darn1tMod
{
    internal class PermanentDamageDisplayBuff : BuffDef
    {
        public PermanentDamageDisplayBuff() : base()
        {
            buffColor = Color.red;
            canStack = true;
            eliteDef = null;
            iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            isCooldown = false;
            isHidden = false;
            startSfx = null;
            name = "PermanentDamageDisplayBuff";

            //Item Interactions
            ignoreGrowthNectar = true;
            isDebuff = false;
            flags = Flags.ExcludeFromNoxiousThorns;

            ContentAddition.AddBuffDef(this);
        }
    }
}
