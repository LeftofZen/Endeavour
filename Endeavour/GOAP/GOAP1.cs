using System;

namespace ConsoleApp1.GOAP
{
	delegate void DepositWoodDel(WorldObject agent, WorldObject target);

	static class GOAP1
	{
		static void GoapMain(string[] args)
		{
			// setup objects
			var woWood = new WorldObject("wood");
			var woAxe = new WorldObject("axe");
			var woBed = new WorldObject("bed");
			var woWoodDeposit = new Agent("wood deposit");

			var Alice = new Agent("Alice");
			// set up actions

			// chop wood requires an axe, no wood
			var ChopWood = new GOAPAction("Chop wood", 3);
			var reqAxe = new GOAPState<WorldObject>("requires Axe", woAxe,
				(WorldObject wo, Agent a1) => { return a1.HasWorldObject(wo); });
			var reqNoWood = new GOAPState<WorldObject>("requires no Wood", woWood,
				(WorldObject wo, Agent a1) => { return !a1.HasWorldObject(wo); });
			ChopWood.AddPrerequisite(reqAxe);
			ChopWood.AddPrerequisite(reqNoWood);

			var giveWood = new GOAPState<WorldObject>("give wood to agent", woWood,
				(WorldObject wo, Agent a1) => { _ = a1.AddObjectToInventory(wo); return true; });

			ChopWood.AddEffect(giveWood);

			// done with chop wood

			// deposit wood

			var DepositWood = new GOAPAction("Deposit wood", 2);
			var reqWood = new GOAPState<WorldObject>("requires Wood", woWood,
				(WorldObject wo, Agent a1) => { return a1.HasWorldObject(wo); });
			DepositWood.AddPrerequisite(reqWood);

			var depWood = new GOAPState<WorldObject, Agent>("give wood to depo", woWood, woWoodDeposit,
				(WorldObject wo, Agent a1, Agent a2) =>
				{
					_ = a2.RemoveObjectFromInventory(wo);
					_ = a1.AddObjectToInventory(wo);
					return true;
				});

			DepositWood.AddEffect(depWood);

			// done with deposit wood

			var Sleep = new GOAPAction("Sleep", -20);
			
			var reqSleep = new GOAPState("requires sleep",
				(Agent a1) => { return a1.mEnergy < 3; });

			var replenishEnergy = new GOAPState<WorldObject>("sleep", woBed,
				(WorldObject wo, Agent a1) => { /*a1.mEnergy += 20; */return true; });

			Sleep.AddPrerequisite(reqSleep);
			Sleep.AddEffect(replenishEnergy);
			// rest

			// done with rest

			Alice.AddAction(ChopWood);
			Alice.AddAction(DepositWood);
			Alice.AddAction(Sleep);

			Console.WriteLine("\n=== Simulation running ===\n");

			// run simulation
			while (Alice.RunPlanner())
			{
				Console.WriteLine("{0}", Alice.ToString());
				Alice.PrintInventory();
				Console.WriteLine();
			}


			_ = Console.ReadLine();
		}
	}
}
