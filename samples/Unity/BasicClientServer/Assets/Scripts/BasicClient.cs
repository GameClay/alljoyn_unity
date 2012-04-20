using UnityEngine;
using AllJoynUnity;

namespace basic_clientserver
{
	class BasicClient
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
				Debug.Log("Client FoundAdvertisedName(name=" + name + ", prefix=" + namePrefix + ")");
				if(string.Compare(SERVICE_NAME, name) == 0)
				{
					// We found a remote bus that is advertising basic service's  well-known name so connect to it
					AllJoyn.SessionOpts opts = new AllJoyn.SessionOpts(AllJoyn.SessionOpts.TrafficType.Messages, false,
						AllJoyn.SessionOpts.ProximityType.Any, AllJoyn.TransportMask.Any);

					AllJoyn.QStatus status = sMsgBus.JoinSession(name, SERVICE_PORT, null, out sSessionId, opts);
					if(status)
					{
						Debug.Log("Client JoinSession SUCCESS (Session id=" + sSessionId + ")");
					}
					else
					{
						Debug.Log("Client JoinSession failed (status=" + status.ToString() + ")");
					}
				}
				sJoinComplete = true;
			}

			protected override void NameOwnerChanged(string busName, string previousOwner, string newOwner)
			{
				if(string.Compare(SERVICE_NAME, busName) == 0)
				{
					Debug.Log("Client NameOwnerChanged: name=" + busName + ", oldOwner=" +
						previousOwner + ", newOwner=" + newOwner);
				}
			}
		}

		public BasicClient()
		{
			// Create message bus
			sMsgBus = new AllJoyn.BusAttachment("myApp", true);

			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.InterfaceDescription testIntf;
			AllJoyn.QStatus status = sMsgBus.CreateInterface(INTERFACE_NAME, false, out testIntf);
			if(status)
			{
				Debug.Log("Client Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Debug.Log("Client Failed to create interface 'org.alljoyn.Bus.method_sample'");
			}

			// Start the msg bus
			if(status)
			{
				status = sMsgBus.Start();
				if(status)
				{
					Debug.Log("Client BusAttachment started.");
				}
				else
				{
					Debug.Log("Client BusAttachment.Start failed.");
				}
			}

			// Connect to the bus
			if(status)
			{
				status = sMsgBus.Connect(connectArgs);
				if(status)
				{
					Debug.Log("Client BusAttchement connected to " + connectArgs);
				}
				else
				{
					Debug.Log("Client BusAttachment::Connect(" + connectArgs + ") failed.");
				}
			}

			// Create a bus listener
			sBusListener = new MyBusListener();

			if(status)
			{
				sMsgBus.RegisterBusListener(sBusListener);
				Debug.Log("Client BusListener Registered.");
			}
		}

		public AllJoyn.QStatus Connect()
		{
			// Begin discovery on the well-known name of the service to be called
			AllJoyn.QStatus status = sMsgBus.FindAdvertisedName(SERVICE_NAME);
			if(!status)
			{
				Debug.Log("Client org.alljoyn.Bus.FindAdvertisedName failed.");
			}

			return status;
		}

		public bool Connected
		{
			get
			{
				return sJoinComplete;
			}
		}

		public string CallRemoteMethod()
		{
			using(AllJoyn.ProxyBusObject remoteObj = new AllJoyn.ProxyBusObject(sMsgBus, SERVICE_NAME, SERVICE_PATH, sSessionId))
			{
				AllJoyn.InterfaceDescription alljoynTestIntf = sMsgBus.GetInterface(INTERFACE_NAME);
				if(alljoynTestIntf == null)
				{
					//throw new Exception("Client Failed to get test interface.");
					return "";
				}
				else
				{
					remoteObj.AddInterface(alljoynTestIntf);

					AllJoyn.Message reply = new AllJoyn.Message(sMsgBus);
					AllJoyn.MsgArgs inputs = new AllJoyn.MsgArgs(2);
					inputs[0] = "Hello ";
					inputs[1] = "World!";

					AllJoyn.QStatus status = remoteObj.MethodCallSynch(SERVICE_NAME, "cat", inputs, reply, 5000, 0);
					
					if(status)
					{
						//Debug.Log(SERVICE_NAME + ".cat(path=" + SERVICE_PATH + ") returned \"" + (string)reply[0] + "\"");
						return (string)reply[0];
					}
					else
					{
						Debug.Log("MethodCall on " + SERVICE_NAME + ".cat failed");
						return "";
					}
				}
			}
		}
	}
}
