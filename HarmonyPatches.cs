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

[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt))]
public class HurtPatch
{
    static void Postfix(int damage, bool invincible, float scoreLossMultiplier, bool explosion, bool instablack, float hardDamageMultiplier, bool ignoreInvincibility) {
        UltraShockPlugin.Logger.LogInfo("Damage" + damage +"!");
    }
}
