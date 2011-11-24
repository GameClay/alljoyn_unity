using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class MsgArg : IDisposable
		{
			internal MsgArg(MsgArgs owner, uint index)
			{
				_msgArgs = owner.UnmanagedPtr;
				_index = index;
			}
			
			internal MsgArg(IntPtr msgArgs)
			{
				_msgArgs = msgArgs;
			}
			
			internal MsgArg(object val)
			{
				_msgArgs = IntPtr.Zero;
				_index = 0;
				_setValue = val;
			}
			
			public static implicit operator MsgArg(string arg)
			{
				return new MsgArg(arg);
			}
			
			public static implicit operator byte(MsgArg arg)
			{
				return alljoyn_msgargs_as_uint8(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator bool(MsgArg arg)
			{
				return (alljoyn_msgargs_as_bool(arg._msgArgs, (UIntPtr)arg._index) == 1 ? true : false);
			}
	
			public static implicit operator short(MsgArg arg)
			{
				return alljoyn_msgargs_as_int16(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator ushort(MsgArg arg)
			{
				return alljoyn_msgargs_as_uint16(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator int(MsgArg arg)
			{
				return alljoyn_msgargs_as_int32(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator uint(MsgArg arg)
			{
				return alljoyn_msgargs_as_uint32(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator long(MsgArg arg)
			{
				return alljoyn_msgargs_as_int64(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator ulong(MsgArg arg)
			{
				return alljoyn_msgargs_as_uint64(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator double(MsgArg arg)
			{
				return alljoyn_msgargs_as_double(arg._msgArgs, (UIntPtr)arg._index);
			}
	
			public static implicit operator string(MsgArg arg)
			{
				return Marshal.PtrToStringAuto(alljoyn_msgargs_as_string(arg._msgArgs, (UIntPtr)arg._index));
			}
	
			public void Set(object value)
			{
				UIntPtr numArgs = (UIntPtr)1;
				string signature = "";
				_setValue = value;
				
				if(_bytePtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(_bytePtr);
					_bytePtr = IntPtr.Zero;
				}
				
				/*
				ALLJOYN_ARRAY            = 'a',    ///< AllJoyn array container type
				ALLJOYN_DICT_ENTRY       = 'e',    ///< AllJoyn dictionary or map container type - an array of key-value pairs
				ALLJOYN_SIGNATURE        = 'g',    ///< AllJoyn signature basic type
				ALLJOYN_HANDLE           = 'h',    ///< AllJoyn socket handle basic type
				ALLJOYN_OBJECT_PATH      = 'o',    ///< AllJoyn Name of an AllJoyn object instance basic type
				ALLJOYN_STRUCT           = 'r',    ///< AllJoyn struct container type
				ALLJOYN_VARIANT          = 'v',    ///< AllJoyn variant container type
				*/
				
				if(value.GetType() == typeof(string))
				{
					signature = "s";
					_bytePtr = Marshal.StringToCoTaskMemAuto((string)value);
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, _bytePtr);
				}
				else if(value.GetType() == typeof(bool))
				{
					signature = "b";
					int newValue = ((bool)value ? 1 : 0);
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, newValue);
				}
				else if(value.GetType() == typeof(double) || value.GetType() == typeof(float))
				{
					signature = "d";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (double)value);
				}
				else if(value.GetType() == typeof(int))
				{
					signature = "i";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (int)value);
				}
				else if(value.GetType() == typeof(uint))
				{
					signature = "u";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (uint)value);
				}
				else if(value.GetType() == typeof(short))
				{
					signature = "n";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (short)value);
				}
				else if(value.GetType() == typeof(ushort))
				{
					signature = "q";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (ushort)value);
				}
				else if(value.GetType() == typeof(long))
				{
					signature = "x";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (long)value);
				}
				else if(value.GetType() == typeof(ulong))
				{
					signature = "t";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (ulong)value);
				}
				else if(value.GetType() == typeof(byte))
				{
					signature = "y";
					alljoyn_msgargs_set(_msgArgs, (UIntPtr)_index, ref numArgs, signature, (byte)value);
				}
			}
			
			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern byte alljoyn_msgargs_as_uint8(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_as_bool(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern short alljoyn_msgargs_as_int16(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern ushort alljoyn_msgargs_as_uint16(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_as_int32(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern uint alljoyn_msgargs_as_uint32(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern long alljoyn_msgargs_as_int64(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern ulong alljoyn_msgargs_as_uint64(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern double alljoyn_msgargs_as_double(IntPtr args, UIntPtr idx);
	
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_msgargs_as_string(IntPtr args, UIntPtr idx);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, byte arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, short arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, int arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, uint arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, long arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, ulong arg);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, double arg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_msgargs_set(IntPtr args, UIntPtr argOffset, ref UIntPtr numArgs, 
				[MarshalAs(UnmanagedType.LPStr)] string signature, IntPtr arg);
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
					if(_bytePtr != IntPtr.Zero)
					{
						Marshal.FreeCoTaskMem(_bytePtr);
						_bytePtr = IntPtr.Zero;
					}
				}
				_isDisposed = true;
			}
			
			~MsgArg()
			{
				Dispose(false);
			}
			#endregion
			
			#region Data
			IntPtr _msgArgs;
			uint _index;
			internal object _setValue;
			IntPtr _bytePtr;
			bool _isDisposed = false;
			#endregion
		}
	}
}
