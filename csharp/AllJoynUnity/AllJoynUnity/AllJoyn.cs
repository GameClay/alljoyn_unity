using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		// DLL name for externs
		private const string DLL_IMPORT_TARGET = "alljoyn_unity";
		
		public static string GetVersion()
		{
			return Marshal.PtrToStringAuto(alljoyn_getversion());
		}
		
		public static string GetBuildInfo()
		{
			return Marshal.PtrToStringAuto(alljoyn_getbuildinfo());
		}
		
		#region DLL Imports
		[DllImport(DLL_IMPORT_TARGET)]
		private extern static IntPtr alljoyn_getversion();
		
		[DllImport(DLL_IMPORT_TARGET)]
		private extern static IntPtr alljoyn_getbuildinfo();
		#endregion
	}
}

