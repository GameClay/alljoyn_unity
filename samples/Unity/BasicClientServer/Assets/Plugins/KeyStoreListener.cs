using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public abstract class KeyStoreListener : IDisposable
		{
			public KeyStoreListener()
			{
				_loadRequest = new InternalLoadRequest(this._LoadRequest);
				_storeRequest = new InternalStoreRequest(this._StoreRequest);

				KeyStoreListenerCallbacks callbacks;
				callbacks.loadRequest = Marshal.GetFunctionPointerForDelegate(_loadRequest);
				callbacks.storeRequest = Marshal.GetFunctionPointerForDelegate(_storeRequest);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_keyStoreListener = alljoyn_keystorelistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
			}

			#region Abstract Methods
			public abstract QStatus LoadRequest(KeyStore store);
			public abstract QStatus StoreRequest(KeyStore store);
			#endregion

			#region Callbacks
			private int _LoadRequest(IntPtr context, IntPtr keyStore)
			{
				return LoadRequest(new KeyStore(keyStore, this.UnmanagedPtr));
			}

			private int _StoreRequest(IntPtr context, IntPtr keyStore)
			{
				return StoreRequest(new KeyStore(keyStore, this.UnmanagedPtr));
			}
			#endregion

			#region Delegates
			private delegate int InternalLoadRequest(IntPtr context, IntPtr keyStore);
			private delegate int InternalStoreRequest(IntPtr context, IntPtr keyStore);
			#endregion

			public class KeyStore
			{
				internal KeyStore(IntPtr keyStore, IntPtr keyStoreListener)
				{
					_keyStore = keyStore;
					_keyStoreListener = keyStoreListener;
				}

				public QStatus PutKeys(string source, string password)
				{
					return alljoyn_keystorelistener_putkeys(_keyStoreListener, _keyStore,
						source, password);
				}

				public string GetKeys()
				{
					int sinkSz = 512;
					byte[] sink;
					QStatus status;
					do
					{
						sinkSz *= 2;
						sink = new byte[sinkSz];
						GCHandle gch = GCHandle.Alloc(sink, GCHandleType.Pinned);
						status = alljoyn_keystorelistener_getkeys(_keyStoreListener, _keyStore,
							gch.AddrOfPinnedObject(), (UIntPtr)sinkSz);
						gch.Free();
					} while(status != QStatus.OK);

					return System.Text.ASCIIEncoding.ASCII.GetString(sink);
				}

				private IntPtr _keyStore;
				private IntPtr _keyStoreListener;
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_keystorelistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_keystorelistener_destroy(IntPtr listener);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_keystorelistener_putkeys(IntPtr listener, IntPtr keyStore,
				[MarshalAs(UnmanagedType.LPStr)] string source,
				[MarshalAs(UnmanagedType.LPStr)] string password);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_keystorelistener_getkeys(IntPtr listener, IntPtr keyStore,
				IntPtr sink, UIntPtr sink_sz);
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
					alljoyn_keystorelistener_destroy(_keyStoreListener);
					_keyStoreListener = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~KeyStoreListener()
			{
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct KeyStoreListenerCallbacks
			{
				public IntPtr loadRequest;
				public IntPtr storeRequest;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _keyStoreListener;
				}
			}
			#endregion

			#region Data
			IntPtr _keyStoreListener;
			bool _isDisposed = false;

			InternalLoadRequest _loadRequest;
			InternalStoreRequest _storeRequest;
			#endregion
		}
	}
}
