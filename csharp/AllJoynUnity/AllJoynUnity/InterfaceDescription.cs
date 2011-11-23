using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class InterfaceDescription
		{
			[Flags]
			public enum AnnotationFlags : byte
			{
				Default = 0,
				NoReply = 1,
				Deprecated = 2
			}
			
			[Flags]
			public enum AccessFlags : byte
			{
				Read = 1,
				Write = 2
			}
			
			internal InterfaceDescription(IntPtr interfaceDescription)
			{
				_interfaceDescription = interfaceDescription;
			}
			
			public QStatus AddMember(Message.Type type, string name, string inputSignature,
				string outputSignature, string argNames, AnnotationFlags annotation = AnnotationFlags.Default)
			{
				return alljoyn_interfacedescription_addmember(_interfaceDescription,
					(int)type, name, inputSignature, outputSignature, argNames, (byte)annotation);
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
			
			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _interfaceDescription;
				}
			}
			#endregion
			
			#region Data
			IntPtr _interfaceDescription;
			#endregion
		}
	}
}

