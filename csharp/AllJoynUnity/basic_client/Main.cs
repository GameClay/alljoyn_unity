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
		private static AllJoyn.BusListener sBusListener;
		private static uint sSessionId;
		
		private static void FoundAdvertisedName(object sender, AllJoyn.BusListener.AdvertisedNameEventArgs ea)
		{
			Console.WriteLine("FoundAdvertisedName(name=" + ea.name + ", prefix=" + ea.namePrefix + ")");
			if(string.Compare(SERVICE_NAME, ea.name) == 0)
			{
				// We found a remote bus that is advertising basic service's  well-known name so connect to it
				AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
				
				AllJoyn.QStatus status = sMsgBus.JoinSession(ea.name, SERVICE_PORT, sBusListener, out sSessionId, opts);
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
		
		private static void NameOwnerChanged(object sender, AllJoyn.BusListener.NameOwnerChangedEventArgs ea)
		{
			if(string.Compare(SERVICE_NAME, ea.busName) == 0)
			{
				Console.WriteLine("NameOwnerChanged: name=" + ea.busName + ", oldOwner=" +
					ea.previousOwner + ", newOwner=" + ea.newOwner);
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
			sBusListener = new AllJoyn.BusListener();
			sBusListener.FoundAdvertisedName += new AllJoyn.BusListener.FoundAdvertisedNameEventHandler(FoundAdvertisedName);
			sBusListener.NameOwnerChanged += new AllJoyn.BusListener.NameOwnerChangedEventHandler(NameOwnerChanged);
			
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
				}
			}
			
			// Stop the bus (not strictly necessary since we are going to delete it anyways)
			if(!sMsgBus.Stop(true))
			{
				Console.WriteLine("BusAttachment.Stop failed");
			}
			
			// Dispose of objects now
			sMsgBus.Dispose();
			sBusListener.Dispose();
			
			Console.WriteLine("basic client exiting with status {0} ({1})\n", status, status.ToString ());
		}
	}
}
