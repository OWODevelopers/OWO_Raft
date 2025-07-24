using HarmonyLib;
using HMLLibrary;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace OWORaftMod
{
    public class OWO_Raft : Mod
    {
        public static OWOSkin owoSkin;
        private Harmony harmony;

        public void Start()
        {
            owoSkin = new OWOSkin();

            harmony = new Harmony("owo.patch.Raft");
            harmony.PatchAll();
        }

        public void OnModUnload()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll("owo.patch.Raft");
            }
        }

        #region PERSON CONTROLLER

        [HarmonyPatch(typeof(PersonController), "ResetJump")]
        public class Patch_ResetJump
        {
            [HarmonyPostfix]
            public static void Postfix(PersonController __instance)
            {
                if (__instance.controllerType == ControllerType.Water)
                    return;
                owoSkin.LOG($"RESET JUMP!. Controller: {__instance.controllerType}");
            }
        }

        [HarmonyPatch(typeof(PersonController), "SwitchControllerType")]
        public class Patch_SwitchControllerType
        {
            [HarmonyPostfix]
            public static void Postfix(ControllerType newType)
            {
                owoSkin.LOG($"Controller: {newType}");
                switch (newType)
                {
                    //TODO
                }
            }
        }
        #endregion

        #region PLAYER STATS
        [HarmonyPatch(typeof(PlayerStats), "Damage")]
        public class Patch_Damage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerStats __instance, float damage, Vector3 hitPoint, Vector3 hitNormal, EntityType damageInflictorEntityType, SO_Buff buffAsset = null)
            {
                if (__instance.IsDead)
                    return;

                switch (damageInflictorEntityType)
                {
                    case EntityType.None:
                        break;
                    case EntityType.Player:
                        break;
                    case EntityType.Enemy:
                        owoSkin.LOG("ENEMY DAMAGE POST");
                        owoSkin.Feel("Hurt", 1);
                        break;
                    case EntityType.FallDamage:
                        owoSkin.LOG("FALL DAMAGE POST");
                        owoSkin.Feel("Hurt", 1);
                        break;
                    case EntityType.Environment:
                        owoSkin.LOG("ENVIRONMENT DAMAGE POST");
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerStats), "SendOnDeath")]
        public class Patch_SendOnDeath
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerStats __instance)
            {
                if (__instance.IsDead)
                    return;

                owoSkin.LOG("DEATH POST");
            }
        }

        [HarmonyPatch(typeof(PlayerStats), "Consume")]
        public class Patch_Consume
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerStats __instance, Item_Base edibleItem)
            {
                if (edibleItem != null)
                {
                    owoSkin.LOG($"Consuming item {edibleItem.UniqueName}");
                }
            }
        }

        [HarmonyPatch(typeof(Stat_Oxygen), "Update")]

        public class Patch_OxygenUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(Stat_Oxygen __instance)
            {
                if (__instance.Value <= 0)
                {
                    owoSkin.LOG($"YOU ARE DROWNING!!");
                    //owoSkin.StartDrowning();
                }
                else
                {
                    //owoSkin.StopDrowning();
                }
            }
        }
        #endregion

        #region ITEM COLLECTOR
        [HarmonyPatch(typeof(ItemCollector), "CollectItem")]
        public class Patch_CollectItem
        {
            [HarmonyPostfix]
            public static void Postfix(PickupItem_Networked item)
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("YOU COLLECTED WITH HOOK!");
            }
        }
        #endregion

        #region WEAPONS

        [HarmonyPatch(typeof(MeleeWeapon), "OnMeleeStart")]
        public class Patch_OnMeleeStart
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("START MELEE ATTACK!");
            }
        }

        [HarmonyPatch(typeof(MeleeWeapon), "OnHitEntity")]
        public class Patch_OnHitEntity
        {
            [HarmonyPostfix]
            public static void Postfix(RaycastHit hit, Network_Entity entity)
            {
                //if(__instance.playerNetwork.isLocalPlayer)
                if (entity == null)
                    return;

                owoSkin.LOG("HIT!");
            }
        }

        [HarmonyPatch(typeof(ThrowableComponent_Bow), "CallStartChargeEvent")]
        public class Patch_CallStartChargeEvent
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)
                owoSkin.LOG("START CHARGING BOW!");
                //owoSkin.StartChargingBow();
            }
        }

        [HarmonyPatch(typeof(ThrowableComponent), "ReleaseHand")]
        public class Patch_ReleaseHand
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)
                owoSkin.LOG("RELEASE HAND!");
                //owoSkin.StopChargingBow();
            }
        }

        #endregion

        #region HOOK

        [HarmonyPatch(typeof(Hook), "OnTrow")]
        public class Patch_OnTrow
        {
            [HarmonyPostfix]
            public static void Postfix(Hook __instance)
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("THROW HOOK!");
            }
        }

        [HarmonyPatch(typeof(Hook), "ResetHookToPlayer")]
        public class Patch_ResetHookToPlayer
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("RESET HOOK!");
            }
        }

        #endregion

        #region FISHING ROD

        [HarmonyPatch(typeof(FishingRod), "OnThrow")]
        public class Patch_OnThrow
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("FISHING ROD THROW!");
            }
        }

        [HarmonyPatch(typeof(FishingRod), "OnFishGrabBait")]
        public class Patch_OnFishGrabBait
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                //owoSkin.StartPullingFish();
                owoSkin.LOG("FISHING GRAB BAIT!");
            }
        }

        [HarmonyPatch(typeof(FishingRod), "ResetRod")]
        public class Patch_ResetRod
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                //owoSkin.StopPullingFish();
                owoSkin.LOG("STOPPED FISHING!");
            }
        }

        #endregion

        #region ARMOR
        [HarmonyPatch(typeof(Equipment_ArmorPiece), "Equip")]
        public class Patch_Equip
        {
            [HarmonyPostfix]
            public static void Postfix(Slot_Equip equippedSlot)
            {
                owoSkin.LOG($"Equipping item {equippedSlot.itemInstance.UniqueName}");
                owoSkin.Feel("Equip", 2);
            }
        }

        [HarmonyPatch(typeof(Equipment_ArmorPiece), "UnEquip")]
        public class Patch_UnEquip
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.LOG($"Unequipping item");
            }
        }
        #endregion

        #region BLOCKS

        //[HarmonyPatch(typeof(BlockCreator), "CreateBlock")]
        //public class Patch_CreateBlock
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(Item_Base blockItem, Vector3 localBuildPosition, Vector3 localBuildRotation, DPS dpsType, int hotslotIndex, bool replicating, uint blockObjectIndex, uint networkedObjectIndex, uint networkedBehaviourIndex)
        //    {
        //        owoSkin.LOG($"Placed {blockItem.UniqueName}");
        //    }
        //}

        #endregion
    }
}