using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class Credentials : IDisposable
		{
			public Credentials()
			{
				_credentials = alljoyn_credentials_create();
			}

			internal Credentials(IntPtr credentials)
			{
				_credentials = credentials;
				_isDisposed = true;
			}

			public bool IsSet(ushort credMask)
			{
				return (alljoyn_credentials_isset(_credentials, credMask) == 1 ? true : false);
			}

			public void Clear()
			{
				alljoyn_credentials_clear(_credentials);
			}

			#region Properties
			public string Password
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_credentials_getpassword(_credentials));
				}
				set
				{
					alljoyn_credentials_setpassword(_credentials, (string)value);
				}
			}

			public string UserName
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_credentials_getusername(_credentials));
				}
				set
				{
					alljoyn_credentials_setusername(_credentials, (string)value);
				}
			}

			public string CertChain
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_credentials_getcertchain(_credentials));
				}
				set
				{
					alljoyn_credentials_setcertchain(_credentials, (string)value);
				}
			}

			public string PrivateKey
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_credentials_getprivateKey(_credentials));
				}
				set
				{
					alljoyn_credentials_setprivatekey(_credentials, (string)value);
				}
			}

			public string LogonEntry
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_credentials_getlogonentry(_credentials));
				}
				set
				{
					alljoyn_credentials_setlogonentry(_credentials, (string)value);
				}
			}

			public uint Expiration
			{
				get
				{
					return alljoyn_credentials_getexpiration(_credentials);
				}
				set
				{
					alljoyn_credentials_setexpiration(_credentials, (uint)value);
				}
			}
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
					alljoyn_credentials_destroy(_credentials);
					_credentials = IntPtr.Zero;
				}
				_isDisposed = true;
			}

			~Credentials()
			{
				Dispose(false);
			}
			#endregion

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_create();

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_destroy(IntPtr msg);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_credentials_isset(IntPtr cred, ushort creds);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setpassword(IntPtr cred, [MarshalAs(UnmanagedType.LPStr)] string pwd);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setusername(IntPtr cred, [MarshalAs(UnmanagedType.LPStr)] string userName);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setcertchain(IntPtr cred, [MarshalAs(UnmanagedType.LPStr)] string certChain);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setprivatekey(IntPtr cred, [MarshalAs(UnmanagedType.LPStr)] string pk);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setlogonentry(IntPtr cred, [MarshalAs(UnmanagedType.LPStr)] string logonEntry);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_setexpiration(IntPtr cred, uint expiration);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_getpassword(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_getusername(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_getcertchain(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_getprivateKey(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_credentials_getlogonentry(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern uint alljoyn_credentials_getexpiration(IntPtr cred);

			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_credentials_clear(IntPtr cred);
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _credentials;
				}
			}
			#endregion

			#region Data
			IntPtr _credentials;
			bool _isDisposed = false;
			#endregion
		}
	}
}
