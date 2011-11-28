using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public abstract class BusObject : IDisposable
		{
			public BusObject(string path, bool isPlaceholder)
			{
				// Can't let the GC free these delegates so they must be members
				_propertyGet = new InternalPropertyGetEventHandler(_PropertyGet);
				_propertySet = new InternalPropertySetEventHandler(_PropertySet);
				_objectRegistered = new InternalObjectRegisteredEventHandler(_ObjectRegistered);
				_objectUnregistered = new InternalObjectUnregisteredEventHandler(_ObjectUnregistered);

				BusObjectCallbacks callbacks;
				callbacks.property_get = Marshal.GetFunctionPointerForDelegate(_propertyGet);
				callbacks.property_set = Marshal.GetFunctionPointerForDelegate(_propertySet);
				callbacks.object_registered = Marshal.GetFunctionPointerForDelegate(_objectRegistered);
				callbacks.object_unregistered = Marshal.GetFunctionPointerForDelegate(_objectUnregistered);

				// Insert this object into the static hashtable to re-map native->managed
				if(_sObjects == null) _sObjects = new Dictionary<IntPtr, BusObject>();
				if(_sRndSource == null) _sRndSource = new Random();

				do { _hashId = (IntPtr)_sRndSource.Next(); } while(_sObjects.ContainsKey(_hashId));
				_sObjects.Add(_hashId, this);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busObject = alljoyn_busobject_create(path, isPlaceholder ? 1 : 0, gch.AddrOfPinnedObject(), _hashId);
				gch.Free();
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
			private delegate void InternalPropertyGetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
			private delegate void InternalPropertySetEventHandler(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val);
			private delegate void InternalObjectRegisteredEventHandler(IntPtr context);
			private delegate void InternalObjectUnregisteredEventHandler(IntPtr context);
			#endregion

			#region Abstract Methods
			protected abstract void OnPropertyGet(string ifcName, string propName, MsgArg val);
			protected abstract void OnPropertySet(string ifcName, string propName, MsgArg val);
			protected abstract void OnObjectRegistered();
			protected abstract void OnObjectUnregistered();
			#endregion

			#region Callbacks
			private static void _PropertyGet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
				BusObject bus = _sObjects[context];
				bus.OnPropertyGet(Marshal.PtrToStringAuto(ifcName),
					Marshal.PtrToStringAuto(propName), new MsgArg(val));
			}

			private static void _PropertySet(IntPtr context, IntPtr ifcName, IntPtr propName, IntPtr val)
			{
				BusObject bus = _sObjects[context];
				bus.OnPropertySet(Marshal.PtrToStringAuto(ifcName),
					Marshal.PtrToStringAuto(propName), new MsgArg(val));
			}

			private static void _ObjectRegistered(IntPtr context)
			{
				BusObject bus = _sObjects[context];
				bus.OnObjectRegistered();
			}

			private static void _ObjectUnregistered(IntPtr context)
			{
				BusObject bus = _sObjects[context];
				bus.OnObjectUnregistered();
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_create(
				[MarshalAs(UnmanagedType.LPStr)] string path,
				int isPlaceholder,
				IntPtr callbacks_in,
				IntPtr context_in);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_destroy(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_busobject_getpath(IntPtr bus);
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
					alljoyn_busobject_destroy(_busObject);
					_busObject = IntPtr.Zero;
					_sObjects.Remove(_hashId);
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
			IntPtr _hashId;

			InternalPropertyGetEventHandler _propertyGet;
			InternalPropertySetEventHandler _propertySet;
			InternalObjectRegisteredEventHandler _objectRegistered;
			InternalObjectUnregisteredEventHandler _objectUnregistered;

			static Dictionary<IntPtr, BusObject> _sObjects;
			static Random _sRndSource;
			#endregion
		}
	}
}
