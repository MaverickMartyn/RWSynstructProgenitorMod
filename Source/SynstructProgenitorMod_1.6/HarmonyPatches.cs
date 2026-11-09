using HarmonyLib;
using LudeonTK;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace SynstructProgenitorMod
{
    /// <summary>
    /// "More useful than the Harmony wiki" ~ Mehni
    /// </summary>

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            SynstructProgenitorHarmony harmony = new SynstructProgenitorHarmony("rimworld.maverickmartyn.synstruct_progenitor.main");
            
            //harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryLovinChanceFactor)), postfix: new HarmonyMethod(patchType, nameof(SecondaryLovinChanceFactorPostfix)),
            //              transpiler: new HarmonyMethod(patchType, nameof(SecondaryLovinChanceFactorTranspiler)));
            //harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.CompatibilityWith)),
            //    postfix: new HarmonyMethod(patchType, nameof(CompatibilityWithPostfix))); // Used in HAR, for checking compatibility between pawns of different races. This is used for romance and lovin' interactions.

            //harmony.Patch(AccessTools.Method(typeof(Designator), nameof(Designator.CanDesignateThing)),
            //    postfix: new HarmonyMethod(patchType, nameof(CanDesignateThingTamePostfix))); // For animal taming

            //harmony.Patch(AccessTools.Method(typeof(WorkGiver_InteractAnimal), "CanInteractWithAnimal", new[] { typeof(Pawn), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
            //    postfix: new HarmonyMethod(patchType, nameof(CanInteractWithAnimalPostfix))); // For animal interaction

            //harmony.Patch(AccessTools.Method(typeof(PreceptComp_SelfTookMemoryThought), nameof(PreceptComp_SelfTookMemoryThought.Notify_MemberTookAction)),
            //              transpiler: new HarmonyMethod(patchType, nameof(SelfTookMemoryThoughtTranspiler))); // For actions that are self-taken, like butchering a corpse.

            //harmony.Patch(AccessTools.Method(typeof(PreceptComp_KnowsMemoryThought), nameof(PreceptComp_KnowsMemoryThought.Notify_MemberWitnessedAction)),
            //              transpiler: new HarmonyMethod(patchType, nameof(KnowsMemoryThoughtTranspiler))); // For actions that are witnessed, like butchering a corpse.

            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker.Interacted)),
                          transpiler: new HarmonyMethod(patchType, nameof(RomanceAttemptInteractTranspiler)));

            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.SuccessChance)),
                          postfix: new HarmonyMethod(patchType, nameof(RomanceAttemptSuccessChancePostfix)));

            harmony.Patch(AccessTools.Method(typeof(BedUtility), nameof(BedUtility.WillingToShareBed)), postfix: new HarmonyMethod(patchType, nameof(WillingToShareBedPostfix)));

            harmony.Patch(AccessTools.Method(typeof(Tradeable_Pawn), nameof(Tradeable_Pawn.ResolveTrade)), transpiler: new HarmonyMethod(patchType, nameof(TradeablePawnResolveTranspiler)));

            harmony.Patch(AccessTools.Method(typeof(TradeUI), nameof(TradeUI.DrawTradeableRow)), transpiler: new HarmonyMethod(patchType, nameof(DrawTradeableRowTranspiler)));

            // Patch vanilla slavery thought workers to avoid mood effects for allowed slavery of artificial or biological beings
            //Log.Message("Patching ShouldHaveThought for ThoughtWorker_Precept_SlavesInColony");
            harmony.Patch(AccessTools.Method(typeof(ThoughtWorker_Precept_SlavesInColony), "ShouldHaveThought"), transpiler: new HarmonyMethod(patchType, nameof(TranspilerSlaveryPatch)));
            // And the Thought_Situational implementations.
            //Log.Message("Patching MoodOffset for Thought_Situational_Precept_SlavesInColony");
            harmony.Patch(AccessTools.Method(typeof(Thought_Situational_Precept_SlavesInColony), nameof(Thought_Situational_Precept_SlavesInColony.MoodOffset)), transpiler: new HarmonyMethod(patchType, nameof(TranspilerSlaveryPatch)));

            Log.Message($"Synstruct Progenitor successfully completed {harmony.PatchReport} with harmony.");
        }

        private static IEnumerable<CodeInstruction> TranspilerSlaveryPatch(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            MethodInfo pawnFactionProperty = AccessTools.PropertyGetter(typeof(Thing), nameof(Pawn.Faction));
            MethodInfo getSlavesInFactionCount = AccessTools.Method(typeof(FactionUtility), nameof(FactionUtility.GetSlavesInFactionCount));
            MethodInfo getDisapprovedSlaveCount = AccessTools.Method(typeof(ExtendedFactionUtility), nameof(ExtendedFactionUtility.GetDisapprovedSlavesInFactionCount));
            //Log.Message($"Faction getter: {pawnFactionProperty}");
            //Log.Message($"Faction getter declaring type: {pawnFactionProperty?.DeclaringType}");
            //Log.Message($"Faction count method: {getSlavesInFactionCount}");
            int matches = 0;
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (instructionList[i].opcode == OpCodes.Call || instructionList[i].opcode == OpCodes.Callvirt)
                {
                    //Log.Message($"Instruction: {instructionList[i].operand}" +
                    //$"Faction getter match: {instructionList[i].Calls(pawnFactionProperty)}, " +
                    //$"Slave count match: {instructionList[i].Calls(getSlavesInFactionCount)}");
        }
                // Replace FactionUtility.GetSlavesInFactionCount with ExtendedFactionUtility.GetDisapprovedSlavesInFactionCount, which checks for biological or artificial beings depending on the pawn's precept.
                if (instructionList[i].Calls(pawnFactionProperty) && i + 1 < instructionList.Count && instructionList[i + 1].Calls(getSlavesInFactionCount))
                {
                    yield return new CodeInstruction(OpCodes.Call, getDisapprovedSlaveCount);
                    i++; // Skip the next instruction too.
                    matches++;
                }
                else
                {
                    yield return instructionList[i];
                }
            }
            if (matches < 1)
            {
                Log.Error("Failed to patch vanila slave precept logic.");
            }
            //Log.Message($"SP: Transpiled {matches} instances of slavery thought worker.");
        }

        //private static IEnumerable<CodeInstruction> TranspilerSlaveryMoodOffset(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        //{
        //    List<CodeInstruction> instructionList = instructions.ToList();
        //    for (int i = 0; i < instructionList.Count; i++)
        //    {
        //        yield return instructionList[i];
        //    }
        //}

        public static IEnumerable<CodeInstruction> DrawTradeableRowTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo doerWillingInfo = AccessTools.Method(typeof(IdeoUtility), nameof(IdeoUtility.DoerWillingToDo), new Type[] { typeof(HistoryEvent) });

            List<CodeInstruction> instructionList = instructions.ToList();

            Label skip = ilg.DefineLabel();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                yield return instruction;

                if (i > 5 && instruction.Calls(doerWillingInfo))
                {
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Brtrue, skip);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TradeSession), nameof(TradeSession.playerNegotiator)));
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return CodeInstruction.Call(patchType, nameof(DrawTransferableRowIsWilling));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ceq);
                    yield return new CodeInstruction(OpCodes.Nop) { labels = new List<Label>() { skip } };
                }
            }
        }

        public static bool DrawTransferableRowIsWilling(Pawn doer, Tradeable trad)
        {
            if (trad is Tradeable_Pawn && trad.AnyThing is Pawn)
            {
                if (SynstructProgenitorScenarioMod.IsArtificial(trad.AnyThing as Pawn) && IdeoUtility.DoerWillingToDo(ArtificialBeingsDefOf.ABF_Artificial_SoldSlave, doer))
                {
                    return true;
                }
                else if (SynstructProgenitorScenarioMod.IsBiological(trad.AnyThing as Pawn) && IdeoUtility.DoerWillingToDo(BiologicalBeingsDefOf.ABF_Biological_SoldSlave, doer))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<CodeInstruction> TradeablePawnResolveTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo sellingSlaveryInfo = AccessTools.Method(typeof(GuestUtility), nameof(GuestUtility.IsSellingToSlavery));
            MethodInfo buyingSlaveryInfo = AccessTools.Method(typeof(ITrader), nameof(ITrader.GiveSoldThingToPlayer));

            Label orbitalTradeLabel = ilg.DefineLabel();

            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(sellingSlaveryInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TradeSession), nameof(TradeSession.playerNegotiator)));
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return CodeInstruction.Call(typeof(List<>).MakeGenericType(typeof(Pawn)), "get_Item");
                    yield return CodeInstruction.Call(patchType, nameof(SoldSlave));
                }

                if (instruction.Calls(buyingSlaveryInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TradeSession), nameof(TradeSession.trader)));
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(Pawn));
                    yield return new CodeInstruction(OpCodes.Brfalse, orbitalTradeLabel);
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(TradeSession), nameof(TradeSession.trader)));
                    yield return new CodeInstruction(OpCodes.Castclass, typeof(Pawn));
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 2);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 3);
                    yield return CodeInstruction.Call(typeof(List<>).MakeGenericType(typeof(Pawn)), "get_Item");
                    yield return CodeInstruction.Call(patchType, nameof(SoldSlave));
                    yield return new CodeInstruction(OpCodes.Nop).WithLabels(orbitalTradeLabel);
                }
            }
        }

        public static void SoldSlave(Pawn pawn, Pawn slave)
        {
            if (ModsConfig.IdeologyActive)
            {
                if (SynstructProgenitorScenarioMod.IsArtificial(slave))
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(ArtificialBeingsDefOf.ABF_Artificial_SoldSlave, pawn.Named(HistoryEventArgsNames.Doer), slave.Named(HistoryEventArgsNames.Victim)));
                if (SynstructProgenitorScenarioMod.IsBiological(slave))
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(BiologicalBeingsDefOf.ABF_Biological_SoldSlave, pawn.Named(HistoryEventArgsNames.Doer), slave.Named(HistoryEventArgsNames.Victim)));
            }
        }

        public static void WillingToShareBedPostfix(Pawn pawn1, Pawn pawn2, ref bool __result)
        {
            if (SynstructProgenitorScenarioMod.IsAnyArtificial(pawn1, pawn2))
                if (!IdeoUtility.DoerWillingToDo(ArtificialBeingsDefOf.ABF_ArtificialDating_SharedBed, pawn1) || !IdeoUtility.DoerWillingToDo(ArtificialBeingsDefOf.ABF_ArtificialDating_SharedBed, pawn2))
                    __result = false;

            if (SynstructProgenitorScenarioMod.IsAnyBiological(pawn1, pawn2))
                if (!IdeoUtility.DoerWillingToDo(BiologicalBeingsDefOf.ABF_BiologicalDating_SharedBed, pawn1) || !IdeoUtility.DoerWillingToDo(BiologicalBeingsDefOf.ABF_BiologicalDating_SharedBed, pawn2))
                    __result = false;
        }

        public static void RomanceAttemptSuccessChancePostfix(Pawn initiator, Pawn recipient, ref float __result)
        {
            if (SynstructProgenitorScenarioMod.IsAnyArtificial(initiator, recipient))
                if (!IdeoUtility.DoerWillingToDo(ArtificialBeingsDefOf.ABF_ArtificialDating_BeginRomance, initiator) || !IdeoUtility.DoerWillingToDo(ArtificialBeingsDefOf.ABF_ArtificialDating_BeginRomance, recipient))
                    __result = -1f;

            if (SynstructProgenitorScenarioMod.IsAnyBiological(initiator, recipient))
                if (!IdeoUtility.DoerWillingToDo(BiologicalBeingsDefOf.ABF_BiologicalDating_BeginRomance, initiator) || !IdeoUtility.DoerWillingToDo(BiologicalBeingsDefOf.ABF_BiologicalDating_BeginRomance, recipient))
                    __result = -1f;
        }

        public static IEnumerable<CodeInstruction> RomanceAttemptInteractTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo becameLoverInfo = AccessTools.Field(typeof(TaleDefOf), nameof(TaleDefOf.BecameLover));
            
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.LoadsField(becameLoverInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return CodeInstruction.Call(patchType, nameof(NewLoverHelper));
                }

                yield return instruction;
            }
        }

        public static void NewLoverHelper(Pawn initiator, Pawn recipient)
        {
            if (ModsConfig.IdeologyActive)
            {
                if (SynstructProgenitorScenarioMod.IsAnyArtificial(initiator, recipient))
                {
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(ArtificialBeingsDefOf.ABF_ArtificialDating_Dating, initiator.Named(HistoryEventArgsNames.Doer), recipient.Named(HistoryEventArgsNames.Victim)));
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(ArtificialBeingsDefOf.ABF_ArtificialDating_Dating, recipient.Named(HistoryEventArgsNames.Doer), initiator.Named(HistoryEventArgsNames.Victim)));
                }
                if (SynstructProgenitorScenarioMod.IsAnyBiological(initiator, recipient))
                {
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(BiologicalBeingsDefOf.ABF_BiologicalDating_Dating, initiator.Named(HistoryEventArgsNames.Doer), recipient.Named(HistoryEventArgsNames.Victim)));
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(BiologicalBeingsDefOf.ABF_BiologicalDating_Dating, recipient.Named(HistoryEventArgsNames.Doer), initiator.Named(HistoryEventArgsNames.Victim)));
                }
            }
        }

        //public static IEnumerable<CodeInstruction> KnowsMemoryThoughtTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        //{
        //    FieldInfo thoughtInfo = AccessTools.Field(typeof(PreceptComp_Thought), nameof(PreceptComp_Thought.thought));
        //    LocalBuilder thoughtLocal = ilg.DeclareLocal(typeof(ThoughtDef));

        //    foreach (CodeInstruction instruction in instructions)
        //    {
        //        yield return instruction;

        //        if (instruction.opcode == OpCodes.Stloc_0)
        //        {
        //            yield return new CodeInstruction(OpCodes.Ldarg_0);
        //            yield return new CodeInstruction(OpCodes.Ldfld, thoughtInfo);
        //            yield return new CodeInstruction(OpCodes.Ldarg_0);
        //            yield return new CodeInstruction(OpCodes.Ldarg_1);
        //            yield return new CodeInstruction(OpCodes.Ldarg_2);
        //            yield return CodeInstruction.Call(patchType, nameof(KnowsGetHistoryEventThoughtDefReplacer));
        //            yield return new CodeInstruction(OpCodes.Stloc, thoughtLocal.LocalIndex);
        //        }

        //        if (instruction.LoadsField(thoughtInfo))
        //        {
        //            yield return new CodeInstruction(OpCodes.Pop);
        //            yield return new CodeInstruction(OpCodes.Ldloc, thoughtLocal.LocalIndex);
        //        }
        //    }
        //}

        //public static ThoughtDef KnowsGetHistoryEventThoughtDefReplacer(ThoughtDef thought, PreceptComp_KnowsMemoryThought comp, HistoryEvent ev, Precept precept)
        //{
        //    ThoughtDef result = thought;

        //    ev.args.TryGetArg(HistoryEventArgsNames.Doer, out Pawn doer);
        //    ev.args.TryGetArg(HistoryEventArgsNames.Victim, out Pawn victim);

        //    if (thought == ArtificialBeingsDefOf.KnowButcheredHumanlikeCorpse)
        //    {
        //        if (Utilities.DifferentRace(doer.def, victim.def) && ModsConfig.IdeologyActive)
        //            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(ArtificialBeingsDefOf.HAR_ButcheredArtificial, doer.Named(HistoryEventArgsNames.Doer), victim.Named(HistoryEventArgsNames.Victim)));

        //        if (doer.def is ThingDef_AlienRace alienPropsPawn)
        //            result = alienPropsPawn.alienRace.thoughtSettings.butcherThoughtSpecific
        //                                ?.FirstOrDefault(predicate: bt => bt.raceList?.Contains(victim.def) ?? false)?.knowThought ??
        //                     alienPropsPawn.alienRace.thoughtSettings.butcherThoughtGeneral.knowThought;
        //    }

        //    return result;
        //}

        //public static IEnumerable<CodeInstruction> SelfTookMemoryThoughtTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        //{
        //    FieldInfo thoughtInfo = AccessTools.Field(typeof(PreceptComp_Thought), nameof(PreceptComp_Thought.thought));
        //    LocalBuilder thoughtLocal = ilg.DeclareLocal(typeof(ThoughtDef));

        //    bool first = true;

        //    foreach (CodeInstruction instruction in instructions)
        //    {
        //        yield return instruction;
        //        if (instruction.LoadsField(thoughtInfo))
        //        {
        //            if (first)
        //            {
        //                first = false;
        //                yield return new CodeInstruction(OpCodes.Ldarg_0);
        //                yield return new CodeInstruction(OpCodes.Ldarg_1);
        //                yield return new CodeInstruction(OpCodes.Ldarg_2);
        //                yield return CodeInstruction.Call(patchType, nameof(SelfTookGetHistoryEventThoughtDefReplacer));
        //                yield return new CodeInstruction(OpCodes.Stloc, thoughtLocal.LocalIndex);
        //            }
        //            else
        //                yield return new CodeInstruction(OpCodes.Pop);

        //            yield return new CodeInstruction(OpCodes.Ldloc, thoughtLocal.LocalIndex);
        //        }
        //    }
        //}

        //public static ThoughtDef SelfTookGetHistoryEventThoughtDefReplacer(ThoughtDef thought, PreceptComp_SelfTookMemoryThought comp, HistoryEvent ev, Precept precept)
        //{
        //    ThoughtDef result = thought;

        //    Pawn doer = ev.args.GetArg<Pawn>(HistoryEventArgsNames.Doer);
        //    ev.args.TryGetArg(HistoryEventArgsNames.Victim, out Pawn victim);

        //    if (thought == ArtificialBeingsDefOf.ButcheredHumanlikeCorpse)
        //    {
        //        if (doer.def is ThingDef_AlienRace alienPropsButcher)
        //            result = alienPropsButcher.alienRace.thoughtSettings.butcherThoughtSpecific
        //                                   ?.FirstOrDefault(predicate: bt => bt.raceList?.Contains(victim.def) ?? false)?.thought ??
        //                     alienPropsButcher.alienRace.thoughtSettings.butcherThoughtGeneral.thought;
        //    }


        //    return result;
        //}

        //public static void CanInteractWithAnimalPostfix(ref bool __result, Pawn pawn, Pawn animal) =>
        //    __result = __result && RaceRestrictionSettings.CanTame(animal.def, pawn.def);

        //public static void CanDesignateThingTamePostfix(Designator __instance, ref AcceptanceReport __result, Thing t)
        //{
        //    if (!__result.Accepted || __instance is not Designator_Build)
        //        return;

        //    __result = colonistRaces.Any(predicate: td => RaceRestrictionSettings.CanTame(t.def, td));
        //}

        // ReSharper disable once RedundantAssignment
        //public static void CompatibilityWithPostfix(Pawn_RelationsTracker __instance, Pawn otherPawn, ref float __result, Pawn ___pawn)
        //{
        //    if (___pawn.RaceProps.Humanlike != otherPawn.RaceProps.Humanlike || ___pawn == otherPawn)
        //    {
        //        __result = 0f;
        //        return;
        //    }

        //    float x = Mathf.Abs(___pawn.ageTracker.AgeBiologicalYearsFloat - otherPawn.ageTracker.AgeBiologicalYearsFloat);
        //    float num = GenMath.LerpDouble(inFrom: 0f, inTo: 20f, outFrom: 0.45f, outTo: -0.45f, x);
        //    num = Mathf.Clamp(num, min: -0.45f, max: 0.45f);
        //    float num2 = __instance.ConstantPerPawnsPairCompatibilityOffset(otherPawn.thingIDNumber);
        //    __result = num + num2;
        //}

        //public static void SecondaryLovinChanceFactorPostfix(Pawn ___pawn, Pawn otherPawn, ref float __result)
        //{

        //}

        //public static IEnumerable<CodeInstruction> SecondaryLovinChanceFactorTranspiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    FieldInfo defField = AccessTools.Field(typeof(Thing), nameof(Pawn.def));
        //    MethodInfo racePropsProperty = AccessTools.Property(typeof(Pawn), nameof(Pawn.RaceProps)).GetGetMethod();
        //    MethodInfo humanlikeProperty = AccessTools.Property(typeof(RaceProperties), nameof(RaceProperties.Humanlike)).GetGetMethod();

        //    foreach (CodeInstruction instruction in instructions)
        //    {
        //        if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(defField))
        //        {
        //            yield return new CodeInstruction(OpCodes.Callvirt, racePropsProperty);

        //            instruction.opcode = OpCodes.Callvirt;
        //            instruction.operand = humanlikeProperty;
        //        }

        //        yield return instruction;
        //    }
        //}
    }
}