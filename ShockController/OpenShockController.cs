// This file was originally part of PeakShock, Copyright (c) 2025 Addzeey.
// Available at https://github.com/addzeey/PeakShock
// Licensed under the MIT License.
// See https://opensource.org/licenses/MIT for details.

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;

#nullable enable
namespace ShockController;

public class OpenShockController : IShockController
{
    private readonly ShockRequestQueue _queue;
    private readonly string _apiUrl;
    private readonly string _deviceId;
    private readonly string _apiKey;
    private readonly float _cooldownSeconds;
    private readonly ManualLogSource _logger;
    private readonly HttpClient _client = new HttpClient();
    private DateTime _lastShockTime = DateTime.MinValue;

    private TimeSpan ShockCooldown => TimeSpan.FromSeconds(0.1 + Math.Max(0.0f, _cooldownSeconds));

    public OpenShockController(string apiUrl, string deviceId, string apiKey, float cooldownSeconds,
            ManualLogSource logger)
    {
        _apiUrl = apiUrl;
        _deviceId = deviceId;
        _apiKey = apiKey;
        _cooldownSeconds = cooldownSeconds;
        _logger = logger;
        _queue = new ShockRequestQueue(logger);
    }

    public void EnqueueShock(int intensity, int duration_ms, string? code = null)
    {
        var utcNow = DateTime.UtcNow;
        if (utcNow - _lastShockTime < ShockCooldown)
        {
            _logger.LogInfo("OpenShock shock skipped due to cooldown.");
            return;
        }
        _lastShockTime = utcNow;
        _queue.Enqueue(() => TriggerShockInternal(intensity, duration_ms, code));
    }

    private async Task TriggerShockInternal(int intensity, int duration_ms, string? code)
    {
        // Clamp duration to API limits
        duration_ms = Math.Clamp(duration_ms, 300, 65535);
        if (string.IsNullOrEmpty(_apiUrl) || string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning($"Would send OpenShock (intensity={intensity}, duration_ms={duration_ms}), but OpenShock credentials are not set.");
            return;
        }
        var id = !string.IsNullOrEmpty(code) ? code : _deviceId;
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogWarning("No deviceId or share code provided for OpenShock.");
            return;
        }
        _logger.LogInfo($"Sending OpenShock: id={id}, intensity={intensity}, duration_ms={duration_ms}");
        var data = new
        {
            Shocks = new[]
            {
                new {
                    Id = id,
                    Type = 1, // 1 = Shock
                    Intensity = intensity,
                    Duration = duration_ms
                }
            },
            CustomName = "Integrations.UltraShock"
        };
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + "/2/shockers/control")
        {
            Content = content
        };
        request.Headers.Add("OpenShockToken", _apiKey);
        request.Headers.Add("User-Agent", $"UnityShock");
        try
        {
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"OpenShock API error: {response.StatusCode} {errorContent}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"OpenShock API exception: {ex}");
        }
    }
}
