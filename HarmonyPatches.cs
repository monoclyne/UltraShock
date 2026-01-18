using HarmonyLib;
// using UnityEngine.UI;

namespace UltraShock;

[HarmonyPatch(typeof(CameraController), nameof(CameraController.CameraShake))]
public class CameraPatch
{
    static void Postfix(float shakeAmount)
    {
        UltraShockPlugin.Logger.LogInfo("SHAKE!");
    }
}
