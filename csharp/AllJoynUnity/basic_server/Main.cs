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

		private static bool sJoinComplete = false;
		private static AllJoyn.BusAttachment sMsgBus;
		private static AllJoyn.BusListener sBusListener;
		private static uint sSessionId;

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

			// Create a bus listener
			sBusListener = new AllJoyn.BusListener();
			sBusListener.NameOwnerChanged += new AllJoyn.BusListener.NameOwnerChangedEventHandler(NameOwnerChanged);
		}
	}
}
