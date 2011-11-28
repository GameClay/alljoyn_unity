using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class ProxyBusObject : IDisposable
		{
			public ProxyBusObject(BusAttachment bus, string service, string path, uint sessionId)
			{
				_proxyBusObject = alljoyn_proxybusobject_create(bus.UnmanagedPtr, service, path, sessionId);
			}
			
			public QStatus AddInterface(InterfaceDescription iface)
			{
				return alljoyn_proxybusobject_addinterface(_proxyBusObject, iface.UnmanagedPtr);
			}

			public QStatus MethodCallSynch(string ifaceName, string methodName, MsgArgs args, Message replyMsg,
				uint timeout, byte flags)
			{
				return alljoyn_proxybusobject_methodcall_synch(_proxyBusObject, ifaceName, methodName, args.UnmanagedPtr,
					(UIntPtr)args.Length, replyMsg.UnmanagedPtr, timeout, flags);
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_proxybusobject_create(IntPtr bus,
				[MarshalAs(UnmanagedType.LPStr)] string service,
				[MarshalAs(UnmanagedType.LPStr)] string path,
				uint sessionId);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_proxybusobject_destroy(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_proxybusobject_addinterface(IntPtr bus, IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_proxybusobject_methodcall_synch(IntPtr obj,
				[MarshalAs(UnmanagedType.LPStr)] string ifaceName,
				[MarshalAs(UnmanagedType.LPStr)] string methodName,
				IntPtr args,
				UIntPtr numArgs,
				IntPtr replyMsg,
				uint timeout,
				byte flags);
			#endregion

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
					alljoyn_proxybusobject_destroy(_proxyBusObject);
					_proxyBusObject = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~ProxyBusObject()
			{
				Dispose(false);
			}
			#endregion

			#region Data
			IntPtr _proxyBusObject;
			bool _isDisposed = false;
			#endregion
		}
	}
}
