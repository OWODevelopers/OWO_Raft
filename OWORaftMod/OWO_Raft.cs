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

        private static ControllerType controllerType;

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
                    owoSkin.Feel("Jump");
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
                owoSkin.Feel("Landing");
            }
        }

        [HarmonyPatch(typeof(PersonController), "SwitchControllerType")]
        public class Patch_SwitchControllerType
        {

            [HarmonyPostfix]
            public static void Postfix(ControllerType newType)
            {
                if (newType == controllerType)
                    return;
                controllerType = newType;

                switch (newType)
                {
                    case ControllerType.Water:
                        owoSkin.Feel("Splash");
                        owoSkin.StartSwimming();
                        break;

                    case ControllerType.Ground:
                        owoSkin.StopSwimming();
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(Paddle), "PaddlePaddle")]
        public class Patch_PaddlePaddle
        {
            [HarmonyPostfix]
            public static void Postfix(Vector3 position, Vector3 direction, float force)
            {
                owoSkin.Feel("Paddeling");
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
                    case EntityType.Player:
                        break;
                    case EntityType.Enemy:
                    case EntityType.FallDamage:
                    case EntityType.Environment:
                        owoSkin.Feel("Hurt", 1);
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
                owoSkin.StopAllHapticFeedback();
                owoSkin.Feel("Death", 4);
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
                    owoSkin.Feel("Consume", 2);
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
                    owoSkin.StartDrowning();
                }
                else
                {
                    owoSkin.StopDrowning();
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
                if (__instance.selectedRecipeBox.ItemToCraft == null)
                    return;

                owoSkin.Feel("Craft", 2);
            }
        }

        #endregion

        #region ITEM COLLECTOR
        [HarmonyPatch(typeof(ItemCollector), "CollectItem")]
        public class Patch_CollectItem
        {
            [HarmonyPostfix]
            public static void Postfix(ItemCollector __instance, PickupItem_Networked item)
            {
                owoSkin.StartHook();
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

                owoSkin.Feel("Pickup Item", 2);
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
                owoSkin.Feel("Melee", 2);
            }
        }

        [HarmonyPatch(typeof(MeleeWeapon), "OnHitEntity")]
        public class Patch_OnHitEntity
        {
            [HarmonyPostfix]
            public static void Postfix(RaycastHit hit, Network_Entity entity)
            {
                if (entity == null)
                    return;

                owoSkin.Feel("Melee Hit", 2);
            }
        }

        [HarmonyPatch(typeof(ThrowableComponent_Bow), "CallStartChargeEvent")]
        public class Patch_CallStartChargeEvent
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StartBow();
            }
        }

        [HarmonyPatch(typeof(ThrowableComponent), "ReleaseHand")]
        public class Patch_ReleaseHand
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StopBow();
                owoSkin.Feel("Bow Release", 2);
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
                owoSkin.Feel("Hook Throw", 2);
            }
        }

        [HarmonyPatch(typeof(Hook), "ResetHookToPlayer")]
        public class Patch_ResetHookToPlayer
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StopHook();
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
                owoSkin.Feel("Rod Throw", 2);
            }
        }

        [HarmonyPatch(typeof(FishingRod), "OnFishGrabBait")]
        public class Patch_OnFishGrabBait
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StartFishing();
            }
        }

        [HarmonyPatch(typeof(FishingRod), "ResetRod")]
        public class Patch_ResetRod
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.StopFishing();
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
                owoSkin.Feel("Digging", 2);
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
                owoSkin.Feel("Equip", 2);
            }
        }

        [HarmonyPatch(typeof(Equipment_ArmorPiece), "UnEquip")]
        public class Patch_UnEquipArmor
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                owoSkin.Feel("Unequip", 2);
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
                    owoSkin.Feel("Unequip", 2);
                }
                else
                {
                    owoSkin.Feel("Equip", 2);
                }
            }
        }
        #endregion
    }
}