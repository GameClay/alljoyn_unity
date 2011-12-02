using System;
using AllJoynUnity;

namespace basic_clientserver
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());

			// Enable callbacks on main thread only
			AllJoyn.SetMainThreadOnlyCallbacks(true);

			BasicServer basicServer = new BasicServer();
			BasicClient basicClient = new BasicClient();

			basicClient.Connect();

			while(!basicClient.Connected)
			{
				AllJoyn.TriggerCallbacks(); // Pump messages
				System.Threading.Thread.Sleep(1);
			}

			Console.WriteLine("BasicClient.CallRemoteMethod returned '{0}'", basicClient.CallRemoteMethod());

			while(basicServer.KeepRunning)
			{
				AllJoyn.TriggerCallbacks(); // Pump messages
				System.Threading.Thread.Sleep(1);
			}
		}
	}
}
