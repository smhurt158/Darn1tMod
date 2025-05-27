using UnityEngine;
using System.IO;

namespace ExamplePlugin
{
    public static class Asset
    {
        //You will load the assetbundle and assign it to here.
        public static AssetBundle mainBundle;
        //A constant of the AssetBundle's name.
        public const string bundleName = "darn1t-items";
        // Uncomment this if your assetbundle is in its own folder. Of course, make sure the name of the folder matches this.
        // public const string assetBundleFolder = "AssetBundles";

        //The direct path to your AssetBundle
        public static string AssetBundlePath
        {
            get
            {
                //This returns the path to your assetbundle assuming said bundle is on the same folder as your DLL. If you have your bundle in a folder, you can instead uncomment the statement below this one.
                return Path.Combine(Path.GetDirectoryName(ExamplePlugin.PInfo.Location), bundleName);
                //return Path.Combine(Path.GetDirectoryName(MainClass.PInfo.Location), assetBundleFolder, bundleName);
            }
        }

        public static void Init()
        {
            //Loads the assetBundle from the Path, and stores it in the static field.
            mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }
    }
}
