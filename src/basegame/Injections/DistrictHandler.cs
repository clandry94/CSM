using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;
using CSM.BaseGame.Commands.Data.Parks;
using Epic.OnlineServices.Presence;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SimulationStepImpl")]
    public class DistrictParkSimulationStep
    {
        public static void Prefix(DistrictManager __instance, out DistrictPark[] __state)
        {
            __state = new DistrictPark[__instance.m_parks.m_size];
            __instance.m_parks.m_buffer.CopyTo(__state, 0);
            if (IgnoreHelper.Instance.IsIgnored())
            {
                Log.Warn("Ignoring in prefix of park!");
            }
        }

        public static void Postfix(DistrictManager __instance, ref DistrictPark[] __state)
        {
            if (IgnoreHelper.Instance.IsIgnored() || Command.CurrentRole == MultiplayerRole.Client)
            {
                Log.Warn("Ignoring in postfix of park!");
                return;
            }

            var currentParks = __instance.m_parks.m_buffer;

            // ID index referenced simulation frame capped within 0-255
            var cappedIndex = (int)Singleton<SimulationManager>.instance.m_currentFrameIndex & (int)byte.MaxValue;

            // Frame id changed from 16 bit to 8 bit (?). This is the "first" parkID
            var parkID = cappedIndex * 128 >> 8;

            // This is the maximum park ID to consider for this frame
            var maxParkID = (cappedIndex + 1) * 128 >> 8;

            for (var targetParkID = parkID; parkID < maxParkID; ++parkID)
            {
                var oldPark = new SerializableDistrictPark(__state[(byte)targetParkID]);
                var newPark = new SerializableDistrictPark(__instance.m_parks.m_buffer[targetParkID]);

                if (!newPark.Equals(oldPark))
                {
                    Log.Info("[DistrictManager] Update found for district park");
                    Log.Info(oldPark.stringSerialize());
                    Log.Info(newPark.stringSerialize());

                    Command.SendToAll(new DistrictParkUpdateCommand
                    {
                        ParkID = (byte) targetParkID,
                        Park = newPark
                    });
                }
            }
        }
        
    }

    //
    // [HarmonyPatch(typeof(DistrictManager))]
    // [HarmonyPatch("SimulationStepImpl")]
    // public class DistrictParkSimulationStep
    // {
    //
    //     public static void Prefix(DistrictManager __instance, out DistrictPark[] __state)
    //     {
    //         __state = new DistrictPark[__instance.m_parks.m_size];
    //         __instance.m_parks.m_buffer.CopyTo(__state, 0);
    //         if (IgnoreHelper.Instance.IsIgnored())
    //         {
    //             Log.Warn("Ignoring in prefix of park!");
    //         }
    //     }
    //
    //
    //     public static void Postfix(DistrictManager __instance, ref DistrictPark[] __state)
    //     {
    //         if (IgnoreHelper.Instance.IsIgnored() || Command.CurrentRole == MultiplayerRole.Client)
    //         {
    //             Log.Warn("Ignoring in postfix of park!");
    //             return;
    //         }
    //         
    //         var currentParks = __instance.m_parks.m_buffer;
    //         
    //         // ID index referenced simulation frame capped within 0-255
    //         var cappedIndex = (int) Singleton<SimulationManager>.instance.m_currentFrameIndex & (int) byte.MaxValue;
    //         
    //         // Frame id changed from 16 bit to 8 bit (?). This is the "first" parkID
    //         var parkID  = cappedIndex * 128 >> 8;
    //         
    //         // This is the maximum park ID to consider for this frame
    //         var maxParkID = (cappedIndex + 1) * 128 >> 8;
    //
    //         for (var targetParkID = parkID; parkID < maxParkID; ++parkID)
    //         {
    //             DoPostfix(targetParkID, __instance.m_parks.m_buffer[targetParkID], __state[(byte)targetParkID]);
    //         }
    //     }
    //
    //     public static void PrefixParks(DistrictManager __instance, out DistrictPark[] __state)
    //     {
    //         __state = new DistrictPark[__instance.m_parks.m_size];
    //         __instance.m_parks.m_buffer.CopyTo(__state, 0);
    //         if (IgnoreHelper.Instance.IsIgnored())
    //         {
    //             Log.Warn("Ignoring in prefix of park!");
    //         }
    //     }
    //
    //     private static void DoPostfix(int parkID, DistrictPark newInstance, DistrictPark oldInstance)
    //     {
    //         if (IgnoreHelper.Instance.IsIgnored() || Command.CurrentRole == MultiplayerRole.Client)
    //         {
    //             Log.Warn("Ignoring in postfix for district park!");
    //             return;
    //         }
    //         
    //         switch (oldInstance.m_parkType)
    //         {
    //             case DistrictPark.ParkType.None:
    //                 Log.Info("None not implemented.");
    //                 break;
    //             case DistrictPark.ParkType.Generic:
    //                 DoGenericParkPostfix(parkID, newInstance, oldInstance);
    //                 break;
    //             case DistrictPark.ParkType.AmusementPark:
    //             case DistrictPark.ParkType.Zoo:
    //             case DistrictPark.ParkType.NatureReserve:
    //                 Log.Info("AmusementPark, Zoo, NatureReserve not implemented.");
    //
    //                 break;
    //             case DistrictPark.ParkType.Industry:
    //             case DistrictPark.ParkType.Farming:
    //             case DistrictPark.ParkType.Forestry:
    //             case DistrictPark.ParkType.Ore:
    //             case DistrictPark.ParkType.Oil:
    //                 Log.Info("Industry, Farming, Forestry, Ore, Oil not implemented.");
    //                 break;
    //             case DistrictPark.ParkType.GenericCampus:
    //             case DistrictPark.ParkType.TradeSchool:
    //             case DistrictPark.ParkType.LiberalArts:
    //             case DistrictPark.ParkType.University:
    //                 Log.Info("GenericCampus, TradeSchool, LiberalArts, University not implemented.");
    //
    //                 break;
    //             case DistrictPark.ParkType.Airport:
    //                 Log.Info("Airports not implemented.");
    //                 break;
    //         }
    //     }
    //     
    //     private static void DoGenericParkPostfix(int parkID, DistrictPark newInstance, DistrictPark oldInstance)
    //     {
    //         SimulationManager instance1 = Singleton<SimulationManager>.instance;
    //         
    //         
    //         if ((newInstance.m_flags & DistrictPark.Flags.Created) != DistrictPark.Flags.None)
    //         {
    //
    //             var parkState = new GenericParkState((byte) parkID, oldInstance);
    //                 
    //             if (parkState.HasChanged(newInstance))
    //             {
    //                 Command.SendToAll(new ParkSimulationCommand
    //                 {
    //                     TargetParkId = (byte) parkID,
    //                     TargetEntertainmentAccumulation = newInstance.m_finalEntertainmentAccumulation,
    //                     TargetAttractivenessAccumulation = newInstance.m_finalAttractivenessAccumulation
    //                 });
    //             }
    //
    //         }
    //     }
    //     
    //     
    //     
    //     public class State
    //     {
    //         public byte ParkId;
    //      
    //         public State(byte parkId, DistrictPark instance)
    //         {
    //             ParkId = parkId;
    //         }
    //
    //         // check if weather properties have been changed since last update
    //         public bool HasChanged(DistrictPark instance)
    //         {
    //             return true;
    //         }
    //     }
    //     
    //     public class GenericParkState
    //     {
    //         public byte ParkId;
    //         public ushort TargetEntertainmentAccumulation;
    //         public ushort TargetAttractivenessAccumulation;
    //
    //         public GenericParkState(byte parkId, DistrictPark instance)
    //         {
    //             ParkId = parkId;
    //             TargetEntertainmentAccumulation = instance.m_finalEntertainmentAccumulation;
    //             TargetAttractivenessAccumulation = instance.m_finalAttractivenessAccumulation;
    //         }
    //
    //         public bool HasChanged(DistrictPark instance)
    //         {
    //             Log.Info($"[DistrictHandler] Diff found for {ParkId}, syncing" +
    //                      $"original entertain={TargetEntertainmentAccumulation} attract={TargetAttractivenessAccumulation}" +
    //                      $"new entertain={instance.m_finalEntertainmentAccumulation} attract={instance.m_finalAttractivenessAccumulation}");
    //             
    //             return TargetEntertainmentAccumulation != instance.m_finalEntertainmentAccumulation ||
    //                    TargetAttractivenessAccumulation != instance.m_finalAttractivenessAccumulation;
    //         }
    //     }
    // }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (__result)
            {
                ulong seed = DistrictManager.instance.m_districts.m_buffer[district].m_randomSeed;

                Command.SendToAll(new DistrictCreateCommand
                {
                    DistrictId = district,
                    Seed = seed
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictTool))]
    [HarmonyPatch("ApplyBrush")]
    [HarmonyPatch(new Type[]
        { typeof(DistrictTool.Layer), typeof(byte), typeof(float), typeof(Vector3), typeof(Vector3) })]
    public class ApplyBrush
    {
        public static void Postfix(DistrictTool.Layer layer, byte district, float brushRadius, Vector3 startPosition,
            Vector3 endPosition)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictAreaModifyCommand
                {
                    Layer = layer,
                    District = district,
                    BrushRadius = brushRadius,
                    StartPosition = startPosition,
                    EndPosition = endPosition
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("ReleaseDistrict")]
    public class ReleaseDistrict
    {
        public static void Postfix(byte district)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictReleaseCommand
                {
                    DistrictId = district,
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetDistrictPolicy")]
    public class SetDistrictPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy, byte district)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyCommand
                {
                    Policy = policy,
                    DistrictId = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetCityPolicy")]
    public class SetCityPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictCityPolicyCommand
                {
                    Policy = policy
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("UnsetDistrictPolicy")]
    public class UnsetDistrictPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy, byte district)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyUnsetCommand
                {
                    Policy = policy,
                    DistrictId = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("UnsetCityPolicy")]
    public class UnsetCityPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictCityPolicyUnsetCommand
                {
                    Policy = policy,
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreatePark")]
    public class CreatePark
    {
        public static void Postfix(bool __result, ref byte park, DistrictPark.ParkType type,
            DistrictPark.ParkLevel level)
        {
            if (__result && !IgnoreHelper.Instance.IsIgnored())
            {
                ulong seed = DistrictManager.instance.m_parks.m_buffer[park].m_randomSeed;

                Command.SendToAll(new ParkCreateCommand
                {
                    ParkId = park,
                    ParkType = type,
                    ParkLevel = level,
                    Seed = seed
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("ReleasePark")]
    public class ReleasePark
    {
        public static void Prefix(ref byte park)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new ParkReleaseCommand
                {
                    ParkId = park
                });
            }
        }
    }
}