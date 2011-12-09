using UnityEngine;
using AllJoynUnity;

namespace basic_clientserver
{
	class BasicServer
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_PATH = "/method_sample";
		private const ushort SERVICE_PORT = 25;

		//private const string connectArgs = "tcp:addr=127.0.0.1,port=9955";
		private const string connectArgs = "unix:abstract=alljoyn";
		//private const string connectArgs = "launchd:";

		private AllJoyn.BusAttachment msgBus;
		private MyBusListener busListener;
		private MySessionPortListener sessionPortListener;
		private TestBusObject testObj;
		private AllJoyn.InterfaceDescription testIntf;

		class TestBusObject : AllJoyn.BusObject
		{
			public TestBusObject(AllJoyn.BusAttachment bus, string path) : base(bus, path, false)
			{
				AllJoyn.InterfaceDescription exampleIntf = bus.GetInterface(INTERFACE_NAME);
				AllJoyn.QStatus status = AddInterface(exampleIntf);
				if(!status)
				{
					Debug.Log("Server Failed to add interface " + status);
				}

				AllJoyn.InterfaceDescription.Member catMember = exampleIntf.GetMember("cat");
				status = AddMethodHandler(catMember, this.Cat);
				if(!status)
				{
					Debug.Log("Server Failed to add method handler " + status);
				}
			}

			protected override void OnObjectRegistered()
			{
				Debug.Log("Server ObjectRegistered has been called");
			}

			protected void Cat(AllJoyn.InterfaceDescription.Member member, AllJoyn.Message message)
			{
				string outStr = (string)message[0] + (string)message[1];
				AllJoyn.MsgArgs outArgs = new AllJoyn.MsgArgs(1);
				outArgs[0] = outStr;

				AllJoyn.QStatus status = MethodReply(message, outArgs);
				if(!status)
				{
					Debug.Log("Server Ping: Error sending reply");
				}
			}
		}

		class MyBusListener : AllJoyn.BusListener
		{
			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					Debug.Log("Server NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner);
				}
			}
		}

		class MySessionPortListener : AllJoyn.SessionPortListener
		{
			protected override bool AcceptSessionJoiner(ushort sessionPort, string joiner, AllJoyn.SessionOpts opts)
			{
				if (sessionPort != SERVICE_PORT)
				{
					Debug.Log("Server Rejecting join attempt on unexpected session port " + sessionPort);
					return false;
				}
				Debug.Log("Server Accepting join session request from " + joiner + 
					" (opts.proximity=" + opts.Proximity + ", opts.traffic=" + opts.Traffic + 
					", opts.transports=" + opts.Transports + ")");
				return true;
			}
		}

		public BasicServer()
		{
			// Create message bus
			msgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.QStatus status = msgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
				Debug.Log("Server Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Debug.Log("Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Create a bus listener
			busListener = new MyBusListener();
			if(status)
			{
				msgBus.RegisterBusListener(busListener);
				Debug.Log("Server BusListener Registered.");
			}

			// Set up bus object
			testObj = new TestBusObject(msgBus, SERVICE_PATH);

			// Start the msg bus
			if(status)
			{
				status = msgBus.Start();
				if(status)
				{
					Debug.Log("Server BusAttachment started.");
					msgBus.RegisterBusObject(testObj);

					status = msgBus.Connect(connectArgs);
					if(status)
					{
						Debug.Log("Server BusAttchement connected to " + connectArgs);
					}
					else
					{
						Debug.Log("Server BusAttachment::Connect(" + connectArgs + ") failed.");
					}
				}
				else
				{
					Debug.Log("Server BusAttachment.Start failed.");
				}
			}

			// Request name
			if(status)
			{
				status = msgBus.RequestName(SERVICE_NAME,
					AllJoyn.DBus.NameFlags.ReplaceExisting | AllJoyn.DBus.NameFlags.DoNotQueue);
				if(!status)
				{
					Debug.Log("Server RequestName(" + SERVICE_NAME + ") failed (status=" + status + ")");
				}
			}

			// Create session
			AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
					AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);
			if(status)
			{
				ushort sessionPort = SERVICE_PORT;
				sessionPortListener = new MySessionPortListener();
				status = msgBus.BindSessionPort(ref sessionPort, opts, sessionPortListener);
				if(!status || sessionPort != SERVICE_PORT)
				{
					Debug.Log("Server BindSessionPort failed (" + status + ")");
				}
			}

			// Advertise name
			if(status)
			{
				status = msgBus.AdvertiseName(SERVICE_NAME, opts.Transports);
				if(!status)
				{
					Debug.Log("Server Failed to advertise name " + SERVICE_NAME + " (" + status + ")");
				}
			}
		}

		public bool KeepRunning
		{
			get
			{
				return true;
			}
		}
	}
}
