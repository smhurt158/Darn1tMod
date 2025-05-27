using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace ExamplePlugin
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
            isDebuff = false;
            isHidden = false;
            startSfx = null;
            name = "HealthDisplayBuff";
            ignoreGrowthNectar = false;

            ContentAddition.AddBuffDef(this);
        }
    }
}
