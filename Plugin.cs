using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // For Image

namespace MycopunkSkipIntro
{
    [BepInPlugin("com.yourname.skipintro", "SkipIntro", "1.0.0")]
    [MycoMod(null, ModFlags.IsClientSide)]
    public class SkipIntroPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.yourname.mycopunkskipintro";
        public const string PluginName = "Mycopunk Skip Intro";
        public const string PluginVersion = "1.0.0";

        private Harmony _harmony;
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            var harmony = new Harmony("com.yourname.skipintro");
            harmony.PatchAll(typeof(IntroPatches));
            Logger.LogInfo($"{harmony.Id} loaded!");
        }
    }

    internal class IntroPatches
    {
        // Patch private Awake using string name
        [HarmonyPatch(typeof(StartMenu), "Awake")]
        [HarmonyPrefix]
        private static bool SkipIntroPrefix(StartMenu __instance)
        {
            // Traverse for static hasInitialized
            var staticTraverse = Traverse.Create(typeof(StartMenu));
            bool hasInitialized = staticTraverse.Field("hasInitialized").GetValue<bool>();

            if (!hasInitialized)
            {
                SkipIntroPlugin.Logger.LogInfo("Skipping Mycopunk intro sequence via Awake prefix...");

                var instanceTraverse = Traverse.Create(__instance);

                // Mark as initialized so original Awake skips the else block (coroutine start)
                staticTraverse.Field("hasInitialized").SetValue(true);

                // Manually call the else-block code that would be skipped (e.g., initialize settings)
                Type gameManagerType = AccessTools.TypeByName("GameManager");
                if (gameManagerType != null)
                {
                    Traverse.Create(gameManagerType).Method("InitializeSettingRecursive", new Type[] { typeof(Transform) }).GetValue(instanceTraverse.Field("settingsWindow").GetValue<Window>().transform);
                }
                else
                {
                    SkipIntroPlugin.Logger.LogWarning("GameManager type not found—settings initialization may be skipped.");
                }

                // Set UI to post-intro state (mimic if(hasInitialized) block and coroutine end)
                instanceTraverse.Field("initializeScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("startScreen").GetValue<RectTransform>().gameObject.SetActive(true);

                // Hide/deactivate intro elements (prevent any partial activation)
                instanceTraverse.Field("bootingScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("splashScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("initializeBar").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("logText").GetValue<TextMeshProUGUI>().gameObject.SetActive(false);
                instanceTraverse.Field("initializeStuff").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("verifiedText").GetValue<TextMeshProUGUI>().gameObject.SetActive(false);
                instanceTraverse.Field("loadingBar").GetValue<Image>().gameObject.SetActive(false);

                // Set wipe to final hidden state
                RectTransform initializeWipe = instanceTraverse.Field("initializeWipe").GetValue<RectTransform>();
                initializeWipe.anchoredPosition = new Vector2(0f, -2160f);
                instanceTraverse.Field("wipeChild").GetValue<RectTransform>().anchoredPosition = new Vector2(0f, 2160f);
                initializeWipe.gameObject.SetActive(false);

                // Unlock cursor
                Type playerInputType = AccessTools.TypeByName("PlayerInput");
                if (playerInputType != null)
                {
                    Traverse.Create(playerInputType).Method("UnlockCursor").GetValue();
                }
                else
                {
                    SkipIntroPlugin.Logger.LogWarning("PlayerInput type not found—cursor may remain locked.");
                }

                // Handle music if not playing
                bool isMusicPlaying = instanceTraverse.Field("isMusicPlaying").GetValue<bool>();
                if (!isMusicPlaying)
                {
                    object musicObj = instanceTraverse.Field("music").GetValue();
                    if (musicObj != null && musicObj.GetType().ToString().Contains("AK.Wwise.Event"))
                    {
                        Traverse.Create(musicObj).Method("Post", new Type[] { typeof(GameObject) }).GetValue(__instance.gameObject);
                        instanceTraverse.Field("isMusicPlaying").SetValue(true);
                    }
                }

                // Handle first-run profile check and auto-host
                Type playerDataType = AccessTools.TypeByName("PlayerData");
                if (playerDataType != null)
                {
                    object playerData = Traverse.Create(playerDataType).Field("Instance").GetValue();
                    if (playerData != null)
                    {
                        object profileConfig = Traverse.Create(playerData).Property("ProfileConfig").GetValue();
                        if (profileConfig != null)
                        {
                            object currentProfile = Traverse.Create(profileConfig).Property("CurrentProfile").GetValue();
                            if (currentProfile != null)
                            {
                                bool isValid = Traverse.Create(currentProfile).Method("IsValid").GetValue<bool>();
                                int autohubFlag = Traverse.Create(playerData).Method("GetFlag", new Type[] { typeof(string) }).GetValue<int>("autohub");

                                if (!isValid && autohubFlag == 0)
                                {
                                    Traverse.Create(playerData).Method("SetFlag", new Type[] { typeof(string), typeof(int) }).GetValue("autohub", 1);
                                    instanceTraverse.Method("Host").GetValue();
                                }
                            }
                        }
                    }
                }
                else
                {
                    SkipIntroPlugin.Logger.LogWarning("PlayerData type not found—skipping profile/auto-host check.");
                }
            }

            // Always return true to run the original Awake for other setup
            return true;
        }
    }
}