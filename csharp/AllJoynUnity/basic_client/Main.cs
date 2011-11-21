using System;
using AllJoynUnity;

namespace basic_client
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("AllJoyn Library version: " + AllJoyn.GetVersion());
			Console.WriteLine("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());
		}
	}
}
