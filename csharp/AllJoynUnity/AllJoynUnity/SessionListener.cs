using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionListener : IDisposable
		{
			public SessionListener()
			{
				_sessionLost = new InternalSessionLost(this._SessionLost);
				_sessionMemberAdded = new InternalSessionMemberAdded(this._SessionMemberAdded);
				_sessionMemberRemoved = new InternalSessionMemberRemoved(this._SessionMemberRemoved);

				SessionListenerCallbacks callbacks;
				callbacks.sessionLost = Marshal.GetFunctionPointerForDelegate(_sessionLost);
				callbacks.sessionMemberAdded = Marshal.GetFunctionPointerForDelegate(_sessionMemberAdded);
				callbacks.sessionMemberRemoved = Marshal.GetFunctionPointerForDelegate(_sessionMemberRemoved);

				GCHandle gch = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
				_sessionListener = alljoyn_sessionlistener_create(gch.AddrOfPinnedObject(), IntPtr.Zero);
				gch.Free();
			}

			#region Virtual Methods
			protected virtual void SessionLost(uint sessionId)
			{
			}

			protected virtual void SessionMemberAdded(uint sessionId, string uniqueName)
			{
			}

			protected virtual void SessionMemberRemoved(uint sessionId, string uniqueName)
			{
			}
			#endregion

			#region Callbacks
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
			#endregion

			#region Delegates
			private delegate void InternalSessionLost(IntPtr context, uint sessionId);
			private delegate void InternalSessionMemberAdded(IntPtr context, uint sessionId, IntPtr uniqueName);
			private delegate void InternalSessionMemberRemoved(IntPtr context, uint sessionId, IntPtr uniqueName);
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_sessionlistener_create(
				IntPtr callbacks,
				IntPtr context);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_sessionlistener_destroy(IntPtr listener);
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
					alljoyn_sessionlistener_destroy(_sessionListener);
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
