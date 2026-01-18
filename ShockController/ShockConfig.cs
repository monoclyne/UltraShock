using BepInEx.Configuration;

namespace ShockController;

public class ShockConfig {
    public ConfigEntry<string> OpenShockApiUrl;

    public ShockConfig(ConfigFile f) {
        OpenShockApiUrl = f.Bind("OpenShock", "ApiUrl", "https://api.openshock.app", "OpenShock API URL");
    }
}
