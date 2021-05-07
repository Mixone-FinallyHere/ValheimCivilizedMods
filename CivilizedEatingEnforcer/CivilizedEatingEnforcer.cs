using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CivilizedEatingEnforcer
{
    [BepInPlugin(ID, title, version)]
    public class CivilizedEatingEnforcer : BaseUnityPlugin
    {
        public const string ID = "mixone.valheim.civilizedeatingenforcer";
        public const string version = "0.0.0.1";     
        public const string title = "Civilized Eating Enforcer";     

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

    #region Player

    [HarmonyPatch(typeof(Player), nameof(Player.CanEat))]
    public static class PieceTable_NextCategory
    {
        public static bool Prefix(ref Player __instance, ref bool __result, ref ItemDrop.ItemData item, ref bool showMessages)
        {
            if (__instance.m_attached &&
                __instance.m_attachPoint.gameObject.GetComponentInParent<Chair>() != null)
            {
                if (Ship.GetLocalShip() != null)
                {
                    return true;
                }
                else
                {
                    int layerMask = __instance.m_placeRayMask;
                    RaycastHit raycastHit;

                    if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward,
                                            out raycastHit, 50f, layerMask) &&
                                        raycastHit.collider && !raycastHit.collider.attachedRigidbody &&
                                        Vector3.Distance(__instance.m_eye.position, raycastHit.point) < __instance.m_maxPlaceDistance)
                    {
                        Piece thePiece = raycastHit.collider.GetComponentInParent<Piece>();
                        if (thePiece != null)
                        {
                            if (thePiece.m_name.ToLower().Contains("table"))
                            {
                                return true;
                            }
                            else
                            {
                                __instance.Message(MessageHud.MessageType.Center,
                                    $"{__instance.GetPlayerName()}, a {thePiece.m_name} is not a table!\nSuch uncouth behaviour!",
                                    0, null);
                                __result = false;
                                return false;
                            }
                        }
                    }
                    __instance.Message(MessageHud.MessageType.Center,
                    $"{__instance.GetPlayerName()}, and where exactly shall you place your food?\nFind a table, do not be a brute.",
                    0, null);
                    __result = false;
                    return false;
                }

            }
            __instance.Message(MessageHud.MessageType.Center,
                $"{__instance.GetPlayerName()}, hast thou then become an animal?\nPlease take ah pew to eat! Goodness me!",
                0, null);
            __result = false;
            return false;
        }

        public static void Postfix(ref Player __instance, ref bool __result, ref ItemDrop.ItemData item, ref bool showMessages)
        {
            if (__result == true)
            {
                string message = $"An explendidly jolly good idea my good sir {__instance.GetPlayerName()}!\n";
                if (Ship.GetLocalShip() != null)
                {
                    message += $"Eating this {item.m_shared.m_name} on your {Ship.GetLocalShip().name.Replace("(Clone)", "")}\nwas a jolly good idea!";
                }
                else
                {
                    message += $"Taking a seat to eat this {item.m_shared.m_name} was a riveting idea!";
                }
                __instance.Message(MessageHud.MessageType.Center,
                message,
                0, null);
            }
        }
    }

    #endregion

    #endregion
}
