using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CivilizedBoatExit
{
    [BepInPlugin(ID, title, version)]
    public class CivilizedBoatExit : BaseUnityPlugin
    {
        public const string ID = "mixone.valheim.civilizedboatexit";
        public const string version = "0.0.0.1";     
        public const string title = "Civilized Boat Exit";     

        public Harmony harmony;
        
        public static BepInEx.Logging.ManualLogSource harmonyLog;

        public void Awake()
        {
            harmony = new Harmony(ID);
            harmony.PatchAll();
            harmonyLog = Logger;

            harmonyLog.LogDebug($"{title} loaded.");
        }
    }

    #region Utils

    #endregion

    #region Transpilers

    #endregion

    #region Patches 

    #region Ladder

    [HarmonyPatch(typeof(Ladder), nameof(Ladder.Interact))]
    public static class Ladder_Interact_Patch
    {
        public static bool Prefix(ref Ladder __instance, ref bool __result, ref Humanoid character, ref bool hold)
        {
            if (hold)
            {
                __result = false;
                return false;
            }
            if (!__instance.InUseDistance(character))
            {
                __result = false;
                return false;
            }
            bool goingUp = __instance.m_targetPos.position.y + 1f > character.transform.position.y &&
                           __instance.m_targetPos.position.y - 1f > character.transform.position.y;
            if (goingUp) {
                CivilizedBoatExit.harmonyLog.LogDebug("Going up");
                character.transform.position = __instance.m_targetPos.position;
                character.transform.rotation = __instance.m_targetPos.rotation;
                character.SetLookDir(__instance.m_targetPos.forward);
            } else
            {
                CivilizedBoatExit.harmonyLog.LogDebug("Going down");
                character.transform.position = __instance.m_targetPos.position - new Vector3(1f, 2f, 0f);
                character.transform.rotation = __instance.m_targetPos.rotation;
                character.SetLookDir(__instance.m_targetPos.forward);
            }
            __result = false;
            return false;
        }
    }

    #endregion

    #endregion
}
