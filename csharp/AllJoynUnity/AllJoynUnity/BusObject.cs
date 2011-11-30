using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class BusObject : IDisposable
		{
			public BusObject(BusAttachment bus, string path, bool isPlaceholder)
			{
				// Can't let the GC free these delegates so they must be members
				_propertyGet = new InternalPropertyGetEventHandler(this._PropertyGet);
				_propertySet = new InternalPropertySetEventHandler(this._PropertySet);
				_objectRegistered = new InternalObjectRegisteredEventHandler(this._ObjectRegistered);
				_objectUnregistered = new InternalObjectUnregisteredEventHandler(this._ObjectUnregistered);

				// Ref holder for method handler internal delegates
				_methodHandlerDelegateRefHolder = new List<InternalMethodHandler>();

				BusObjectCallbacks callbacks;
				callbacks.property_get = Marshal.GetFunctionPointerForDelegate(_propertyGet);
				callbacks.property_set = Marshal.GetFunctionPointerForDelegate(_propertySet);
				callbacks.object_registered = Marshal.GetFunctionPointerForDelegate(_objectRegistered);
				callbacks.object_unregistered = Marshal.GetFunctionPointerForDelegate(_objectUnregistered);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busObject = alljoyn_busobject_create(bus.UnmanagedPtr, path, isPlaceholder ? 1 : 0, gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
			}

			public QStatus AddInterface(InterfaceDescription iface)
			{
				return alljoyn_busobject_addinterface(_busObject, iface.UnmanagedPtr);
			}

			public QStatus AddMethodHandler(InterfaceDescription.Member member, MethodHandler handler)
			{
				InternalMethodHandler internalMethodHandler = (IntPtr bus, IntPtr m, IntPtr msg) =>
				{
					MethodHandler h = handler;
					h(new InterfaceDescription.Member(m), new Message(msg));
				};
				_methodHandlerDelegateRefHolder.Add(internalMethodHandler);

				GCHandle membGch = GCHandle.Alloc(member._member, GCHandleType.Pinned);

				MethodEntry entry;
				entry.member = membGch.AddrOfPinnedObject();
				entry.method_handler = Marshal.GetFunctionPointerForDelegate(internalMethodHandler);

				GCHandle gch = GCHandle.Alloc(entry, GCHandleType.Pinned);
				QStatus ret = alljoyn_busobject_addmethodhandlers(_busObject, gch.AddrOfPinnedObject(), (UIntPtr)1);
				gch.Free();
				membGch.Free();

				return ret;
			}

			protected QStatus MethodReply(Message message, MsgArgs args)
			{
				return alljoyn_busobject_methodreply_args(_busObject, message.UnmanagedPtr, args.UnmanagedPtr,
					(UIntPtr)args.Length);
			}

			protected QStatus MethodReply(Message message, string error, string errorMessage)
			{
				return alljoyn_busobject_methodreply_err(_busObject, message.UnmanagedPtr, error,
					errorMessage);
			}

			protected QStatus MethodReply(Message message, QStatus status)
			{
				return alljoyn_busobject_methodreply_status(_busObject, message.UnmanagedPtr, status.value);
			}

			#region Properties
			public string Path
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_busobject_getpath(_busObject));
				}
			}

			public string Name
			{
				get
				{
					return ""; // TODO
				}
			}
			#endregion

			#region Delegates
			public delegate void MethodHandler(InterfaceDescription.Member member, Message message);

			private delegate void InternalMethodHandler(IntPtr bus, IntPtr member, IntPtr message);
			private delegate void InternalPropertyGetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
			private delegate void InternalPropertySetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
			private delegate void InternalObjectRegisteredEventHandler(IntPtr context);
			private delegate void InternalObjectUnregisteredEventHandler(IntPtr context);
			#endregion

			#region Virtual Methods
			protected virtual void OnPropertyGet(string ifcName, string propName, MsgArg val)
			{
			}

			protected virtual void OnPropertySet(string ifcName, string propName, MsgArg val)
			{
			}

			protected virtual void OnObjectRegistered()
			{
			}

			protected virtual void OnObjectUnregistered()
			{
			}
			#endregion

			#region Callbacks
			private void _PropertyGet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
				OnPropertyGet(Marshal.PtrToStringAuto(ifcName),
					Marshal.PtrToStringAuto(propName), new MsgArg(val));
			}

			private void _PropertySet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
				OnPropertySet(Marshal.PtrToStringAuto(ifcName),
					Marshal.PtrToStringAuto(propName), new MsgArg(val));
			}

			private void _ObjectRegistered(IntPtr context)
			{
				OnObjectRegistered();
			}

			private void _ObjectUnregistered(IntPtr context)
			{
				OnObjectUnregistered();
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_create(
				IntPtr busAttachment,
				[MarshalAs(UnmanagedType.LPStr)] string path,
				int isPlaceholder,
				IntPtr callbacks_in,
				IntPtr context_in);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_destroy(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_getpath(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_addinterface(IntPtr bus, IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_addmethodhandlers(IntPtr bus,
				IntPtr entries, UIntPtr numEntries);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_args(IntPtr bus,
				IntPtr msg, IntPtr msgArgs, UIntPtr numArgs);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_err(IntPtr bus, IntPtr msg,
				[MarshalAs(UnmanagedType.LPStr)] string error,
				[MarshalAs(UnmanagedType.LPStr)] string errorMsg);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_busobject_methodreply_status(IntPtr bus, IntPtr msg, int status);
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
					Thread destroyThread = new Thread((object o) => { alljoyn_busobject_destroy(_busObject); });
					while(destroyThread.IsAlive)
					{
						AllJoyn.TriggerCallbacks();
						Thread.Sleep(0);
					}
					_busObject = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~BusObject()
			{
				Dispose(false);
			}
			#endregion

			#region Structs
			[StructLayout(LayoutKind.Sequential)]
			private struct BusObjectCallbacks
			{
				public IntPtr property_get;
				public IntPtr property_set;
				public IntPtr object_registered;
				public IntPtr object_unregistered;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct MethodEntry
			{
				public IntPtr member;
				public IntPtr method_handler;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busObject;
				}
			}
			#endregion

			#region Data
			IntPtr _busObject;
			bool _isDisposed = false;

			InternalPropertyGetEventHandler _propertyGet;
			InternalPropertySetEventHandler _propertySet;
			InternalObjectRegisteredEventHandler _objectRegistered;
			InternalObjectUnregisteredEventHandler _objectUnregistered;

			List<InternalMethodHandler> _methodHandlerDelegateRefHolder;
			#endregion
		}
	}
}
