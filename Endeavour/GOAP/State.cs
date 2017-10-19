using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.GOAP
{
	interface IGOAPState
	{
		bool Evaluate();

		void SetAgent(Agent a);

		Agent GetAgent();

		string GetName();
	}

	#region Variadic Templates

	abstract class GOAPStateAbstract : IGOAPState
	{
		protected GOAPStateAbstract(string name, Agent a = null)
		{
			mName = name;
			mAgent = a;
		}

		public abstract bool Evaluate();

		public void SetAgent(Agent a)
		{ mAgent = a; }

		public Agent GetAgent()
		{ return mAgent; }

		public string GetName() { return mName; }


		protected Agent mAgent = null;
		protected string mName;
	}

	class GOAPState : GOAPStateAbstract
	{
		public GOAPState(string name, Func<Agent, bool> func, Agent a = null) : base(name, a)
		{
			mPrerequisite = func;
		}

		public override bool Evaluate() { return mPrerequisite(mAgent); }

		Func<Agent, bool> mPrerequisite;
	}

	class GOAPState<T1> : GOAPStateAbstract
	{
		public GOAPState(string name, T1 t1, Func<T1, Agent, bool> func, Agent a = null) : base(name, a)
		{
			mT1 = t1;
			mPrerequisite = func;
		}

		public override bool Evaluate() { return mPrerequisite(mT1, mAgent); }

		Func<T1, Agent, bool> mPrerequisite;
		T1 mT1;
	}

	class GOAPState<T1, T2> : GOAPStateAbstract
	{
		public GOAPState(string name, T1 t1, T2 t2, Func<T1, T2, Agent, bool> func, Agent a = null) : base(name, a)
		{
			mT1 = t1;
			mT2 = t2;
			mBehaviour = func;
		}

		public override bool Evaluate() { return mBehaviour(mT1, mT2, mAgent); }

		Func<T1, T2, Agent, bool> mBehaviour;
		T1 mT1;
		T2 mT2;
	}

	class GOAPState<T1, T2, T3> : GOAPStateAbstract
	{
		public GOAPState(string name, T1 t1, T2 t2, T3 t3, Func<T1, T2, T3, Agent, bool> func, Agent a = null) : base(name, a)
		{
			mT1 = t1;
			mT2 = t2;
			mT3 = t3;
			mBehaviour = func;
		}

		public override bool Evaluate() { return mBehaviour(mT1, mT2, mT3, mAgent); }

		Func<T1, T2, T3, Agent, bool> mBehaviour;
		T1 mT1;
		T2 mT2;
		T3 mT3;
	}

	#endregion
}
