﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace CombatExtended
{
    public class WorkGiver_ReloadTurret : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial); ;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_TurretGunCE turret = t as Building_TurretGunCE;
            if (!forced && (turret == null || !turret.AllowAutomaticReload)) return false;
            
            if (turret == null
                || !turret.NeedsReload
                || !pawn.CanReserveAndReach(turret, PathEndMode.ClosestTouch, Danger.Deadly)
                || turret.IsForbidden(pawn.Faction))
            {
                return false;
            }
            Thing ammo = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                            ThingRequest.ForDef(turret.CompAmmo.selectedAmmo),
                            PathEndMode.ClosestTouch,
                            TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn),
                            80,
                            x => !x.IsForbidden(pawn) && pawn.CanReserve(x));
            return ammo != null;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_TurretGunCE turret = t as Building_TurretGunCE;
            if (turret == null || turret.CompAmmo == null) return null;

            if (!turret.CompAmmo.useAmmo)
            {
                //return new Job(DefDatabase<JobDef>.GetNamed("ReloadTurret"), t, null);
                return null;
            }

            Thing ammo = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                            ThingRequest.ForDef(turret.CompAmmo.selectedAmmo),
                            PathEndMode.ClosestTouch,
                            TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn),
                            80,
                            x => !x.IsForbidden(pawn) && pawn.CanReserve(x));

            if (ammo == null) return null;
            int amountNeeded = turret.CompAmmo.Props.magazineSize;
            if (turret.CompAmmo.currentAmmo == turret.CompAmmo.selectedAmmo) amountNeeded -= turret.CompAmmo.curMagCount;
            return new Job(DefDatabase<JobDef>.GetNamed("ReloadTurret"), t, ammo) { count = Mathf.Min(amountNeeded, ammo.stackCount) };
        }


    }
}