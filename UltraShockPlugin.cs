using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;
using HarmonyLib;
using ShockController;

namespace UltraShock;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class UltraShockPlugin : BaseUnityPlugin {

    public static new ManualLogSource Logger;
    public static UltraShockPlugin Instance;

    internal ShockConfig _shockConf;
    public static IShockController Shocker;

    public static ConfigEntry<int> ShockScale { get; private set; } = null!;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);

        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded :3");

        SceneManager.sceneLoaded += (_, _) =>
        {
            Logger.LogInfo("Scene loaded!");
        };

        var harmony = HarmonyLib.Harmony.CreateAndPatchAll(typeof(HurtPatch));

        ConfigFile f = new ConfigFile($"BepInEx/config/{PluginInfo.PLUGIN_GUID}.cfg", saveOnInit: true);
        ShockScale = f.Bind("Shock", "ShockScale", 50, "How much to scale shock intensity (0-100)");
        _shockConf = new ShockConfig(f);

        var provider = _shockConf.ShockProviderType.Value;
        Logger.LogInfo($"Shock provider is: {provider}");
        if (provider == ShockConfig.ShockProvider.OpenShock) {
            Shocker = new OpenShockController(
                apiUrl: _shockConf.OpenShockApiUrl.Value,
                deviceId: _shockConf.OpenShockDeviceId.Value,
                apiKey: _shockConf.OpenShockApiKey.Value,
                cooldownSeconds: _shockConf.ShockCooldownSeconds.Value,
                Logger
            );
        } else if (provider == ShockConfig.ShockProvider.PiShock) {
            Shocker = new PiShockController(
                userName: _shockConf.PiShockUserName.Value,
                shareCode: _shockConf.PiShockShareCode.Value,
                apiKey: _shockConf.PiShockAPIKey.Value,
                shockerID: _shockConf.PiShockShockerID.Value,
                opMode: _shockConf.PiShockOpMode.Value,
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
