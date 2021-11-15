using R2API;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace CustomItem
{
    internal static class Assets
    {
        internal static GameObject YellowSignPrefab;
        internal static Sprite YellowSignIcon;

        internal static ItemDef YellowSignItemDef;

        private const string ModPrefix = "@CustomItem:";

        internal static void Init()
        {
            // First registering your AssetBundle into the ResourcesAPI with a modPrefix that'll also be used for your prefab and icon paths
            // note that the string parameter of this GetManifestResourceStream call will change depending on
            // your namespace and file name
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KingInYellow.exampleitemmod"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);

                YellowSignPrefab = bundle.LoadAsset<GameObject>("Assets/Import/model/Sphere.prefab");
                YellowSignIcon = bundle.LoadAsset<Sprite>("Assets/Import/Icon/Icon.png");
            }

            YellowSignRedTierItem();

            AddLanguageTokens();
        }

        private static void YellowSignRedTierItem()
        {
            YellowSignItemDef = new ItemDef
            {
                name = "KingInYellow", // its the internal name, no spaces, apostrophes and stuff like that
                tier = ItemTier.Tier3,
                pickupModelPrefab = YellowSignPrefab,
                pickupIconSprite = YellowSignIcon,
                nameToken = "YELLOWSIGN_NAME", // stylised name
                pickupToken = "YELLOWSIGN_PICKUP",
                descriptionToken = "YELLOWSIGN_DESC",
                loreToken = "YELLOWSIGN_LORE",
                tags = new[]
                {
                    ItemTag.Utility
                }
            };

            var itemDisplayRules = new ItemDisplayRule[0]; // keep this null if you don't want the item to show up on the survivor 3d model. You can also have multiple rules !
            //itemDisplayRules[0].followerPrefab = YellowSignPrefab; // the prefab that will show up on the survivor
            //itemDisplayRules[0].childName = "Chest"; // this will define the starting point for the position of the 3d model, you can see what are the differents name available in the prefab model of the survivors
            //itemDisplayRules[0].localScale = new Vector3(0.15f, 0.15f, 0.15f); // scale the model
            //itemDisplayRules[0].localAngles = new Vector3(0f, 180f, 0f); // rotate the model
            //itemDisplayRules[0].localPos = new Vector3(-0.35f, -0.1f, 0f); // position offset relative to the childName, here the survivor Chest

            var KingInYellow = new R2API.CustomItem(YellowSignItemDef, itemDisplayRules);

            ItemAPI.Add(KingInYellow); // ItemAPI sends back the ItemIndex of your item
        }

        private static void AddLanguageTokens()
        {
            //The Name should be self explanatory
            LanguageAPI.Add("YELLOWSIGN_NAME", "The Yellow Sign");
            //The Pickup is the short text that appears when you first pick this up. This text should be short and to the point, nuimbers are generally ommited.
            LanguageAPI.Add("YELLOWSIGN_PICKUP", "Chance to turn an enemy into an ally");
            //The Description is where you put the actual numbers and give an advanced description.
            LanguageAPI.Add("YELLOWSIGN_DESC",
                "Gain a <style=cIsUtility>2%</style> <style=cStack>(+1% per stack)</style> chance to <style=cIsUtility>convert</style> an enemy into an ally.\nGives them the gift of the <style=cShrine>Yellow King.</style>");
            //The Lore is, well, flavor. You can write pretty much whatever you want here.
            LanguageAPI.Add("YELLOWSIGN_LORE",
                "Good morning USA I got a feeling that it's gonna be a wonderful day The sun in the sky has a smile on his face And he's shining a salute to the American race");
        }
    }
}
