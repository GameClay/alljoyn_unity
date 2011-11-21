using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class BusAttachment : IDisposable
		{
			public BusAttachment(string applicationName, bool allowRemoteMessages)
			{
				_busAttachment = alljoyn_busattachment_create(applicationName, allowRemoteMessages ? 1 : 0);
			}
			
			public InterfaceDescription CreateInterface(string interfaceName, bool secure)
			{
				IntPtr interfaceDescription;
				int qstatus = alljoyn_busattachment_createinterface(_busAttachment,
					interfaceName, ref interfaceDescription, secure ? 1 : 0);
				
				return (qstatus == 0 ? new InterfaceDescription(interfaceDescription) : null);
			}
			
			public bool Start()
			{
				int qstatus = alljoyn_busattachment_start(_busAttachment);
				return (qstatus == 0);
			}
			
			public bool Connect(string connectSpec)
			{
				int qstatus = alljoyn_busattachment_connect(_busAttachment, connectSpec);
				return (qstatus == 0);
			}
			
			#region IDisposable
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}
			
			protected virtual void Dispose(bool disposing)
			{
				if(!_isDisposed)
				{
					if(disposing) 
					{
						// Code to dispose the managed resources
						// held by the class
					}
					
					alljoyn_busattachment_destroy(_busAttachment);
				}
				_isDisposed = true;
			}
			
			~BusAttachment()
			{
				Dispose(false);
			}
			#endregion
			
			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_create(
				[MarshalAs(UnmanagedType.LPStr)] string applicationName,
				int allowRemoteMessages);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_destroy(IntPtr busAttachment);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_createinterface(
				IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref IntPtr iface,
				int secure);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_start(IntPtr bus);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_connect(IntPtr bus, [MarshalAs(UnmanagedType.LPStr)] string connectSpec);
			#endregion
			
			#region Data
			IntPtr _busAttachment;
			bool _isDisposed = false;
			#endregion
		}
	}
}

