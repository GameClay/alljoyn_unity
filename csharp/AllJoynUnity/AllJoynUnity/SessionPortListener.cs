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
			}

			#region Callbacks

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
			#endregion
		}
	}
}
