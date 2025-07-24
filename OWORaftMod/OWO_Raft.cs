using System;
using UnityEngine;

using HarmonyLib;
using HMLLibrary;

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

        [HarmonyPatch(typeof(PlayerAnimator), "SetAnimation", new Type[] { typeof(PlayerAnimation), typeof(bool) })]
        public class Patch_SetAnimation
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerAnimation animation, bool triggering)
            {
                if (animation == PlayerAnimation.Trigger_Jump)
                {
                    owoSkin.LOG($"JUMPED!");
                }
            }
        }

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
                    //TODO: Swimming controller indicates player is swimming
                }
            }
        }

        [HarmonyPatch(typeof(Paddle), "PaddlePaddle")]
        public class Patch_PaddlePaddle
        {
            [HarmonyPostfix]
            public static void Postfix(Vector3 position, Vector3 direction, float force)
            {
                owoSkin.LOG($"PADDLE!");
            }
        }

        #endregion

        #region GAME MANAGER

        [HarmonyPatch(typeof(GameManager), "OnWorldRecievedLate")]
        public class Patch_OnWorldReceivedLate
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.canReceiveSensations = true;
            }
        }

        [HarmonyPatch(typeof(Raft_Network), "LoadScene")]
        public class Patch_LoadScene
        {
            [HarmonyPostfix]
            public static void Postfix(string sceneName)
            {
                if (sceneName == "MainMenuScene")
                {
                    owoSkin.StopAllHapticFeedback();
                    owoSkin.canReceiveSensations = false;
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

        #region CRAFT

        [HarmonyPatch(typeof(CraftingMenu), "CraftItem")]
        public class Patch_CraftItem
        {
            [HarmonyPostfix]
            public static void Postfix(CraftingMenu __instance)
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                if (__instance.selectedRecipeBox.ItemToCraft == null)
                    return;

                owoSkin.LOG($"CRAFTED {__instance.selectedRecipeBox.ItemToCraft}!");
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

        [HarmonyPatch(typeof(Pickup), "PickupItem", new Type[] { typeof(PickupItem), typeof(bool), typeof(bool) })]
        public class Patch_PickupItem
        {
            [HarmonyPostfix]
            public static void Postfix(PickupItem pickup, bool forcePickup, bool triggerHandAnimation)
            {
                if (!forcePickup && !pickup.canBePickedUp)
                {
                    return;
                }

                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("PICKED UP ITEM!");
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

        #region SHOVEL

        [HarmonyPatch(typeof(Shovel), "DigDown")]
        public class Patch_DigDown
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                //if(__instance.playerNetwork.isLocalPlayer)

                owoSkin.LOG("DIGGING A HOLE!");
            }
        }

        #endregion

        #region EQUIPMENT
        [HarmonyPatch(typeof(Equipment_ArmorPiece), "Equip")]
        public class Patch_EquipArmor
        {
            [HarmonyPostfix]
            public static void Postfix(Slot_Equip equippedSlot)
            {
                if (!owoSkin.canReceiveSensations) return;

                owoSkin.LOG($"Equipping armor {equippedSlot.itemInstance.UniqueName}");
                owoSkin.Feel("Equip", 2);
            }
        }

        [HarmonyPatch(typeof(Equipment_ArmorPiece), "UnEquip")]
        public class Patch_UnEquipArmor
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.LOG($"Unequipping armor");
            }
        }

        [HarmonyPatch(typeof(Equipment_Hat), "SetModelState")]
        public class Patch_EquipHat
        {
            [HarmonyPostfix]
            public static void Postfix(bool state)
            {
                if (!owoSkin.canReceiveSensations) return;

                if (state == false)
                {
                    owoSkin.LOG($"Unequip hat");
                }
                else
                {
                    owoSkin.LOG($"Equipping hat");
                }
            }
        }
        #endregion
    }
}