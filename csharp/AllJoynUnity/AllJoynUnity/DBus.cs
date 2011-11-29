using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class DBus
		{
			[Flags]
			public enum NameFlags : uint
			{
				AllowReplacement = 0x01,
				ReplaceExisting = 0x02,
				DoNotQueue = 0x04
			}
		}
	}
}
