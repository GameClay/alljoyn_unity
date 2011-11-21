using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class Message
		{
			public enum Type : int
			{
				Invalid = 0,
				MethodCall = 1,
				MethodReturn = 2,
				Error = 3,
				Signal = 4
			}
			
			#region DLL Imports
			#endregion
			
			#region Data
			#endregion
		}
	}
}
