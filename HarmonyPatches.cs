using HarmonyLib;
using UnityEngine;

namespace UltraShock;

[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt))]
public class HurtPatch
{
    static void Postfix(int damage, bool invincible, float scoreLossMultiplier, bool explosion, bool instablack, float hardDamageMultiplier, bool ignoreInvincibility) {
        UltraShockPlugin.Logger.LogInfo("Damage" + damage +"!");
        float scale = UltraShockPlugin.ShockScale.Value / 100f;
        UltraShockPlugin.Shocker.EnqueueShock(Mathf.RoundToInt(damage * scale), 500);
    }
}
