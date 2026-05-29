// This file was adapted from PeakShock, Copyright (c) 2025 Addzeey.
// Available at https://github.com/addzeey/PeakShock
// Licensed under the MIT License.
// See https://opensource.org/licenses/MIT for details.

using BepInEx.Configuration;

namespace ShockController;

public class ShockConfig {

    public ConfigFile f { get; private set; } = null!;

    public ConfigEntry<float> ShockCooldownSeconds { get; private set; } = null!;

    public enum ShockProvider { OpenShock, PiShock }
    public ConfigEntry<ShockProvider> ShockProviderType { get; private set; } = null!;

    public ConfigEntry<string> OpenShockApiUrl { get; private set; } = null!;
    public ConfigEntry<string> OpenShockDeviceId { get; private set; } = null!;
    public ConfigEntry<string> OpenShockApiKey { get; private set; } = null!;

    public ConfigEntry<string> PiShockUserName { get; private set; } = null!;
    public ConfigEntry<string> PiShockAPIKey { get; private set; } = null!;
    public ConfigEntry<string> PiShockShareCode { get; private set; } = null!;
    public ConfigEntry<string> PiShockShockerID { get; private set; } = null!;

    public ShockConfig(ConfigFile f) {

        ShockCooldownSeconds = f.Bind("Shock", "ShockCooldownSeconds", 2f, "Minimum seconds between shocks (prevents shock spam)");

        ShockProviderType = f.Bind("Shock", "Provider", ShockProvider.OpenShock, "Choose PiShock or OpenShock");

        OpenShockApiUrl = f.Bind("OpenShock", "ApiUrl", "https://api.openshock.app", "OpenShock API URL");
        OpenShockDeviceId = f.Bind("OpenShock", "DeviceId", "", "OpenShock Device ID");
        OpenShockApiKey = f.Bind("OpenShock", "ApiKey", "", "OpenShock API Key");

        PiShockUserName = f.Bind("PiShock", "UserName", "", "Your PiShock username");
        PiShockAPIKey = f.Bind("PiShock", "APIKey", "", "Your PiShock API Key");
        PiShockShareCode = f.Bind("PiShock", "ShareCode", "", "Your PiShock ShareCode");
        PiShockShockerID = f.Bind("PiShock", "ShockerID", "", "Your PiShock Shocker IDs. Comma separated list.");
    }
}
