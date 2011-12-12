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

			public QStatus EnablePeerSecurity(string authMechanisms, AuthListener listener, string keyStoreFileName, bool isShared)
			{
				return alljoyn_busattachment_enablepeersecurity(_busAttachment,
					authMechanisms, (listener == null ? IntPtr.Zero : listener.UnmanagedPtr),
					keyStoreFileName, (isShared ? 1 : 0));
			}

			public QStatus CreateInterfacesFromXml(string xml)
			{
				return alljoyn_busattachment_createinterfacesfromxml(_busAttachment, xml);
			}

			public InterfaceDescription[] GetInterfaces()
			{
				UIntPtr numIfaces = alljoyn_busattachment_getinterfaces(_busAttachment, IntPtr.Zero, (UIntPtr)0);
				IntPtr[] ifaces = new IntPtr[(int)numIfaces];
				GCHandle gch = GCHandle.Alloc(ifaces, GCHandleType.Pinned);
				UIntPtr numIfacesFilled = alljoyn_busattachment_getinterfaces(_busAttachment,
					gch.AddrOfPinnedObject(), numIfaces);
				gch.Free();
				if(numIfaces != numIfacesFilled)
				{
					// Warn?
				}
				InterfaceDescription[] ret = new InterfaceDescription[(int)numIfacesFilled];
				for(int i = 0; i < ret.Length; i++)
				{
					ret[i] = new InterfaceDescription(ifaces[i]);
				}
				return ret;
			}

			public QStatus DeleteInterface(InterfaceDescription iface)
			{
				return alljoyn_busattachment_deleteinterface(_busAttachment, iface.UnmanagedPtr);
			}

			public QStatus Disconnect(string connectSpec)
			{
				return alljoyn_busattachment_disconnect(_busAttachment, connectSpec);
			}

			public QStatus RegisterKeyStoreListener(KeyStoreListener listener)
			{
				return alljoyn_busattachment_registerkeystorelistener(_busAttachment, listener.UnmanagedPtr);
			}

			public QStatus ReloadKeyStore()
			{
				return alljoyn_busattachment_reloadkeystore(_busAttachment);
			}

			public void ClearKeyStore()
			{
				alljoyn_busattachment_clearkeystore(_busAttachment);
			}

			public QStatus ClearKeys(string guid)
			{
				return alljoyn_busattachment_clearkeys(_busAttachment, guid);
			}

			public QStatus SetKeyExpiration(string guid, uint timeout)
			{
				return alljoyn_busattachment_setkeyexpiration(_busAttachment, guid, timeout);
			}

			public QStatus GetKeyExpiration(string guid, out uint timeout)
			{
				uint _timeout = 0;
				QStatus ret = alljoyn_busattachment_getkeyexpiration(_busAttachment, guid, ref _timeout);
				timeout = _timeout;
				return ret;
			}

			public QStatus AddLogonEntry(string authMechanism, string userName, string password)
			{
				return alljoyn_busattachment_addlogonentry(_busAttachment, authMechanism, userName, password);
			}

			public QStatus AddMatch(string rule)
			{
				return alljoyn_busattachment_addmatch(_busAttachment, rule);
			}

			public QStatus RemoveMatch(string rule)
			{
				return alljoyn_busattachment_removematch(_busAttachment, rule);
			}

			public QStatus SetSessionListener(SessionListener listener, uint sessionId)
			{
				return alljoyn_busattachment_setsessionlistener(_busAttachment, sessionId, listener.UnmanagedPtr);
			}

			public QStatus LeaveSession(uint sessionId)
			{
				return alljoyn_busattachment_leavesession(_busAttachment, sessionId);
			}

			public QStatus SetLinkTimeout(uint sessionId, ref uint linkTimeout)
			{
				return alljoyn_busattachment_setlinktimeout(_busAttachment, sessionId, ref linkTimeout);
			}

			public QStatus GetPeerGuid(string name, out string guid)
			{
				UIntPtr guidSz;
				QStatus ret = alljoyn_busattachment_getpeerguid(_busAttachment, name,
					IntPtr.Zero, ref guidSz);
				if(!ret)
				{
					guid = "";
				}
				else
				{
					byte[] guidBuffer = new byte[(int)guidSz];
					GCHandle gch = GCHandle.Alloc(guidBuffer, GCHandleType.Pinned);
					ret = alljoyn_busattachment_getpeerguid(_busAttachment, name,
						gch.AddrOfPinnedObject(), ref guidSz);
					gch.Free();
					if(!ret)
					{
						guid = "";
					}
					else
					{
						guid = System.Text.ASCIIEncoding.ASCII.GetString(guidBuffer);
					}
				}
				return ret;
			}

			public QStatus NameHasOwner(string name, out bool hasOwner)
			{
				int intHasOwner = 0;
				QStatus ret = alljoyn_busattachment_namehasowner(_busAttachment, name, ref intHasOwner);
				hasOwner = (intHasOwner == 1 ? true : false);
				return ret;
			}

			public QStatus SetDaemonDebug(string module, uint level)
			{
				return alljoyn_busattachment_setdaemondebug(_busAttachment, module, level);
			}

			#region Properties
			public bool IsPeerSecurityEnabled
			{
				get
				{
					return (alljoyn_busattachment_ispeersecurityenabled(_busAttachment) == 1 ? true : false);
				}
			}

			public bool IsStarted
			{
				get
				{
					return (alljoyn_busattachment_isstarted(_busAttachment) == 1 ? true : false);
				}
			}

			public bool IsStopping
			{
				get
				{
					return (alljoyn_busattachment_isstopping(_busAttachment) == 1 ? true : false);
				}
			}

			public bool IsConnected
			{
				get
				{
					return (alljoyn_busattachment_isconnected(_busAttachment) == 1 ? true : false);
				}
			}

			public ProxyBusObject DBusProxyObj
			{
				get
				{
					return new ProxyBusObject(alljoyn_busattachment_getdbusproxyobj(_busAttachment));
				}
			}

			public ProxyBusObject AllJoynProxyObj
			{
				get
				{
					return new ProxyBusObject(alljoyn_busattachment_getalljoynproxyobj(_busAttachment));
				}
			}

			public ProxyBusObject AllJoynDebugObj
			{
				get
				{
					return new ProxyBusObject(alljoyn_busattachment_getalljoyndebugobj(_busAttachment));
				}
			}

			public string UniqueName
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_busattachment_getuniquename(_busAttachment));
				}
			}

			public string GlobalGUIDString
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_busattachment_getglobalguidstring(_busAttachment));
				}
			}

			public static uint Timestamp
			{
				get
				{
					return alljoyn_busattachment_gettimestamp();
				}
			}
			#endregion

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
			private extern static int alljoyn_busattachment_stop(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_createinterface(
				IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref IntPtr iface,
				int secure);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_start(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_connect(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string connectSpec);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_registerbuslistener(IntPtr bus, IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_unregisterbuslistener(IntPtr bus, IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_findadvertisedname(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string namePrefix);

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
			private extern static IntPtr alljoyn_busattachment_getinterface(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_joinsession(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string sessionHost,
				ushort sessionPort,
				IntPtr listener,
				ref uint sessionId,
				IntPtr opts);

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

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_enablepeersecurity(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string authMechanisms,
				IntPtr listener,
				[MarshalAs(UnmanagedType.LPStr)] string keyStoreFileName,
				int isShared);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_ispeersecurityenabled(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_isstarted(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_isstopping(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_isconnected(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static uint alljoyn_busattachment_gettimestamp();

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_createinterfacesfromxml(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string xml);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_busattachment_getinterfaces(IntPtr bus, IntPtr ifaces, UIntPtr numIfaces);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_deleteinterface(IntPtr bus, IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_disconnect(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string connectSpec);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getdbusproxyobj(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getalljoynproxyobj(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getalljoyndebugobj(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getuniquename(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busattachment_getglobalguidstring(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_registerkeystorelistener(IntPtr bus, IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_reloadkeystore(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_busattachment_clearkeystore(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_clearkeys(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string guid);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_setkeyexpiration(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string guid,
				uint timeout);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_getkeyexpiration(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string guid,
				ref uint timeout);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_addlogonentry(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string authMechanism,
				[MarshalAs(UnmanagedType.LPStr)] string userName,
				[MarshalAs(UnmanagedType.LPStr)] string password);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_addmatch(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string rule);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_removematch(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string rule);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_setsessionlistener(IntPtr bus, uint sessionId,
				IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_leavesession(IntPtr bus, uint sessionId);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_setlinktimeout(IntPtr bus, uint sessionid, ref uint linkTimeout);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_namehasowner(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref int hasOwner);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_getpeerguid(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				IntPtr guid, ref UIntPtr guidSz);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busattachment_setdaemondebug(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string module,
				uint level);
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

