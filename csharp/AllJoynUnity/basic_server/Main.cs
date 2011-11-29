using System;
using AllJoynUnity;

namespace basic_server
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

		private static AllJoyn.BusAttachment sMsgBus;
		private static MyBusListener sBusListener;

		class TestBusObject : AllJoyn.BusObject
		{
			public TestBusObject(AllJoyn.BusAttachment bus, string path) : base(bus, path, false)
			{
				AllJoyn.InterfaceDescription exampleIntf = bus.GetInterface(INTERFACE_NAME);
				AllJoyn.QStatus status = AddInterface(exampleIntf);
				if(!status)
				{
					Console.WriteLine("Failed to add interface {0}", status);
				}

				AllJoyn.InterfaceDescription.Member catMember = exampleIntf.GetMember("cat");
				status = AddMethodHandler(catMember, this.Cat);
				if(!status)
				{
					Console.WriteLine("Failed to add method handler {0}", status);
				}
			}

			protected override void OnObjectRegistered()
			{
				Console.WriteLine("ObjectRegistered has been called");
			}

			protected void Cat(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
			{
				
			}
		}

		class MyBusListener : AllJoyn.BusListener
		{
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

			// Create a bus listener
			sBusListener = new MyBusListener();
			if(status)
			{
				sMsgBus.RegisterBusListener(sBusListener);
				Console.WriteLine("BusListener Registered.");
			}

			// Set up bus object
			TestBusObject testObj = new TestBusObject(sMsgBus, SERVICE_PATH);

			// Start the msg bus
			if(status)
			{
				status = sMsgBus.Start();
				if(status)
				{
					Console.WriteLine("BusAttachment started.");
					sMsgBus.RegisterBusObject(testObj);

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
				else
				{
					Console.WriteLine("BusAttachment.Start failed.");
				}
			}

			// Request name
			if(status)
			{
				status = sMsgBus.RequestName(SERVICE_NAME,
					AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
				if(!status)
				{
					Console.WriteLine("RequestName({0}) failed (status={1})\n", SERVICE_NAME, status);
				}
			}

			// Create session
			if(status)
			{
				AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

				//status = 
			}

			// Dispose of objects now
			sMsgBus.Dispose();
			sBusListener.Dispose();

			Console.WriteLine("basic server exiting with status {0} ({1})\n", status, status.ToString());
		}
	}
}
