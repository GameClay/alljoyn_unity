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
				_argArray = new MsgArg[numArgs];
				for(uint i = 0; i < numArgs; i++)
				{
					_argArray[i] = new MsgArg(this, i);
				}
			}
			
			public int Length
			{
				get
				{
					return _argArray.Length;
				}
			}
			
			public MsgArg this[int i]
			{
				get
				{
					return _argArray[i];
				}
				set
				{
					MsgArg arg = value as MsgArg;
					if(arg != null)
					{
						if(arg._setValue != null)
						{
							_argArray[i].Set(arg._setValue);
						}
					}
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
			MsgArg[] _argArray;
			bool _isDisposed = false;
			#endregion
		}
	}
}
