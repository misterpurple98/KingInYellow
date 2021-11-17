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
                name = "KingInYellow",
                tier = ItemTier.Tier3,
                pickupModelPrefab = YellowSignPrefab,
                pickupIconSprite = YellowSignIcon,
                nameToken = "YELLOWSIGN_NAME",
                pickupToken = "YELLOWSIGN_PICKUP",
                descriptionToken = "YELLOWSIGN_DESC",
                loreToken = "YELLOWSIGN_LORE",
                tags = new[]
                {
                    ItemTag.Utility
                }
            };

            var itemDisplayRules = new ItemDisplayRule[0];

            var KingInYellow = new R2API.CustomItem(YellowSignItemDef, itemDisplayRules);

            ItemAPI.Add(KingInYellow);
        }

        private static void AddLanguageTokens()
        {
            LanguageAPI.Add("YELLOWSIGN_NAME", "The Yellow Sign");
            LanguageAPI.Add("YELLOWSIGN_PICKUP", "Chance to turn an enemy into an ally");
            LanguageAPI.Add("YELLOWSIGN_DESC",
                "Gain a <style=cIsUtility>0.5%</style> <style=cStack>(+0.5% per stack)</style> chance to <style=cIsUtility>convert</style> an enemy into an ally.\nGives them the <style=cIsUtility>boon</style> of the <style=cShrine>Yellow King.</style>");
            LanguageAPI.Add("YELLOWSIGN_LORE",
                "Good morning USA I got a feeling that it's gonna be a wonderful day The sun in the sky has a smile on his face And he's shining a salute to the American race");
        }
    }
}
