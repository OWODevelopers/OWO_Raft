using OWOGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace OWORaftMod
{
    public class OWOSkin
    {
        private string owoPath = "\\mods\\OWO";
        public bool suitEnabled = false;
        public bool canReceiveSensations =  false;

        public Dictionary<string, Sensation> FeedbackMap = new Dictionary<string, Sensation>();

        private bool fishingIsActive;
        private bool hookIsActive;
        private bool drowingIsActive;
        private bool swimmingIsActive;
        private bool bowIsActive;

        public OWOSkin()
        {
            RegisterAllSensationsFiles();
            InitializeOWO();
        }

        #region Skin Configuration

        private void RegisterAllSensationsFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + owoPath;

            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.owo", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    Sensation test = Sensation.Parse(tactFileStr);
                    FeedbackMap.Add(prefix, test);
                }
                catch (Exception e)
                {
                    LOG(e.Message);
                }

            }
        }

        private async void InitializeOWO()
        {
            LOG("Initializing OWO skin");

            var gameAuth = GameAuth.Create(AllBakedSensations()).WithId("31239533");
            LOG("Auth generated");

            OWO.Configure(gameAuth);
            string[] myIPs = GetIPsFromFile("OWO_Manual_IP.txt");
            if (myIPs.Length == 0) await OWO.AutoConnect();
            else
            {
                await OWO.Connect(myIPs);
            }

            if (OWO.ConnectionState == OWOGame.ConnectionState.Connected)
            {
                suitEnabled = true;
                LOG("OWO suit connected.");
                Feel("Equip", 2);
            }
            if (!suitEnabled)
                LOG("OWO is not enabled?!?!");
        }

        public BakedSensation[] AllBakedSensations()
        {
            var result = new List<BakedSensation>();

            foreach (var sensation in FeedbackMap.Values)
            {
                if (sensation is BakedSensation baked)
                {
                    LOG("Registered baked sensation: " + baked.name);
                    result.Add(baked);
                }
                else
                {
                    LOG("Sensation not baked? " + sensation);
                    continue;
                }
            }
            return result.ToArray();
        }

        public string[] GetIPsFromFile(string filename)
        {
            List<string> ips = new List<string>();
            string filePath = Directory.GetCurrentDirectory() + owoPath + filename;
            if (File.Exists(filePath))
            {
                LOG("Manual IP file found: " + filePath);
                var lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    if (IPAddress.TryParse(line, out _)) ips.Add(line);
                    else LOG("IP not valid? ---" + line + "---");
                }
            }
            return ips.ToArray();
        }

        ~OWOSkin()
        {
            DisconnectOWO();
        }

        public void DisconnectOWO()
        {
            OWO.Disconnect();
        }
        #endregion

        public void LOG(String msg)
        {
            Debug.Log($"[OWO_Raft] : {msg}");
        }

        #region Feel

        public void Feel(String key, int Priority = 0, int intensity = 0)
        {
            LOG($"{key}");
            Sensation toSend = GetBackedId(key);
            if (toSend == null || !CanFeel()) return;

            if (intensity != 0)
            {
                toSend = toSend.WithMuscles(Muscle.All.WithIntensity(intensity));
            }

            OWO.Send(toSend.WithPriority(Priority));
        }

        private Sensation GetBackedId(string sensationKey)
        {
            if (FeedbackMap.ContainsKey(sensationKey))
            {
                return FeedbackMap[sensationKey];
            }
            else
            {
                LOG($"Feedback not registered: {sensationKey}");
                return null;
            }
        }

        #endregion

        #region Loops

        #region Fishing

        public void StartFishing()
        {
            if (fishingIsActive) return;

            fishingIsActive = true;
            FishingFuncAsync();
        }

        public void StopFishing()
        {
            if (fishingIsActive == false) return;
            fishingIsActive = false;
        }

        public async Task FishingFuncAsync()
        {
            while (fishingIsActive)
            {
                Feel("Fishing", 0);
                await Task.Delay(200);
            }
        }

        #endregion Fishing

        #region Hook

        public void StartHook()
        {
            if (hookIsActive) return;

            hookIsActive = true;
            HookFuncAsync();
        }

        public void StopHook()
        {
            if (hookIsActive == false) return;
            hookIsActive = false;
        }

        public async Task HookFuncAsync()
        {
            while (hookIsActive)
            {
                Feel("Object Attach", 0);
                await Task.Delay(200);
            }
        }

        #endregion Fishing

        #region Drowning

        public void StartDrowning()
        {
            if (drowingIsActive) return;

            drowingIsActive = true;
            DrowningFuncAsync();
        }

        public void StopDrowning()
        {
            if(drowingIsActive == false) return;
            drowingIsActive = false;
        }

        public async Task DrowningFuncAsync()
        {
            while (drowingIsActive)
            {
                Feel("Drowning", 1);
                await Task.Delay(200);
            }
        }

        #endregion Drowning

        #region Swimming

        public void StartSwimming()
        {
            if (swimmingIsActive) return;

            swimmingIsActive = true;
            SwimmingFuncAsync();
        }

        public void StopSwimming()
        {
            if(swimmingIsActive == false) return;
            swimmingIsActive = false;
        }

        public async Task SwimmingFuncAsync()
        {
            while (swimmingIsActive)
            {
                Feel("Swimming", 0);
                await Task.Delay(300);
            }
        }

        #endregion Swimming

        #region Bow
        public void StartBow()
        {
            if (bowIsActive) return;

            bowIsActive = true;
            BowFuncAsync();
        }

        public void StopBow()
        {
            if (bowIsActive == false) return;
            bowIsActive = false;
        }

        public async Task BowFuncAsync()
        {
            while (bowIsActive)
            {
                Feel("Bow Pull", 0);
                await Task.Delay(300);
            }
        } 
        #endregion


        #endregion Loops

        public void StopAllHapticFeedback()
        {
            StopFishing();
            StopHook();
            StopDrowning();
            StopSwimming();
            OWO.Stop();
        }

        public bool CanFeel()
        {
            return suitEnabled;
        }
    }
}