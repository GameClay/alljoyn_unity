using System;
using AllJoynUnity;

namespace basic_client
{
	class MainClass
	{
		private const string INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_NAME = "org.alljoyn.Bus.method_sample";
		private const string SERVICE_PATH = "/method_sample";
		
		//private const string connectArgs = "tcp:addr=127.0.0.1,port=9955";
		//private const string connectArgs = "unix:abstract=alljoyn";
		private const string connectArgs = "launchd:";
		
		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());
			
			// Create message bus
			AllJoyn.BusAttachment msgBus = new AllJoyn.BusAttachment("myApp", true);
			
			// Add org.alljoyn.Bus.method_sample interface
			AllJoyn.InterfaceDescription testIntf = msgBus.CreateInterface(INTERFACE_NAME, false);
			if(testIntf != null)
			{
				Console.WriteLine("Interface Created.");
				testIntf.AddMember(AllJoyn.Message.Type.MethodCall, "cat", "ss", "s", "inStr1,inStr2,outStr");
				testIntf.Activate();
			}
			else
			{
				Console.WriteLine("Failed to create interface 'org.alljoyn.Bus.method_sample'");
				return;
			}
			
			// Start the msg bus
			if(msgBus.Start())
			{
				Console.WriteLine("BusAttachment started.");
			}
			else
			{
				Console.WriteLine("BusAttachment.Start failed.");
				return;
			}
			
			// Connect to the bus
			if(msgBus.Connect(connectArgs))
			{
				Console.WriteLine("BusAttchement connected to " + connectArgs);
			}
			else
			{
				Console.WriteLine("BusAttachment::Connect(" + connectArgs + ") failed.");
			}
		}
	}
}
