using System;
using System.Collections.Generic;
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

				// Insert this object into the static hashtable to re-map native->managed
				if(_sListeners == null) _sListeners = new Dictionary<IntPtr, BusListener>();
				if(_sRndSource == null) _sRndSource = new Random();
				
				do { _hashId = (IntPtr)_sRndSource.Next(); } while(_sListeners.ContainsKey(_hashId));
				_sListeners.Add(_hashId, this);
				
				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_busListener = alljoyn_buslistener_create(gch.AddrOfPinnedObject(), _hashId);
				gch.Free();
			}
			
			#region Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _busListener;
				}
			}
			#endregion
			
			#region Events
			public event ListenerRegisteredEventHandler ListenerRegistered;
			public event ListenerUnregisteredEventHandler ListenerUnregistered;
			public event FoundAdvertisedNameEventHandler FoundAdvertisedName;
			public event LostAdvertisedNameEventHandler LostAdvertisedName;
			public event NameOwnerChangedEventHandler NameOwnerChanged;
			public event BusStoppingEventHandler BusStopping;
			public event BusDisconnectedEventHandler BusDisconnected;
			#endregion
			
			#region EventArgs types
			public class ListenerRegisteredEventArgs : EventArgs
			{
				public ListenerRegisteredEventArgs(BusAttachment bus)
				{
					this.bus = bus;
				}
				
				public BusAttachment bus;
			}
			public class AdvertisedNameEventArgs : EventArgs
			{
				public AdvertisedNameEventArgs(string name, TransportMask transport, string namePrefix)
				{
					this.name = name;
					this.transport = transport;
					this.namePrefix = namePrefix;
				}
				
				public string name;
				public TransportMask transport;
				public string namePrefix;
			}
			
			public class NameOwnerChangedEventArgs : EventArgs
			{
				public NameOwnerChangedEventArgs(string busName, string previousOwner, string newOwner)
				{
					this.busName = busName;
					this.previousOwner = previousOwner;
					this.newOwner = newOwner;
				}
				
				public string busName;
				public string previousOwner;
				public string newOwner;
			}
			#endregion
			
			#region Delegates
			public delegate void ListenerRegisteredEventHandler(object sender, ListenerRegisteredEventArgs ea);
			public delegate void ListenerUnregisteredEventHandler(object sender);
			public delegate void FoundAdvertisedNameEventHandler(object sender, AdvertisedNameEventArgs ea);
			public delegate void LostAdvertisedNameEventHandler(object sender, AdvertisedNameEventArgs ea);
			public delegate void NameOwnerChangedEventHandler(object sender, NameOwnerChangedEventArgs ea);
			public delegate void BusStoppingEventHandler(object sender);
			public delegate void BusDisconnectedEventHandler(object sender);
			
			private delegate void InternalListenerRegisteredDelegate(IntPtr context, IntPtr bus);
			private delegate void InternalListenerUnregisteredDelegate(IntPtr context);
			private delegate void InternalFoundAdvertisedNameDelegate(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix);
			private delegate void InternalLostAdvertisedNameDelegate(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix);
			private delegate void InternalNameOwnerChangedDelegate(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner);
			private delegate void InternalBusStoppingDelegate(IntPtr context);
			private delegate void InternalBusDisconnectedDelegate(IntPtr context);
			#endregion
			
			#region Callbacks
			private static void _ListenerRegistered(IntPtr context, IntPtr bus)
			{
				BusListener listener = _sListeners[context];
				if(listener.ListenerRegistered != null)
					listener.ListenerRegistered(listener, new ListenerRegisteredEventArgs(null)); // TODO: Remap
			}
			
			private static void _ListenerUnregistered(IntPtr context)
			{
				BusListener listener = _sListeners[context];
				if(listener.ListenerUnregistered != null)
					listener.ListenerUnregistered(listener);
			}
			
			private static void _FoundAdvertisedName(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix)
			{
				BusListener listener = _sListeners[context];
				if(listener.FoundAdvertisedName != null)
					listener.FoundAdvertisedName(listener,
						new AdvertisedNameEventArgs(Marshal.PtrToStringAuto(name), (TransportMask)transport,
							Marshal.PtrToStringAuto(namePrefix)));
			}
			
			private static void _LostAdvertisedName(IntPtr context, IntPtr name, ushort transport, IntPtr namePrefix)
			{
				BusListener listener = _sListeners[context];
				if(listener.LostAdvertisedName != null)
					listener.LostAdvertisedName(listener,
						new AdvertisedNameEventArgs(Marshal.PtrToStringAuto(name), (TransportMask)transport,
							Marshal.PtrToStringAuto(namePrefix)));
			}
			
			private static void _NameOwnerChanged(IntPtr context, IntPtr busName, IntPtr previousOwner, IntPtr newOwner)
			{
				BusListener listener = _sListeners[context];
				if(listener.NameOwnerChanged != null)
					listener.NameOwnerChanged(listener,
						new NameOwnerChangedEventArgs(Marshal.PtrToStringAuto(busName), Marshal.PtrToStringAuto(previousOwner),
							Marshal.PtrToStringAuto(newOwner)));
			}
			
			private static void _BusStopping(IntPtr context)
			{
				BusListener listener = _sListeners[context];
				if(listener.BusStopping != null)
					listener.BusStopping(listener);
			}
			
			private static void _BusDisconnected(IntPtr context)
			{
				BusListener listener = _sListeners[context];
				if(listener.BusDisconnected != null)
					listener.BusDisconnected(listener);
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
					alljoyn_buslistener_destroy(_busListener);
					_busListener = IntPtr.Zero;
					_sListeners.Remove(_hashId);
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
			
			#region Data
			IntPtr _busListener;
			bool _isDisposed = false;
			IntPtr _hashId;
			
			InternalListenerRegisteredDelegate _listenerRegistered;
			InternalListenerUnregisteredDelegate _listenerUnregistered;
			InternalFoundAdvertisedNameDelegate _foundAdvertisedName;
			InternalLostAdvertisedNameDelegate _lostAdvertisedName;
			InternalNameOwnerChangedDelegate _nameOwnerChanged;
			InternalBusStoppingDelegate _busStopping;
			InternalBusDisconnectedDelegate _busDisconnected;
			
			static Dictionary<IntPtr, BusListener> _sListeners;
			static Random _sRndSource;
			#endregion
		}
	}
}

