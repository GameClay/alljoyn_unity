using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class BusListener : IDisposable
		{
			public BusListener()
			{
				// Can't let the GC free these delegates so they must be members
				_listenerRegistered = new InternalListenerRegisteredDelegate(_ListenerRegistered);
				_listenerUnregistered = new InternalListenerUnregisteredDelegate(_ListenerUnregistered);
				_foundAdvertisedName = new InternalFoundAdvertisedNameDelegate(_FoundAdvertisedName);
				_lostAdvertisedName = new InternalLostAdvertisedNameDelegate(_LostAdvertisedName);
				_nameOwnerChanged = new InternalNameOwnerChangedDelegate(_NameOwnerChanged);
				_busStopping = new InternalBusStoppingDelegate(_BusStopping);
				_busDisconnected = new InternalBusDisconnectedDelegate(_BusDisconnected);

				BusListenerCallbacks callbacks;
				callbacks.listenerRegistered = Marshal.GetFunctionPointerForDelegate(_listenerRegistered);
				callbacks.listenerUnregistered = Marshal.GetFunctionPointerForDelegate(_listenerUnregistered);
				callbacks.foundAdvertisedName = Marshal.GetFunctionPointerForDelegate(_foundAdvertisedName);
				callbacks.lostAdvertisedName = Marshal.GetFunctionPointerForDelegate(_lostAdvertisedName);
				callbacks.nameOwnerChanged = Marshal.GetFunctionPointerForDelegate(_nameOwnerChanged);
				callbacks.busStopping = Marshal.GetFunctionPointerForDelegate(_busStopping);
				callbacks.busDisconnected = Marshal.GetFunctionPointerForDelegate(_busDisconnected);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busListener = alljoyn_buslistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
			}

			#region Virtual Methods
			protected virtual void ListenerRegistered(BusAttachment busAttachment){}

			protected virtual void ListenerUnregistered() {}

			protected virtual void FoundAdvertisedName(string name, TransportMask transport, string namePrefix) {}

			protected virtual void LostAdvertisedName(string name, TransportMask transport, string namePrefix) {}

			protected virtual void NameOwnerChanged(string busName, string previousOwner, string newOwner) {}

			protected virtual void BusStopping() {}

			protected virtual void BusDisconnected() {}
			#endregion

			#region Delegates
			private delegate void InternalListenerRegisteredDelegate(IntPtr context, IntPtr bus);
			private delegate void InternalListenerUnregisteredDelegate(IntPtr context);
			private delegate void InternalFoundAdvertisedNameDelegate(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix);
			private delegate void InternalLostAdvertisedNameDelegate(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix);
			private delegate void InternalNameOwnerChangedDelegate(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner);
			private delegate void InternalBusStoppingDelegate(IntPtr context);
			private delegate void InternalBusDisconnectedDelegate(IntPtr context);
			#endregion

			#region Callbacks
			private void _ListenerRegistered(IntPtr context, IntPtr bus)
			{
				_registeredBus = BusAttachment.MapBusAttachment(bus);
				ListenerRegistered(_registeredBus);
			}

			private void _ListenerUnregistered(IntPtr context)
			{
				ListenerUnregistered();
				_registeredBus = null;
			}

			private void _FoundAdvertisedName(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix)
			{
				FoundAdvertisedName(Marshal.PtrToStringAuto(name), (TransportMask)transport,
							Marshal.PtrToStringAuto(namePrefix));
			}

			private void _LostAdvertisedName(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix)
			{
				LostAdvertisedName(Marshal.PtrToStringAuto(name), (TransportMask)transport,
							Marshal.PtrToStringAuto(namePrefix));
			}

			private void _NameOwnerChanged(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner)
			{
				NameOwnerChanged(Marshal.PtrToStringAuto(busName), Marshal.PtrToStringAuto(previousOwner),
							Marshal.PtrToStringAuto(newOwner));
			}

			private void _BusStopping(IntPtr context)
			{
				BusStopping();
			}

			private void _BusDisconnected(IntPtr context)
			{
				BusDisconnected();
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_buslistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_buslistener_destroy(IntPtr listener);
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
					// Dispose of BusAttachment before listeners
					if(_registeredBus != null)
					{
						_registeredBus.Dispose();
					}
					
					Thread destroyThread = new Thread((object o) => { alljoyn_buslistener_destroy(_busListener); });
					while(destroyThread.IsAlive)
					{
						AllJoyn.TriggerCallbacks();
						Thread.Sleep(0);
					}
					
					_busListener = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~BusListener()
			{
				Dispose(false);
			}
			#endregion

			#region Structs
			[StructLayout(LayoutKind.Sequential)]
			private struct BusListenerCallbacks
			{
				public IntPtr listenerRegistered;
				public IntPtr listenerUnregistered;
				public IntPtr foundAdvertisedName;
				public IntPtr lostAdvertisedName;
				public IntPtr nameOwnerChanged;
				public IntPtr busStopping;
				public IntPtr busDisconnected;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busListener;
				}
			}
			#endregion

			#region Data
			IntPtr _busListener;
			bool _isDisposed = false;
			BusAttachment _registeredBus;

			InternalListenerRegisteredDelegate _listenerRegistered;
			InternalListenerUnregisteredDelegate _listenerUnregistered;
			InternalFoundAdvertisedNameDelegate _foundAdvertisedName;
			InternalLostAdvertisedNameDelegate _lostAdvertisedName;
			InternalNameOwnerChangedDelegate _nameOwnerChanged;
			InternalBusStoppingDelegate _busStopping;
			InternalBusDisconnectedDelegate _busDisconnected;
			#endregion
		}
	}
}

