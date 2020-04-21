using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Endeavour
{
	interface IWorldObject
	{
		void ApplyAction(Func<IWorldObject, bool> func);
	}

	class WorldObject : IWorldObject
	{
		public void ApplyAction(Func<IWorldObject, bool> func)
		{
			//func.Invoke();
		}
	}

	interface IHas { };

	interface IHasInventory : IHas
	{
		void AddToInventory(IWorldObject wo);
		void RemoveFromInventory(IWorldObject wo);
		bool HasInInventory(IWorldObject wo);
	}

	class Action
	{
		public List<System.Action> mPrerequisites = new List<System.Action>();
		public List<System.Action> mEffect = new List<System.Action>();
		//public IWorldObject mTarget;
		public int mCost;
		public Agent mOwner;


		public bool AreAllPrerequisitesSatisfied()
		{
			var result = true;
			foreach (var v in mPrerequisites)
			{
				result &= (bool)v.DynamicInvoke();
			}
			return result;
		}
	}

	class Agent : IWorldObject, IHasInventory
	{
		public Agent()
		{
		}


		public void AddAction(Action a)
		{
			a.mOwner = this;
			mActions.Add(a);
		}

		Dictionary<IWorldObject, bool> mInventory = new Dictionary<IWorldObject, bool>();
		List<Action> mActions = new List<Action>(); // should be prio queue
		public int mEnergy = 20;

		public bool Run()
		{
			if (mActions.Count() == 0)
			{
				return false;
			}

			// get lowest energy, valid task
			var min = mActions[0];
			foreach (var a in mActions)
			{
				if (a.AreAllPrerequisitesSatisfied())
				{
					if (a.mCost < min.mCost)
					{
						min = a;
					}
				}
			}

			// execute task
			foreach (var v in min.mEffect)
			{
				// run the effect on the owner
				Console.WriteLine(v);
				_ = v.DynamicInvoke();
			}

			// return status
			return false;
		}

		// IHasInventory
		public void AddToInventory(IWorldObject wo)
		{ mInventory[wo] = true; }

		// IHasInventory
		public void RemoveFromInventory(IWorldObject wo)
		{ mInventory[wo] = false; }

		// IHasInventory
		public bool HasInInventory(IWorldObject wo)
		{ return mInventory[wo]; }

		public void ApplyAction(Func<IWorldObject, bool> func)
		{
			throw new NotImplementedException();
		}

	}

	delegate void TransferObject(IHasInventory src, IHasInventory dst, IWorldObject obj);


	static class GOAP1
	{
		static System.Action Curry<T1>(System.Delegate action, T1 t1) { return () => action.DynamicInvoke(t1); }
		static System.Action Curry<T1, T2>(System.Delegate action, T1 t1, T2 t2) { return () => action.DynamicInvoke(t1, t2); }
		static System.Action Curry<T1, T2, T3>(System.Delegate action, T1 t1, T2 t2, T3 t3) { return () => action.DynamicInvoke(t1, t2, t3); }
		static System.Action Curry<T1, T2, T3, T4>(System.Delegate action, T1 t1, T2 t2, T3 t3, T4 t4) { return () => action.DynamicInvoke(t1, t2, t3, t4); }

		// predefined effects
		static TransferObject Transfer = (a, b, c) => { b.AddToInventory(c); a.RemoveFromInventory(c); };

		// predefined prerequisites

		static void Main2(string[] args)
		{

		}
	}
}