using System;
using System.Collections.Generic;

namespace MessageSystem
{
	// MessageSystem holds n SubMessage systems, one for each type of message it sends
	// Message types are 1-1 with class types, i.e. the source code IS the protocol
	// All creation of the sub-message systems is automatic and handled internally :D
	class MessageSystem
	{
		interface ISubMessageSystem
		{ }

		// Implementation of the Observer pattern for a type T
		internal class SubMessageSystem<T> : ISubMessageSystem
		{
			public SubMessageSystem()
			{
				mListeners = new List<Action<T>>();
			}

			public void SendMessage(T tMsg)
			{
				foreach (var listener in mListeners)
				{
					listener(tMsg);
				}
			}

			public void AddListener(Action<T> funcPtr)
			{
				mListeners.Add(funcPtr);
			}

			readonly List<Action<T>> mListeners;
		}

		public void SendMessage<T>(ref T msg)
		{
			// broken into 2 lines for readability
			var subMsgSystem = mMsgSubSystemMap[typeof(T)];
			((SubMessageSystem<T>)subMsgSystem).SendMessage(msg);
		}

		// take in std::function
		public void AddListener<T>(Action<T> funcPtr)
		{
			var mapKey = typeof(T);

			// if message subsystem doesn't exist, add it on the fly
			if (!mMsgSubSystemMap.ContainsKey(mapKey))
			{
				//map.emplace(mapKey, std::make_unique<SubMessageSystem<T>>());
				mMsgSubSystemMap.Add(mapKey, new SubMessageSystem<T>());
			}

			((SubMessageSystem<T>)mMsgSubSystemMap[mapKey]).AddListener(funcPtr);
		}

		public MessageSystem()
		{
			mMsgSubSystemMap = new Dictionary<Type, ISubMessageSystem>();
		}

		readonly Dictionary<Type, ISubMessageSystem> mMsgSubSystemMap;
	}

	//class TestMsg { }

	//class ListenerOfTestMsgs
	//{
	//	public void OnMessage(TestMsg testMsg)
	//	{
	//		Console.WriteLine($"OnMessage_Trade: {testMsg}");
	//	}
	//}

	//public class ExampleProgram
	//{
	//	public static void Main(string[] args)
	//	{
	//		var msgSystem = new MessageSystem();
	//		var listener = new ListenerOfTestMsgs();

	//		var testMsg = new TestMsg();

	//		msgSystem.AddListener<TestMsg>(listener.OnMessage);
	//		msgSystem.SendMessage(ref testMsg);

	//		Console.ReadLine();
	//	}
	//}
}
