using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Verse;

namespace CombatExtended.Harmony
{
	[StaticConstructorOnStartup]
	static class Main
	{
		static Main()
		{
			var harmony = HarmonyInstance.Create("CombatExtended.Harmony.Pawn_HealthTracker");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
		
		[HarmonyPatch(typeof(Pawn_HealthTracker))]
		[HarmonyPatch("CheckForStateChange")]
		static class Disable_RNG_Death
		{
			public class RNGReplacer : CodeProcessor
			{
				int countReplace = 0;
				
				public override List<CodeInstruction> Process(CodeInstruction instruction)
				{
					if (instruction == null) return null;
					//Log.Message(string.Concat("Instruction: ", instruction.opcode.ToString(), " :: ", instruction.operand == null ? "null" : instruction.operand.ToString()));
					if (countReplace < 2 && instruction.opcode == OpCodes.Ldc_R4)
					{
						instruction.operand = 0.0f;
						countReplace++;
						//Log.Message(string.Concat("Changed to: ", instruction.opcode.ToString(), " :: ", instruction.operand == null ? "null" : instruction.operand.ToString()));
					}
					return new List<CodeInstruction> { instruction };
				}
			}
			
			[HarmonyProcessorFactory]
			static HarmonyProcessor Disable_RNG_Death_Patch(MethodBase original)
			{
				var processor = new HarmonyProcessor();
				processor.Add(new RNGReplacer());
				return processor;
			}
		}
	}
}