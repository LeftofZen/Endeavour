using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.GOAP
{
	class WorldObject
	{
		public WorldObject(string name)
		{
			mName = name;
		}

		void ApplyAction(Func<WorldObject, bool> func)
		{

		}

		public override string ToString()
		{ return mName; }

		string mName;
	}
}
