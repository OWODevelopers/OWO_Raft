using HarmonyLib;
using HMLLibrary;
using RaftModLoader;
using System.Reflection;
﻿using UnityEngine;

namespace OWORaftMod
{
    public class OWO_Raft : Mod
    {
        public OWOSkin owoSkin;

        public void Start()
        {
            owoSkin = new OWOSkin();

            var harmony = new Harmony("owo.patch.Raft");
            harmony.PatchAll();
        }

        public void OnModUnload()
        {
            Debug.Log("[OWO_Raft] Mod unloaded");
        }
    }
}