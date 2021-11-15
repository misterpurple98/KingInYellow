using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

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
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) => { MakeFriend(orig, self, damageInfo, victim); };
        }
        public static void MakeFriend(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            CharacterBody vic2 = victim.GetComponent<CharacterBody>();

            if (!body.Equals(null) || body.isPlayerControlled)
            {
                int icnt = body.inventory.GetItemCount(Assets.YellowSignItemDef);
                if (icnt > 0)
                {
                    if (!vic2.master.isBoss && BossGroup.FindBossGroup(vic2) is null)
                    {
                        //if (Util.CheckRoll((2 + 1 * icnt) * damInfo.procCoefficient, body.master))
                        if (Util.CheckRoll(100, body.master))
                        {
                            vic2.master.teamIndex = TeamIndex.Player;
                            vic2.teamComponent.teamIndex = TeamIndex.Player;
                            vic2.baseMaxHealth *= 2;
                            if (!vic2.master.IsDeadAndOutOfLivesServer())
                            {
                                vic2.healthComponent.health = vic2.maxHealth;
                            }
                            vic2.baseArmor *= 4;
                            vic2.baseDamage *= 2;
                            vic2.baseAttackSpeed *= 4;

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
            }
        }
    }
}