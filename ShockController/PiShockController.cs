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
        private readonly string[] _shockerIDs;
        private readonly float _cooldownSeconds;

        private DateTime _lastShockTime = DateTime.MinValue;
        private TimeSpan ShockCooldown => TimeSpan.FromSeconds(0.1 + Math.Max(0.0f, _cooldownSeconds));

        public PiShockController(string userName, string shareCode, string apiKey, string shockerID, float cooldownSeconds,
                ManualLogSource logger)
        {
            _userName = userName;
            _shareCode = shareCode;
            _apiKey = apiKey;
            _shockerIDs = shockerID.Split(",");

            _cooldownSeconds = cooldownSeconds;
            _logger = logger;
            _queue = new ShockRequestQueue(logger);
            _queue.Enqueue(() => SetupSharecodeInternal(shareCode));
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

        private async Task SetupSharecodeInternal(string code)
        {
            _logger.LogInfo("Attempting setup of sharecode.");
            var json = JsonConvert.SerializeObject(new
            {
                Shares = new[] {
                    code
                }
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage shareRequest = new HttpRequestMessage(
                HttpMethod.Put,
                "https://api.pishock.com/Share"
            );

            shareRequest.Headers.Add("X-PiShock-API-Key", _apiKey);
            shareRequest.Headers.Add("X-PiShock-Username", _userName);

            shareRequest.Content = content;

            try
            {
                _logger.LogInfo($"Sending request to PiShock API: Share Code = {code}, Serialized Content = {content}");
                var response = await _client.SendAsync(shareRequest);
                _logger.LogInfo($"Recieved response {response}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"PiShock API exception: {ex}");
            }
        }

        private async Task TriggerShockInternal(int intensity, int duration, string code)
        {
            var user = _userName;
            var key = _apiKey;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(code))
            {
                _logger.LogWarning($"Would send shock (intensity={intensity}, duration={duration}, code={code}), but PiShock credentials are not set.");
                return;
            }
            _logger.LogInfo($"Sending shock: intensity={intensity}, duration={duration}");

            foreach (string shockerID in _shockerIDs)
            {

                var json = JsonConvert.SerializeObject(new
                {
                    Username = user,
                    APIKey = key,
                    Code = code,
                    Intensity = intensity,
                    Duration = duration,
                    Operation = 1 // 0 = shock, 1 = vibrate (testing)
                });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");


                HttpRequestMessage shockRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"https://api.pishock.com/Shockers/{shockerID}"
                );

                shockRequest.Headers.Add("X-PiShock-API-Key", _apiKey);
                shockRequest.Headers.Add("X-PiShock-Username", _userName);

                shockRequest.Content = content;

                try
                {
                    _logger.LogInfo($"Sending request to PiShock API: {user}, Intensity={intensity}, Duration={duration}, ID={_shockerID}");
                    var response = await _client.SendAsync(shockRequest);
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
}
