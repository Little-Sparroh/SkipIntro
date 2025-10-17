using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MycopunkSkipIntro
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [MycoMod(null, ModFlags.IsClientSide)]
    public class SkipIntroPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "sparroh.skipintro";
        public const string PluginName = "SkipIntro";
        public const string PluginVersion = "1.0.1";

        private Harmony _harmony;
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            var harmony = new Harmony(PluginGUID);
            harmony.PatchAll(typeof(IntroPatches));
            Logger.LogInfo($"{PluginName} loaded successfully.");
        }
    }

    internal class IntroPatches
    {
        [HarmonyPatch(typeof(StartMenu), "Awake")]
        [HarmonyPrefix]
        private static bool SkipIntroPrefix(StartMenu __instance)
        {
            var staticTraverse = Traverse.Create(typeof(StartMenu));
            bool hasInitialized = staticTraverse.Field("hasInitialized").GetValue<bool>();

            if (!hasInitialized)
            {
                var instanceTraverse = Traverse.Create(__instance);

                staticTraverse.Field("hasInitialized").SetValue(true);

                Type gameManagerType = AccessTools.TypeByName("GameManager");
                if (gameManagerType != null)
                {
                    Traverse.Create(gameManagerType).Method("InitializeSettingRecursive", new Type[] { typeof(Transform) }).GetValue(instanceTraverse.Field("settingsWindow").GetValue<Window>().transform);
                }


                instanceTraverse.Field("initializeScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("startScreen").GetValue<RectTransform>().gameObject.SetActive(true);

                instanceTraverse.Field("bootingScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("splashScreen").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("initializeBar").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("logText").GetValue<TextMeshProUGUI>().gameObject.SetActive(false);
                instanceTraverse.Field("initializeStuff").GetValue<GameObject>().SetActive(false);
                instanceTraverse.Field("verifiedText").GetValue<TextMeshProUGUI>().gameObject.SetActive(false);
                instanceTraverse.Field("loadingBar").GetValue<Image>().gameObject.SetActive(false);

                RectTransform initializeWipe = instanceTraverse.Field("initializeWipe").GetValue<RectTransform>();
                initializeWipe.anchoredPosition = new Vector2(0f, -2160f);
                instanceTraverse.Field("wipeChild").GetValue<RectTransform>().anchoredPosition = new Vector2(0f, 2160f);
                initializeWipe.gameObject.SetActive(false);

                Type playerInputType = AccessTools.TypeByName("PlayerInput");
                if (playerInputType != null)
                {
                    Traverse.Create(playerInputType).Method("UnlockCursor").GetValue();
                }


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

            }
            return true;
        }
    }
}
