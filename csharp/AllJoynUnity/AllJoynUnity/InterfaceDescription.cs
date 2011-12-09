using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class InterfaceDescription
		{
			[Flags]
			public enum AnnotationFlags : byte
			{
				Default = 0,
				NoReply = 1,
				Deprecated = 2
			}

			[Flags]
			public enum AccessFlags : byte
			{
				Read = 1,
				Write = 2
			}

			internal InterfaceDescription(IntPtr interfaceDescription)
			{
				_interfaceDescription = interfaceDescription;
			}

			public QStatus AddMember(Message.Type type, string name, string inputSignature,
				string outputSignature, string argNames, AnnotationFlags annotation = AnnotationFlags.Default)
			{
				return alljoyn_interfacedescription_addmember(_interfaceDescription,
					(int)type, name, inputSignature, outputSignature, argNames, (byte)annotation);
			}

			public void Activate()
			{
				alljoyn_interfacedescription_activate(_interfaceDescription);
			}

			public Member GetMember(string name)
			{
				_Member retMember = new _Member();
				if(alljoyn_interfacedescription_getmember(_interfaceDescription, name, ref retMember) > 0)
				{
					return new Member(retMember);
				}
				else
				{
					return null;
				}
			}

			public Member[] GetMembers()
			{
				UIntPtr numMembers = alljoyn_interfacedescription_getmembers(_interfaceDescription,
					IntPtr.Zero, (UIntPtr)0);
				_Member[] members = new _Member[(int)numMembers];
				GCHandle gch = GCHandle.Alloc(members, GCHandleType.Pinned);
				UIntPtr numFilledMembers = alljoyn_interfacedescription_getmembers(_interfaceDescription,
					gch.AddrOfPinnedObject(), numMembers);
				if(numMembers != numFilledMembers)
				{
					// Warn?
				}
				Member[] ret = new Member[(int)numFilledMembers];
				for(int i = 0; i < ret.Length; i++)
				{
					ret[i] = new Member(members[i]);
				}

				return ret;
			}

			public class Member
			{
				public InterfaceDescription Iface
				{
					get
					{
						return null; //TODO
					}
				}

				public Message.Type MemberType
				{
					get
					{
						return (Message.Type)_member.memberType;
					}
				}

				public string Name
				{
					get
					{
						return Marshal.PtrToStringAuto(_member.name);
					}
				}

				public string Signature
				{
					get
					{
						return Marshal.PtrToStringAuto(_member.signature);
					}
				}

				public string ReturnSignature
				{
					get
					{
						return Marshal.PtrToStringAuto(_member.returnSignature);
					}
				}

				public string ArgNames
				{
					get
					{
						return Marshal.PtrToStringAuto(_member.argNames);
					}
				}

				public AnnotationFlags Annotation
				{
					get
					{
						return (AnnotationFlags)_member.annotation;
					}
				}

				internal Member(_Member member)
				{
					_member = member;
				}

				internal Member(IntPtr memberPtr)
				{
					_member = (_Member)Marshal.PtrToStructure(memberPtr, typeof(_Member));
				}

				#region Data
				internal _Member _member;
				#endregion
			}

			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_addmember(
				IntPtr iface,
				int type,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string inputSig,
				[MarshalAs(UnmanagedType.LPStr)] string outSig,
				[MarshalAs(UnmanagedType.LPStr)] string argNames,
				byte annotation);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static void alljoyn_interfacedescription_activate(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_getmember(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref _Member member);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_interfacedescription_getmembers(IntPtr iface,
				IntPtr members, UIntPtr numMembers);
			#endregion

			#region Internal Structures
			[StructLayout(LayoutKind.Sequential)]
			internal struct _Member
			{
				public IntPtr iface;
				public int memberType;
				public IntPtr name;
				public IntPtr signature;
				public IntPtr returnSignature;
				public IntPtr argNames;
				public byte annotation;
				private IntPtr internal_member;
			}
			#endregion

			#region Internal Properties
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _interfaceDescription;
				}
			}
			#endregion

			#region Data
			IntPtr _interfaceDescription;
			#endregion
		}
	}
}

