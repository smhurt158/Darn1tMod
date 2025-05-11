using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace ExamplePlugin
{
    internal class HealthDisplayBuff : BuffDef
    {
        public HealthDisplayBuff() : base()
        {
            buffColor = Color.red;
            canStack = true;
            eliteDef = null;
            iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            isCooldown = false;
            isDebuff = true;
            isHidden = false;
            startSfx = null;
            name = "HealthDisplayBuff";

            ContentAddition.AddBuffDef(this);
        }
    }
}
