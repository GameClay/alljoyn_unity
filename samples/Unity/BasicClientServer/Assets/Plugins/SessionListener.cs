using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionListener : IDisposable
		{
			public SessionListener()
			{
				_sessionLost = new InternalSessionLost(_SessionLost);
				_sessionMemberAdded = new InternalSessionMemberAdded(_SessionMemberAdded);
				_sessionMemberRemoved = new InternalSessionMemberRemoved(_SessionMemberRemoved);

#if UNITY_ANDROID
				_sessionListener = alljoyn_unitysessionlistener_create(this, _sessionLost,
					_sessionMemberAdded, _sessionMemberRemoved);
#else
				SessionListenerCallbacks callbacks;
				callbacks.sessionLost = Marshal.GetFunctionPointerForDelegate(_sessionLost);
				callbacks.sessionMemberAdded = Marshal.GetFunctionPointerForDelegate(_sessionMemberAdded);
				callbacks.sessionMemberRemoved = Marshal.GetFunctionPointerForDelegate(_sessionMemberRemoved);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionListener = alljoyn_sessionlistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
#endif
			}

			#region Virtual Methods
			protected virtual void SessionLost(uint sessionId) {}
			protected virtual void SessionMemberAdded(uint sessionId, string uniqueName) {}
			protected virtual void SessionMemberRemoved(uint sessionId, string uniqueName) {}
			#endregion

			#region Callbacks
#if UNITY_ANDROID
			private void _SessionLost(SessionListener context, uint sessionId)
			{
				context.SessionLost(sessionId);
			}

			private void _SessionMemberAdded(SessionListener context, uint sessionId, IntPtr uniqueName)
			{
				context.SessionMemberAdded(sessionId, Marshal.PtrToStringAuto(uniqueName));
			}

			private void _SessionMemberRemoved(SessionListener context, uint sessionId, IntPtr uniqueName)
			{
				context.SessionMemberRemoved(sessionId, Marshal.PtrToStringAuto(uniqueName));
			}
#else
			private void _SessionLost(IntPtr context, uint sessionId)
			{
				SessionLost(sessionId);
			}

			private void _SessionMemberAdded(IntPtr context, uint sessionId, IntPtr uniqueName)
			{
				SessionMemberAdded(sessionId, Marshal.PtrToStringAuto(uniqueName));
			}

			private void _SessionMemberRemoved(IntPtr context, uint sessionId, IntPtr uniqueName)
			{
				SessionMemberRemoved(sessionId, Marshal.PtrToStringAuto(uniqueName));
			}
#endif
			#endregion

			#region Delegates
#if UNITY_ANDROID
			private delegate void InternalSessionLost(SessionListener context, uint sessionId);
			private delegate void InternalSessionMemberAdded(SessionListener context, uint sessionId, IntPtr uniqueName);
			private delegate void InternalSessionMemberRemoved(SessionListener context, uint sessionId, IntPtr uniqueName);
#else
			private delegate void InternalSessionLost(IntPtr context, uint sessionId);
			private delegate void InternalSessionMemberAdded(IntPtr context, uint sessionId, IntPtr uniqueName);
			private delegate void InternalSessionMemberRemoved(IntPtr context, uint sessionId, IntPtr uniqueName);
#endif
			#endregion

			#region DLL Imports
#if UNITY_ANDROID
			[MethodImplAttribute(MethodImplOptions.InternalCall)]
			private extern static IntPtr alljoyn_unitysessionlistener_create(
				object thisObj,
				object sessionLostDelegate,
				object sessionMemberAddedDelegate,
				object sessionMemberRemovedDelegate);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_unitysessionlistener_destroy(IntPtr listener);
#else
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionlistener_destroy(IntPtr listener);
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
					UnityEngine.Debug.Log("TRASHING _sessionListener: " + _sessionListener);
#if UNITY_ANDROID
					alljoyn_unitysessionlistener_destroy(_sessionListener);
#else
					alljoyn_sessionlistener_destroy(_sessionListener);
#endif
					_sessionListener = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~SessionListener()
			{
				Dispose(false);
			}
			#endregion

			#region Structs
			private struct SessionListenerCallbacks
			{
				public IntPtr sessionLost;
				public IntPtr sessionMemberAdded;
				public IntPtr sessionMemberRemoved;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _sessionListener;
				}
			}
			#endregion

			#region Data
			IntPtr _sessionListener;
			bool _isDisposed = false;

			InternalSessionLost _sessionLost;
			InternalSessionMemberAdded _sessionMemberAdded;
			InternalSessionMemberRemoved _sessionMemberRemoved;
			#endregion
		}
	}
}
