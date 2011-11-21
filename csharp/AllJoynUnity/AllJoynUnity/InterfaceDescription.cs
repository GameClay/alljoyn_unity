using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class InterfaceDescription
		{
			public enum AnnotationFlags : byte
			{
				Default = 0,
				NoReply = 1,
				Deprecated = 2,
				NoReply_Deprecated = 3
			}
			
			public enum AccessFlags : byte
			{
				Read = 1,
				Write = 2,
				ReadWrite = 3
			}
			
			internal InterfaceDescription(IntPtr interfaceDescription)
			{
				_interfaceDescription = interfaceDescription;
			}
			
			public bool AddMember(Message.Type type, string name, string inputSignature,
				string outputSignature, string argNames, AnnotationFlags annotation = AnnotationFlags.Default)
			{
				int qstatus = alljoyn_interfacedescription_addmember(_interfaceDescription,
					(int)type, name, inputSignature, outputSignature, argNames, (byte)annotation);
				return (qstatus == 0);
			}
			
			public void Activate()
			{
				alljoyn_interfacedescription_activate(_interfaceDescription);
			}
			
			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_addmember(
				IntPtr iface,
				int type,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string inputSig,
				[MarshalAs(UnmanagedType.LPStr)] string outSig,
				[MarshalAs(UnmanagedType.LPStr)] string argNames,
				byte annotation);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_interfacedescription_activate(IntPtr iface);
			#endregion
			
			#region Data
			IntPtr _interfaceDescription;
			#endregion
		}
	}
}

