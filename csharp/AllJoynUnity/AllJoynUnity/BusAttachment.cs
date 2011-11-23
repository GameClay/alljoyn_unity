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
			
			public QStatus CreateInterface(string interfaceName, bool secure, out InterfaceDescription iface)
			{
				IntPtr interfaceDescription;
				int qstatus = alljoyn_busattachment_createinterface(_busAttachment,
					interfaceName, ref interfaceDescription, secure ? 1 : 0);
				if(qstatus == 0)
				{
					iface = new InterfaceDescription(interfaceDescription);
				}
				else
				{
					iface = null;
				}
				return qstatus;
			}
			
			public QStatus Start()
			{
				return alljoyn_busattachment_start(_busAttachment);
			}
			
			public QStatus Stop(bool blockUntilStopped)
			{
				return alljoyn_busattachment_stop(_busAttachment, blockUntilStopped ? 1 : 0);
			}
			
			public QStatus Connect(string connectSpec)
			{
				return alljoyn_busattachment_connect(_busAttachment, connectSpec);
			}
			
			public void RegisterBusListener(BusListener listener)
			{
				alljoyn_busattachment_registerbuslistener(_busAttachment, listener.UnmanagedPtr);
			}
			
			public QStatus FindAdvertisedName(string namePrefix)
			{
				return alljoyn_busattachment_findadvertisedname(_busAttachment, namePrefix);
			}
			
			public QStatus JoinSession(string sessionHost, ushort sessionPort, BusListener listener,
				out uint sessionId, SessionOpts opts)
			{
				IntPtr optsPtr = opts.UnmanagedPtr;
				uint sessionId_out = 0;
				int qstatus = alljoyn_busattachment_joinsession(_busAttachment, sessionHost, sessionPort,
					listener.UnmanagedPtr, ref sessionId_out, optsPtr);
				sessionId = sessionId_out;
				return qstatus;
			}
			
			public InterfaceDescription GetInterface(string name)
			{
				IntPtr iface = alljoyn_busattachment_getinterface(_busAttachment, name);
				InterfaceDescription ret = (iface != IntPtr.Zero ? new InterfaceDescription(iface) : null);
				
				return ret;
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
					alljoyn_busattachment_destroy(_busAttachment);
					_busAttachment = IntPtr.Zero;
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
			private extern static int alljoyn_busattachment_stop(IntPtr bus, int blockUntilStopped);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_connect(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string connectSpec);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_registerbuslistener(IntPtr bus, IntPtr listener);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_findadvertisedname(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string namePrefix);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_joinsession(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string sessionHost,
				ushort sessionPort,
				IntPtr listener,
				ref uint sessionId,
				IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getinterface(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name);
			#endregion
			
			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busAttachment;
				}
			}
			#endregion
			
			#region Data
			IntPtr _busAttachment;
			bool _isDisposed = false;
			#endregion
		}
	}
}

