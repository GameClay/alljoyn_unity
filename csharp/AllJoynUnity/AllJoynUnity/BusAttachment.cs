using System;
using System.Threading;
using System.Collections.Generic;
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

				if(_sBusAttachmentMap == null) _sBusAttachmentMap = new Dictionary<IntPtr, BusAttachment>();
				_sBusAttachmentMap.Add(_busAttachment, this);
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

			public QStatus Stop()
			{
				return alljoyn_busattachment_stop(_busAttachment);
			}

			public QStatus Connect(string connectSpec)
			{
				return alljoyn_busattachment_connect(_busAttachment, connectSpec);
			}

			public void RegisterBusListener(BusListener listener)
			{
				alljoyn_busattachment_registerbuslistener(_busAttachment, listener.UnmanagedPtr);
			}

			public void UnregisterBusListener(BusListener listener)
			{
				alljoyn_busattachment_unregisterbuslistener(_busAttachment, listener.UnmanagedPtr);
			}

			public QStatus FindAdvertisedName(string namePrefix)
			{
				return alljoyn_busattachment_findadvertisedname(_busAttachment, namePrefix);
			}

			public QStatus CancelFindAdvertisedName(string namePrefix)
			{
				return alljoyn_busattachment_cancelfindadvertisedname(_busAttachment, namePrefix);
			}

			public QStatus JoinSession(string sessionHost, ushort sessionPort, SessionListener listener,
				out uint sessionId, SessionOpts opts)
			{
				IntPtr optsPtr = opts.UnmanagedPtr;
				uint sessionId_out = 0;
				int qstatus = 0;
				Thread joinSessionThread = new Thread((object o) => {
					qstatus = alljoyn_busattachment_joinsession(_busAttachment, sessionHost, sessionPort,
						(listener == null ? IntPtr.Zero : listener.UnmanagedPtr), ref sessionId_out, optsPtr);
				});
				joinSessionThread.Start();
				while(joinSessionThread.IsAlive)
				{
					AllJoyn.TriggerCallbacks();
					Thread.Sleep(1);
				}
				
				sessionId = sessionId_out;
				return qstatus;
			}

			public QStatus AdvertiseName(string name, TransportMask transports)
			{
				return alljoyn_busattachment_advertisename(_busAttachment, name, (ushort)transports);
			}

			public QStatus CancelAdvertisedName(string name, TransportMask transports)
			{
				return alljoyn_busattachment_canceladvertisename(_busAttachment, name, (ushort)transports);
			}

			public InterfaceDescription GetInterface(string name)
			{
				IntPtr iface = alljoyn_busattachment_getinterface(_busAttachment, name);
				InterfaceDescription ret = (iface != IntPtr.Zero ? new InterfaceDescription(iface) : null);
				
				return ret;
			}

			public QStatus RegisterBusObject(BusObject obj)
			{
				return alljoyn_busattachment_registerbusobject(_busAttachment, obj.UnmanagedPtr);
			}

			public void UnregisterBusObject(BusObject obj)
			{
				alljoyn_busattachment_unregisterbusobject(_busAttachment, obj.UnmanagedPtr);
			}

			public QStatus RequestName(string requestedName, DBus.NameFlags flags)
			{
				return alljoyn_busattachment_requestname(_busAttachment, requestedName, (uint)flags);
			}

			public QStatus ReleaseName(string requestedName)
			{
				return alljoyn_busattachment_releasename(_busAttachment, requestedName);
			}

			public QStatus BindSessionPort(ref ushort sessionPort, SessionOpts opts, SessionPortListener listener)
			{
				QStatus ret = QStatus.OK;
				ushort otherSessionPort = sessionPort;
				Thread bindThread = new Thread((object o) => {
					ret = alljoyn_busattachment_bindsessionport(_busAttachment, ref otherSessionPort,
						opts.UnmanagedPtr, listener.UnmanagedPtr);
				});
				bindThread.Start();
				while(bindThread.IsAlive)
				{
					AllJoyn.TriggerCallbacks();
					Thread.Sleep(0);
				}
				sessionPort = otherSessionPort;
				return ret;
			}

			public QStatus UnbindSessionPort(ushort sessionPort)
			{
				QStatus ret = QStatus.OK;
				Thread bindThread = new Thread((object o) => {
					ret = alljoyn_busattachment_unbindsessionport(_busAttachment, sessionPort);
				});
				bindThread.Start();
				while(bindThread.IsAlive)
				{
					AllJoyn.TriggerCallbacks();
					Thread.Sleep(0);
				}
				return ret;
			}

			internal static BusAttachment MapBusAttachment(IntPtr key)
			{
				return _sBusAttachmentMap[key];
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
					Thread destroyThread = new Thread((object o) => { alljoyn_busattachment_destroy(_busAttachment); });
					destroyThread.Start();
					while(destroyThread.IsAlive)
					{
						AllJoyn.TriggerCallbacks();
						Thread.Sleep(0);
					}
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
			private extern static int alljoyn_busattachment_stop(IntPtr bus);

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

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_unregisterbuslistener(IntPtr bus, IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_cancelfindadvertisedname(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string namePrefix);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_advertisename(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name, ushort transports);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_canceladvertisename(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name, ushort transports);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_registerbusobject(IntPtr bus, IntPtr obj);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_unregisterbusobject(IntPtr bus, IntPtr obj);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_requestname(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string requestedName, uint flags);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_releasename(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_bindsessionport(IntPtr bus,
				ref ushort sessionPort,
				IntPtr opts,
				IntPtr listener);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_unbindsessionport(IntPtr bus, ushort sessionPort);
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

			static Dictionary<IntPtr, BusAttachment> _sBusAttachmentMap;
			#endregion
		}
	}
}

