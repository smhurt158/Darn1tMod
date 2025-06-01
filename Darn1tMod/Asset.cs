using UnityEngine;
using System.IO;

namespace Darn1tMod
{
    public static class Asset
    {
        public static AssetBundle mainBundle;

        public const string bundleName = "darn1tmod";

        public static void Init()
        {
            string AssetBundlePath = Path.Combine(Path.GetDirectoryName(Darn1tMod.PInfo.Location), bundleName);
            //Loads the assetBundle from the Path, and stores it in the static field.
            mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }
    }
}
