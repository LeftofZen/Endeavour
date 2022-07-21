using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ConsoleApp1
{
	[Serializable]
	struct MsgHeader
	{
		public int size;
		public int msgType;
		//public int timestamp;

		public override string ToString()
		{
			return string.Format("[MsgHeader] size={0} msgType={1}", size, msgType);
		}
	}

	[Serializable]
	class Player
	{
		public Player(string name)
		{
			mName = name;
		}

		public int x = 0;
		public int y = 0;
		public string mName = "null";

		public override string ToString()
		{
			return string.Format("[Player] name={0} x={1} y={2}", mName, x, y);
		}
	}

	class Client
	{
		public Client(string clientName)
		{
			mClientName = clientName;
			mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			mPlayer = new Player(clientName);

			bwReceiver.DoWork += new DoWorkEventHandler(OnReceive);
			bwReceiver.RunWorkerAsync();

			heartbeater.DoWork += new DoWorkEventHandler(SendHeartbeat);
			heartbeater.RunWorkerAsync();
		}

		public bool ConnectToServer(IPEndPoint endpoint)
		{
			mServerSocket.Connect(endpoint);
			Console.WriteLine("[Client] [{0}] Connected={0} Address={1}", mClientName, mServerSocket.Connected, endpoint);

			return mServerSocket.Connected;
		}

		public void SendHeartbeat(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				if (mServerSocket.Connected)
				{
					// send an update msg
					var msg = Serialise();
					_ = mServerSocket.Send(msg);
				}

				System.Threading.Thread.Sleep(1000);
			}

		}

		private void OnReceive(object sender, DoWorkEventArgs e)
		{
			var buffer = new byte[1000];

			while (true)
			{
				if (mServerSocket.Connected)
				{
					var bytes = mServerSocket.Receive(buffer);
					if (bytes > 0)
					{
						Console.WriteLine("[Client] [OnReceive] bytes={0}", bytes);
						var ms = new MemoryStream(buffer);
						var formatter = new BinaryFormatter();

						var hdr = new MsgHeader();
						hdr = (MsgHeader)formatter.Deserialize(ms);

						var player = new Player("_");
						player = (Player)formatter.Deserialize(ms);
						Console.WriteLine("[Client] [OnReceive] [{0}] hdr={{{1}}} player_recv={{{2}}}",
							mPlayer, hdr, player);
					}
				}
				else
				{
					Console.WriteLine("[Client] [{0}] Error: server isn't connected", mClientName);
				}
			}
		}

		byte[] Serialise()
		{
			var formatter = new BinaryFormatter();
			var stream = new MemoryStream();

			// write header
			var hdr = new MsgHeader()
			{
				size = 1,
				msgType = 2
			};

			formatter.Serialize(stream, hdr);

			// write player data
			mPlayer.x = new Random().Next(100);
			mPlayer.y = new Random().Next(100);
			formatter.Serialize(stream, mPlayer);
			Console.WriteLine("[Client] [Serialise] Player={0}", mPlayer);
			return stream.ToArray();
		}

		Socket mServerSocket;
		BackgroundWorker bwReceiver = new BackgroundWorker();
		BackgroundWorker heartbeater = new BackgroundWorker();
		string mClientName = "";

		// game-specific
		Player mPlayer;
	}

	class Server
	{
		public Server()
		{
			mLocalEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
			mSockets = new List<Socket>();

			// setup main listen thread
			bwListener.DoWork += new DoWorkEventHandler(StartToListen);
			bwListener.RunWorkerAsync();

			bwReceiver.DoWork += new DoWorkEventHandler(OnReceive);
			bwReceiver.RunWorkerAsync();
		}

		BackgroundWorker bwListener = new BackgroundWorker();
		BackgroundWorker bwReceiver = new BackgroundWorker();

		private void BroadcastMsgToAllButOne(Socket from, Player data)
		{
			lock (mSockets)
			{
				var formatter = new BinaryFormatter();
				var stream = new MemoryStream();

				// write header
				var hdr = new MsgHeader()
				{
					size = 2,
					msgType = 3
				};

				Console.WriteLine("[Server] [BroadcastMsgToAllButOne] from={0} hdr={1} msg={2}",
					from.LocalEndPoint, hdr, data);

				formatter.Serialize(stream, hdr);
				formatter.Serialize(stream, data);

				foreach (var sock in mSockets)
				{
					if (sock != from)
					{
						_ = sock.Send(stream.ToArray());
					}
				}
			}
		}

		private void OnReceive(object sender, DoWorkEventArgs e)
		{
			var buffer = new byte[1000];
			var mMsgs = new Queue<KeyValuePair<Socket, Player>>();
			while (true)
			{
				lock (mSockets)
				{
					foreach (var sock in mSockets)
					{
						var bytes = sock.Receive(buffer);
						if (bytes > 0)
						{
							Console.WriteLine("[Server] [OnReceive] bytes={0}", bytes);
							var ms = new MemoryStream(buffer);
							var formatter = new BinaryFormatter();

							var hdr = new MsgHeader();
							hdr = (MsgHeader)formatter.Deserialize(ms);

							var player = new Player("_");
							player = (Player)formatter.Deserialize(ms);

							Console.WriteLine("[Server] [OnReceive] hdr={{{0}}} player={{{1}}}", hdr, player);

							mMsgs.Enqueue(new KeyValuePair<Socket, Player>(sock, player));
						}
					}
				}

				// forward to clients
				foreach (var msg in mMsgs)
				{
					BroadcastMsgToAllButOne(msg.Key, msg.Value);
				}
				mMsgs.Clear();
			}
		}

		private void StartToListen(object sender, DoWorkEventArgs e)
		{
			var listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listenerSocket.Bind(mLocalEndPoint);
			listenerSocket.Listen(200);

			while (true)
			{
				var sock = listenerSocket.Accept();
				if (sock.Connected && !mSockets.Contains(sock))
				{
					Console.WriteLine("[Server] [StartToListen] Client={0}", sock.LocalEndPoint);
					lock (mSockets)
					{
						mSockets.Add(sock);
					}
				}
			}
		}

		string serverIP = "127.0.0.1";
		int serverPort = 8000;

		List<Socket> mSockets;
		public IPEndPoint mLocalEndPoint;
	}

	class TcpServerProgram
	{
		public static void Sleep(int ms)
		{
			System.Threading.Thread.Sleep(ms);
		}

		public static void RunTcp()
		{
			Console.WriteLine("---");

			var mServer = new Server();
			var C1 = new Client("c1");
			var C2 = new Client("c2");
			var C3 = new Client("c3");

			_ = C1.ConnectToServer(mServer.mLocalEndPoint);
			_ = C2.ConnectToServer(mServer.mLocalEndPoint);
			_ = C3.ConnectToServer(mServer.mLocalEndPoint);

			Console.WriteLine("---");
			_ = Console.ReadLine();
		}

		public static void TcpMain(string[] args)
		{
			RunTcp();
		}
	}
}
