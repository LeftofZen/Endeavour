using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Simulation
	{
		class Agent
		{
			public void Update()
			{
				System.Threading.Thread.Sleep(1);
				//Console.WriteLine("[Update] time={0}", DateTime.Now.Millisecond);
			}

			public void Render()
			{
				System.Threading.Thread.Sleep(1);
				//Console.WriteLine("[Render] time={0}", DateTime.Now.Millisecond);
			}
		}

		public static void SimMain(string[] args)
		{
			List<Agent> mAgents = new List<Agent>();
			int mAgentCount = 1000;

			for (int i = 0; i < mAgentCount; ++i)
			{
				mAgents.Add(new Agent());
			}
			
			int count = 0;
			while (count++ < 100)
			{
				DateTime dt = DateTime.Now;

				foreach (Agent a in mAgents)
					a.Update();

				foreach (Agent a in mAgents)
					a.Render();

				TimeSpan diff = DateTime.Now - dt;
				Console.WriteLine("Update {0}, Time={1}", count, diff);

			}

			Console.ReadLine();
		}

	}
}
