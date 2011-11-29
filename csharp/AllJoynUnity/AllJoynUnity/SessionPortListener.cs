using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionPortListener : IDisposable
		{
			public SessionPortListener()
			{
				_acceptSessionJoiner = new InternalAcceptSessionJoiner(this._AcceptSessionJoiner);
				_sessionJoined = new InternalSessionJoined(this._SessionJoined);

				SessionPortListenerCallbacks callbacks;
				callbacks.acceptSessionJoiner = Marshal.GetFunctionPointerForDelegate(_acceptSessionJoiner);
				callbacks.sessionJoined = Marshal.GetFunctionPointerForDelegate(_sessionJoined);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionPortListener = alljoyn_sessionportlistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
			}

			#region Virtual Methods
			protected virtual bool AcceptSessionJoiner(ushort sessionPort, string joiner, SessionOpts opts)
			{
				return false;
			}

			protected virtual void SessionJoined(ushort sessionPort, uint sessionId, string joiner)
			{
			}
			#endregion

			#region Callbacks
			private int _AcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts)
			{
				return (AcceptSessionJoiner(sessionPort, Marshal.PtrToStringAuto(joiner), new SessionOpts(opts)) ? 1 : 0);
			}

			private void _SessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner)
			{
				SessionJoined(sessionPort, sessionId, Marshal.PtrToStringAuto(joiner));
			}
			#endregion

			#region Delegates
			private delegate int InternalAcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts);
			private delegate void InternalSessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner);
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionportlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionportlistener_destroy(IntPtr listener);
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
					alljoyn_sessionportlistener_destroy(_sessionPortListener);
					_sessionPortListener = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~SessionPortListener()
			{
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct SessionPortListenerCallbacks
			{
				public IntPtr acceptSessionJoiner;
				public IntPtr sessionJoined;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _sessionPortListener;
				}
			}
			#endregion

			#region Data
			IntPtr _sessionPortListener;
			bool _isDisposed = false;

			InternalAcceptSessionJoiner _acceptSessionJoiner;
			InternalSessionJoined _sessionJoined;
			#endregion
		}
	}
}
