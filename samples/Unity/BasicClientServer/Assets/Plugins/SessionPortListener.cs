using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionPortListener : IDisposable
		{
			public SessionPortListener()
			{
				_acceptSessionJoiner = new InternalAcceptSessionJoiner(_AcceptSessionJoiner);
				_sessionJoined = new InternalSessionJoined(_SessionJoined);

#if UNITY_ANDROID
				_sessionPortListener = alljoyn_unitysessionportlistener_create(this,
					_acceptSessionJoiner, _sessionJoined);
#else
				SessionPortListenerCallbacks callbacks;
				callbacks.acceptSessionJoiner = Marshal.GetFunctionPointerForDelegate(_acceptSessionJoiner);
				callbacks.sessionJoined = Marshal.GetFunctionPointerForDelegate(_sessionJoined);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionPortListener = alljoyn_sessionportlistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
#endif
			}

			#region Virtual Methods
			protected virtual bool AcceptSessionJoiner(ushort sessionPort, string joiner, SessionOpts opts)
			{
				return false;
			}

			protected virtual void SessionJoined(ushort sessionPort, uint sessionId, string joiner) {}
			#endregion

			#region Callbacks
#if UNITY_ANDROID
			private static int _AcceptSessionJoiner(SessionPortListener context, ushort sessionPort, IntPtr joiner, IntPtr opts)
			{
				return (context.AcceptSessionJoiner(sessionPort, Marshal.PtrToStringAuto(joiner), new SessionOpts(opts)) ? 1 : 0);
			}

			private static void _SessionJoined(SessionPortListener context, ushort sessionPort, uint sessionId, IntPtr joiner)
			{
				context. SessionJoined(sessionPort, sessionId, Marshal.PtrToStringAuto(joiner));
			}
#else
			private int _AcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts)
			{
				return (AcceptSessionJoiner(sessionPort, Marshal.PtrToStringAuto(joiner), new SessionOpts(opts)) ? 1 : 0);
			}

			private void _SessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner)
			{
				SessionJoined(sessionPort, sessionId, Marshal.PtrToStringAuto(joiner));
			}
#endif
			#endregion

			#region Delegates
#if UNITY_ANDROID
			private delegate int InternalAcceptSessionJoiner(SessionPortListener context, ushort sessionPort, IntPtr joiner, IntPtr opts);
			private delegate void InternalSessionJoined(SessionPortListener context, ushort sessionPort, uint sessionId, IntPtr joiner);
#else
			private delegate int InternalAcceptSessionJoiner(IntPtr context, ushort sessionPort, IntPtr joiner, IntPtr opts);
			private delegate void InternalSessionJoined(IntPtr context, ushort sessionPort, uint sessionId, IntPtr joiner);
#endif
			#endregion

			#region DLL Imports
#if UNITY_ANDROID
			[MethodImplAttribute(MethodImplOptions.InternalCall)]
			private extern static IntPtr alljoyn_unitysessionportlistener_create(
				object thisObj, object accept_session_joiner, object session_joined);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_unitysessionportlistener_destroy(IntPtr listener);
#else
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionportlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionportlistener_destroy(IntPtr listener);
#endif
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
					UnityEngine.Debug.Log("TRASHING _sessionPortListener: " + _sessionPortListener);
#if UNITY_ANDROID
					alljoyn_unitysessionportlistener_destroy(_sessionPortListener);
#else
					alljoyn_sessionportlistener_destroy(_sessionPortListener);
#endif
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
