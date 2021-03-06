﻿using GiddyUpCore.Jobs;
using GiddyUpCore.Storage;
using GiddyUpRideAndRoll.Jobs;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace GiddyUpRideAndRoll.Harmony
{


    [HarmonyPatch(typeof(JobDriver_Mounted), "shouldCancelJob")]
    class JobDriver_Mounted_ShouldCancelJob
    {
        //TODO: maybe xml this instead of hard coding. 
        static JobDef[] allowedJobs = {GU_RR_JobDefOf.RideToJob, JobDefOf.Arrest, JobDefOf.AttackMelee, JobDefOf.AttackStatic, JobDefOf.Capture, JobDefOf.DropEquipment, JobDefOf.EscortPrisonerToBed, JobDefOf.ExtinguishSelf, JobDefOf.Flee, JobDefOf.FleeAndCower, JobDefOf.Goto, JobDefOf.GotoSafeTemperature, JobDefOf.GotoWander, JobDefOf.HaulToCell, JobDefOf.HaulToContainer, JobDefOf.Ignite, JobDefOf.Insult, JobDefOf.Kidnap, JobDefOf.Open, JobDefOf.RemoveApparel, JobDefOf.Rescue, JobDefOf.TakeWoundedPrisonerToBed, JobDefOf.TradeWithPawn, JobDefOf.UnloadInventory, JobDefOf.UseArtifact, JobDefOf.UseVerbOnThing, JobDefOf.Vomit, JobDefOf.Wait, JobDefOf.WaitCombat, JobDefOf.WaitMaintainPosture, JobDefOf.WaitSafeTemperature, JobDefOf.WaitWander, JobDefOf.Wear, JobDefOf.TakeInventory};
        static void Postfix(ExtendedPawnData riderData, JobDriver_Mounted __instance, ref bool __result)
        {
            if (__instance.pawn.Faction == Faction.OfPlayer && !__instance.Rider.Drafted && __instance.Rider.CurJob != null && !allowedJobs.Contains(__instance.Rider.CurJob.def))
            {
                if(__instance.Rider.CurJob.def == JobDefOf.Hunt && !Base.noMountedHunting)
                {
                    __result = false;
                }
                else
                {
                    __result = true;
                }
            }
        }
    }

   

    [HarmonyPatch(typeof(JobDriver_Mounted), "FinishAction")]
    class JobDriver_Mounted_FinishAction
    {
        static void Postfix(JobDriver_Mounted __instance)
        {
            ExtendedPawnData pawnData = Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(__instance.pawn);
            if(!__instance.Rider.Drafted && __instance.pawn.Faction == Faction.OfPlayer)
            {
                if (pawnData.ownedBy != null && !__instance.interrupted)
                {
                    //TODO: this job gets cancelled now, should make it more dominant.
                    __instance.pawn.jobs.jobQueue.EnqueueFirst(new Job(JobDefOf.Wait, 10000, true)); //wait a while before returning to camp, to give the rider the chance to ride back. Not needed when pawn is master.
                    //__instance.pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.Wait, 10000, true)); 
                }
            }
        }
    }

    
    

}
