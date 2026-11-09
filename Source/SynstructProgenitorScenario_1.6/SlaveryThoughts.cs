using AlienRace;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace SynstructProgenitorScenario
{
    public class ThoughtWorker_Precept_ArtificialSlavesInColony : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                        return true;
                }
                else if (map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && !p.HostileTo(c) && c.IsSlave))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_ArtificialSlavery_NoSlavesInColony : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (!lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (!car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (!map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && c.IsSlave))
                        return true;
                }
                else if (!map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && !p.HostileTo(c) && c.IsSlave))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_BiologicalSlavesInColony : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                        return true;
                }
                else if (map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && !p.HostileTo(c) && c.IsSlave))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_BiologicalSlavery_NoSlavesInColony : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (!lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (!car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (!map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && c.IsSlave))
                        return true;
                }
                else if (!map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && !p.HostileTo(c) && c.IsSlave))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
