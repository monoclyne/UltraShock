using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace UltraShock;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class UltraShockPlugin : BaseUnityPlugin {

    public static new ManualLogSource Logger;
    public static UltraShockPlugin Instance;

    internal ShockController.ShockConfig _shockConf;
    public static ShockController.IShockController Shocker;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);

        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded :3");

        SceneManager.sceneLoaded += (_, _) =>
        {
            Logger.LogInfo("Scene loaded!");
        };

        var harmony = HarmonyLib.Harmony.CreateAndPatchAll(typeof(CameraPatch));
        harmony.PatchAll(typeof(HurtPatch));

        ConfigFile f = new ConfigFile($"BepInEx/config/{PluginInfo.PLUGIN_GUID}.cfg", saveOnInit: true);
        _shockConf = new ShockController.ShockConfig(f);

        Logger.LogInfo($"Shock provider is: {_shockConf.ShockProviderType.Value}");
        if (_shockConf.ShockProviderType.Value == ShockController.ShockConfig.ShockProvider.OpenShock) {
            Shocker = new ShockController.OpenShockController(
                apiUrl: _shockConf.OpenShockApiUrl.Value,
                deviceId: _shockConf.OpenShockDeviceId.Value,
                apiKey: _shockConf.OpenShockApiKey.Value,
                cooldownSeconds: _shockConf.ShockCooldownSeconds.Value,
                Logger
            );
        }
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
