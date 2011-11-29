using System;
using AllJoynUnity;

namespace basic_client
{
	class MainClass
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_PATH = "/method_sample";
		private const ushort SERVICE_PORT = 25;

		//private const string connectArgs = "tcp:addr=127.0.0.1,port=9955";
		private const string connectArgs = "unix:abstract=alljoyn";
		//private const string connectArgs = "launchd:";

		private static bool sJoinComplete = false;
		private static AllJoyn.BusAttachment sMsgBus;
		private static MyBusListener sBusListener;
		private static uint sSessionId;

		class MyBusListener : AllJoyn.BusListener
		{
			protected override void FoundAdvertisedName(string name, AllJoyn.TransportMask transport, string namePrefix)
			{
				Console.WriteLine("FoundAdvertisedName(name=" + name + ", prefix=" + namePrefix + ")");
				if(string.Compare(SERVICE_NAME, name) == 0)
				{
					// We found a remote bus that is advertising basic service's  well-known name so connect to it
					AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
						AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

					AllJoyn.QStatus status = sMsgBus.JoinSession(name, SERVICE_PORT, this, out sSessionId, opts);
					if(status)
					{
						Console.WriteLine("JoinSession SUCCESS (Session id={0})", sSessionId);
					}
					else
					{
						Console.WriteLine("JoinSession failed (status={0})", status.ToString ());
					}
				}
				sJoinComplete = true;
			}

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					Console.WriteLine("NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner);
				}
			}
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());

			// Create message bus
			sMsgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.InterfaceDescription testIntf;
			AllJoyn.QStatus status = sMsgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
				Console.WriteLine("Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Console.WriteLine("Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Start the msg bus
			if(status)
			{
				status = sMsgBus.Start();
				if(status)
				{
					Console.WriteLine("BusAttachment started.");
				}
				else
				{
					Console.WriteLine("BusAttachment.Start failed.");
				}
			}

			// Connect to the bus
			if(status)
			{
				status = sMsgBus.Connect(connectArgs);
				if(status)
				{
					Console.WriteLine("BusAttchement connected to " + connectArgs);
				}
				else
				{
					Console.WriteLine("BusAttachment::Connect(" + connectArgs + ") failed.");
				}
			}

			// Create a bus listener
			sBusListener = new MyBusListener();

			if(status)
			{
				sMsgBus.RegisterBusListener(sBusListener);
				Console.WriteLine("BusListener Registered.");
			}

			// Begin discovery on the well-known name of the service to be called
			if(status)
			{
				status = sMsgBus.FindAdvertisedName(SERVICE_NAME);
				if(!status)
				{
					Console.WriteLine("org.alljoyn.Bus.FindAdvertisedName failed.");
				}
			}

			// Wait for join session to complete
			while(sJoinComplete == false)
			{
				System.Threading.Thread.Sleep(1);
			}

			if(status)
			{
				using(AllJoyn.ProxyBusObject remoteObj = new AllJoyn.ProxyBusObject(sMsgBus, SERVICE_NAME, SERVICE_PATH, sSessionId))
				{
					AllJoyn.InterfaceDescription alljoynTestIntf = sMsgBus.GetInterface(INTERFACE_NAME);
					if(alljoynTestIntf == null)
					{
						throw new Exception("Failed to get test interface.");
					}
					remoteObj.AddInterface(alljoynTestIntf);

					AllJoyn.Message reply = new AllJoyn.Message(sMsgBus);
					AllJoyn.MsgArgs inputs = new AllJoyn.MsgArgs(2);
					inputs[0] = "Hello ";
					inputs[1] = "World!";

					status = remoteObj.MethodCallSynch(SERVICE_NAME, "cat", inputs, reply, 5000, 0);
					
					if(status)
					{
						Console.WriteLine("{0}.{1} (path={2}) returned \"{3}\"", SERVICE_NAME, "cat", SERVICE_PATH,
							(string)reply[0]);
					}
					else
					{
						Console.WriteLine("MethodCall on {0}.{1} failed", SERVICE_NAME, "cat");
					}
				}
			}

			// Dispose of objects now
			sMsgBus.Dispose();
			sBusListener.Dispose();

			Console.WriteLine("basic client exiting with status {0} ({1})\n", status, status.ToString());
		}
	}
}
