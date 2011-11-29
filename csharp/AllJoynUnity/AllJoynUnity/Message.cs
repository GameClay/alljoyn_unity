using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class Message : IDisposable
		{
			public enum Type : int
			{
				Invalid = 0,
				MethodCall = 1,
				MethodReturn = 2,
				Error = 3,
				Signal = 4
			}

			public Message(BusAttachment bus)
			{
				_message = alljoyn_message_create(bus.UnmanagedPtr);
			}

			internal Message(IntPtr message)
			{
				_message = message;
				_isDisposed = true;
			}

			public MsgArg GetArg(int index)
			{
				IntPtr msgArgs = alljoyn_message_getarg(_message, (UIntPtr)index);
				return (msgArgs != IntPtr.Zero ? new MsgArg(msgArgs) : null);
			}

			public MsgArg this[int i]
			{
				get
				{
					return GetArg(i);
				}
			}

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
					alljoyn_message_destroy(_message);
					_message = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~Message()
			{
				Dispose(false);
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_message_create(IntPtr bus);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_message_destroy(IntPtr msg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_message_getarg(IntPtr msg, UIntPtr argN);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _message;
				}
			}
			#endregion

			#region Data
			IntPtr _message;
			bool _isDisposed = false;
			#endregion
		}
	}
}
