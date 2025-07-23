using HarmonyLib;
using HMLLibrary;
using UnityEngine;

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
            Debug.Log("[OWO_Raft] Mod unloaded");
        }

        #region PLAYER CONTROLLER

        //[HarmonyPatch(typeof(PersonController), "OnHitGround")]
        //public class Patch_OnHitGround
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        owoSkin.LOG("FALL POST");
        //    }
        //}

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