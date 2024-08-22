using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using static RoR2.RoR2Content;
using JetBrains.Annotations;

namespace ExamplePlugin
{
    internal class PermanentDamageArtifact : ArtifactDef
    {
        public PermanentMaxHealthDecreaseItem PermanentMaxHealthDecreaseItem = new();
        public PermanentDamageArtifact() 
        {
            cachedName = "RiskyArtifactOfArrogance";
            nameToken = "RISKYARTIFACTS_ARROGANCE_NAME";
            descriptionToken = "RISKYARTIFACTS_ARROGANCE_DESC";
            smallIconDeselectedSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            smallIconSelectedSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();

            ContentAddition.AddArtifactDef(this);

            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(this))
            {

            }
        }
    }
}
