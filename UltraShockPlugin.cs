using BepInEx;
using BepInEx.Logging;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace UltraShock;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class UltraShockPlugin : BaseUnityPlugin {

    public static new ManualLogSource Logger;
    public static UltraShockPlugin Instance;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);

        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded :3");

        SceneManager.sceneLoaded += (_, _) =>
        {
            Logger.LogInfo("Scene loaded!");
        };

        var harmony = HarmonyLib.Harmony.CreateAndPatchAll(typeof(CameraPatch));
    }

    private void OnEnable() {
        Logger.LogInfo("Enabled");
    }

    private void OnDisable() {
        Logger.LogInfo("Disabled");
    }

    private void Update() {
        Logger.LogError("OWO");
    }
}
