using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;

namespace ExamplePlugin
{
    internal class SpeedNerf:BuffDef
    {
        public SpeedNerf():base()
        {
            buffColor = Color.white;
            canStack = true;
            eliteDef = null;
            iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            isCooldown = false;
            isDebuff = true;
            isHidden = false;
            startSfx = null;
            name = "SpeedNerf";

            ContentAddition.AddBuffDef(this);
        }
    }
}
