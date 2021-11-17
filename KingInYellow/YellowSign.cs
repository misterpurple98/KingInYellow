using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace CustomItem
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class KingInYellow : BaseUnityPlugin
    {
        private const string ModVer = "0.0.1";
        private const string ModName = "The Yellow Sign";
        public const string ModGuid = "com.MisterPurple98.KingInYellow";

        internal new static ManualLogSource Logger;
        public void Awake()
        {
            Logger = base.Logger;

            Assets.Init();
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) => { DoChecks(orig, self, damageInfo, victim); };
        }

        public static void DoChecks(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!NetworkServer.active || !victim || !damageInfo.attacker || damageInfo.procCoefficient <= 0f) return;

            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            CharacterBody vic2 = victim.GetComponent<CharacterBody>();

            if (!vic2.mainHurtBox || !vic2.healthComponent || !vic2 || !body) return;

            if (!body.Equals(null) || body.isPlayerControlled || !vic2.Equals(null) || !vic2.inventory.Equals(null))
            {
                if (!vic2.master.isBoss && BossGroup.FindBossGroup(vic2) is null && vic2.master.teamIndex is TeamIndex.Monster)
                {
                    int icnt = body.inventory.GetItemCount(Assets.YellowSignItemDef);
                    if (icnt > 0)
                    {
                        icnt--;
                        float aProc = 0.5f;
                        aProc += 0.5f * (icnt);
                        //aProc = 100;

                        if (Util.CheckRoll(aProc * damageInfo.procCoefficient, body.master))
                        {
                            MakeFriend(vic2);
                        }
                    }
                }
            }
        }
        public static void MakeFriend(CharacterBody vic2)
        {
            vic2.master.teamIndex = TeamIndex.Player;
            vic2.teamComponent.teamIndex = TeamIndex.Player;
            vic2.baseMaxHealth *= 2f;
            if (!vic2.master.IsDeadAndOutOfLivesServer())
            {
                vic2.healthComponent.health = vic2.maxHealth;
            }
            vic2.baseArmor *= 4f;
            vic2.baseDamage *= 2f;
            vic2.baseAttackSpeed *= 3f;
            vic2.baseRegen = -(vic2.maxHealth * 0.5f);

            var baseAi = vic2.master.GetComponent<RoR2.CharacterAI.BaseAI>();
            baseAi.currentEnemy.Reset();
            baseAi.ForceAcquireNearestEnemyIfNoCurrentEnemy();

            var players = TeamComponent.GetTeamMembers(TeamIndex.Player);
            foreach (var player in players)
            {
                if (!player.body.isPlayerControlled)
                {
                    var ai = player.body.master.GetComponent<RoR2.CharacterAI.BaseAI>();
                    if (ai.currentEnemy.characterBody.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        ai.currentEnemy.Reset();
                        ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                }
            }
        }
    }
}