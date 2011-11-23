using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class MsgArgs : IDisposable
		{
			public MsgArgs(uint numArgs)
			{
				_msgArgs = alljoyn_msgargs_create((UIntPtr)numArgs);
			}
			
			internal MsgArgs(IntPtr msgArgs)
			{
				_msgArgs = msgArgs;
				_isDisposed = true;
			}
			
			QStatus Set(uint argOffset, ref uint numArgs, string signature, __arglist)
			{
				UIntPtr nArgs = (UIntPtr)numArgs;
				QStatus ret = alljoyn_msgargs_set(_msgArgs, (UIntPtr)argOffset, ref nArgs, signature, __arglist(1));
				numArgs = (uint)nArgs;
				return ret;
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
					alljoyn_msgargs_destroy(_msgArgs);
					_msgArgs = IntPtr.Zero;
				}
				_isDisposed = true;
			}
			
			~MsgArgs()
			{
				Dispose(false);
			}
			#endregion
			
			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgargs_create(UIntPtr numArgs); // UIntPtr must map to the same size as size_t, not a typo
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_msgargs_destroy(IntPtr arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, __arglist);
			#endregion
			
			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _msgArgs;
				}
			}
			#endregion
			
			#region Data
			IntPtr _msgArgs;
			bool _isDisposed = false;
			#endregion
		}
	}
}
