using System;
using System.Collections.Generic;

namespace ConsoleApp1.GOAP
{
	class GOAPAction
	{
		public GOAPAction(string name, int cost)
		{ mName = name; mCost = cost; }
		
		List<IGOAPState> mPrerequisites = new List<IGOAPState>();
		List<IGOAPState> mEffects = new List<IGOAPState>();
		public int mCost = 0;
		public Agent mAgent = null;
		public WorldObject mTarget = null;

		string mName;

		public void AddPrerequisite(IGOAPState state)
		{
			UpdateAgent(state.GetAgent());

			Console.WriteLine("[AddPrerequisite] \"{0}\"", state.GetName());
			mPrerequisites.Add(state);
		}

		public void AddEffect(IGOAPState state)
		{
			UpdateAgent(state.GetAgent());

			Console.WriteLine("[AddEffect] \"{0}\"", state.GetName());
			mEffects.Add(state);
		}

		public List<IGOAPState> GetEffects()
		{ return mEffects; }

		void UpdateAgent(Agent a)
		{
			mAgent = a;
		}

		public bool AreAllPrerequisitesSatisfied()
		{
			var result = true;
			foreach (var v in mPrerequisites)
			{
				// every prere needs an agent
				if (v.GetAgent() == null)
				{
					v.SetAgent(mAgent);
				}

				result &= v.Evaluate();
			}
			return result;
		}

		public override string ToString()
		{
			return mName;
		}
	}
}
