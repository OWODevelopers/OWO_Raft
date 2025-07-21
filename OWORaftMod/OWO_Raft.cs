using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;

namespace OWORaftMod
{
    public class OWO_Raft : Mod
    {
        public OWOSkin owoSkin;

        public void Start()
        {
            owoSkin = new OWOSkin();
        }

        public void OnModUnload()
        {
            Debug.Log("[OWO_Raft] Mod unloaded");
        }
    }
}