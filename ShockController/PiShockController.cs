// This file was originally part of PeakShock, Copyright (c) 2025 Addzeey.
// Available at https://github.com/addzeey/PeakShock
// Licensed under the MIT License.
// See https://opensource.org/licenses/MIT for details.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BepInEx.Logging;

#nullable enable

namespace ShockController
{
    public class PiShockController : IShockController
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly ShockRequestQueue _queue;
        private readonly ManualLogSource _logger;

        private readonly string _userName;
        private readonly string _shareCode;
        private readonly string _apiKey;
        private readonly float _cooldownSeconds;

        private DateTime _lastShockTime = DateTime.MinValue;
        private TimeSpan ShockCooldown => TimeSpan.FromSeconds(0.1 + Math.Max(0.0f, _cooldownSeconds));

        public PiShockController(string userName, string shareCode, string apiKey, float cooldownSeconds,
                ManualLogSource logger)
        {
            _userName = userName;
            _shareCode = shareCode;
            _apiKey = apiKey;

            _cooldownSeconds = cooldownSeconds;
            _logger = logger;
            _queue = new ShockRequestQueue(logger);
        }

        public void TriggerShock(int intensity, int duration_ms = 1, string? shareCode = null)
        {
            var now = DateTime.UtcNow;
            if (now - _lastShockTime < ShockCooldown)
            {
                _logger.LogInfo($"Shock skipped due to cooldown.");
                return;
            }
            _lastShockTime = now;
            var code = shareCode ?? _shareCode;
            _logger.LogInfo($"Enqueue shock: intensity={intensity}, duration={duration_ms}, code={code}");
            _queue.Enqueue(() => TriggerShockInternal(intensity, duration_ms, code));
        }

        public void EnqueueShock(int intensity, int duration_ms, string? code = null)
        {
            TriggerShock(intensity, duration_ms, code);
        }

        private async Task TriggerShockInternal(int intensity, int duration, string code)
        {
            var user = _userName;
            var key = _apiKey;
            duration /= 1000;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(code))
            {
                _logger.LogWarning($"Would send shock (intensity={intensity}, duration={duration}, code={code}), but PiShock credentials are not set.");
                return;
            }
            _logger.LogInfo($"Sending shock: intensity={intensity}, duration_sec={duration}, code={code}");

            var json = JsonConvert.SerializeObject(new
            {
                Username = user,
                APIKey = key,
                Code = code,
                Intensity = intensity,
                Duration = duration,
                Op = 0 // 0 = shock
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            { 
                _logger.LogInfo($"Sending request to PiShock API: {user}, Intensity={intensity}, Duration={duration}");
                var response = await _client.PostAsync("https://do.pishock.com/api/apioperate/", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"PiShock API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PiShock API exception: {ex}");
            }
        }
    }
}
