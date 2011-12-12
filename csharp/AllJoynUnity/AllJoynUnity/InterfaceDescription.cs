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

			#region Equality
			public static bool operator ==(InterfaceDescription one, InterfaceDescription other)
			{
				if((object)one == null && (object)other == null) return true;
				else if((object)one == null || (object)other == null) return false;
				return (alljoyn_interfacedescription_eql(one.UnmanagedPtr, other.UnmanagedPtr) == 1 ? true : false);
			}

			public static bool operator !=(InterfaceDescription one, InterfaceDescription other)
			{
				return !(one == other);
			}

			public override bool Equals(object o) 
			{
				try
				{
					return (this == (InterfaceDescription)o);
				}
				catch
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return (int)_interfaceDescription;
			}
			#endregion

			#region Properties
			public bool HasProperties
			{
				get
				{
					return (alljoyn_interfacedescription_hasproperties(_interfaceDescription) == 1 ? true : false);
				}
			}

			public string Name
			{
				get
				{
					return Marshal.PtrToStringAuto(alljoyn_interfacedescription_getname(_interfaceDescription));
				}
			}

			public bool IsSecure
			{
				get
				{
					return (alljoyn_interfacedescription_issecure(_interfaceDescription) == 1 ? true : false);
				}
			}
			#endregion

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
				if(alljoyn_interfacedescription_getmember(_interfaceDescription, name, ref retMember) == 1)
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

			public bool HasMember(string name, string inSig, string outSig)
			{
				return (alljoyn_interfacedescription_hasmember(_interfaceDescription,
					name, inSig, outSig) == 1 ? true : false );
			}

			public Property GetProperty(string name)
			{
				_Property retProp = new _Property();
				if(alljoyn_interfacedescription_getproperty(_interfaceDescription, name, ref retProp) == 1)
				{
					return new Property(retProp);
				}
				else
				{
					return null;
				}
			}

			public Property[] GetProperties()
			{
				UIntPtr numProperties = alljoyn_interfacedescription_getproperties(_interfaceDescription,
					IntPtr.Zero, (UIntPtr)0);
				_Property[] props = new _Property[(int)numProperties];
				GCHandle gch = GCHandle.Alloc(props, GCHandleType.Pinned);
				UIntPtr numFilledProperties = alljoyn_interfacedescription_getproperties(_interfaceDescription,
					gch.AddrOfPinnedObject(), numProperties);
				if(numProperties != numFilledProperties)
				{
					// Warn?
				}
				Property[] ret = new Property[(int)numFilledProperties];
				for(int i = 0; i < ret.Length; i++)
				{
					ret[i] = new Property(props[i]);
				}

				return ret;
			}

			public QStatus AddProperty(string name, string signature, AccessFlags access)
			{
				return alljoyn_interfacedescription_addproperty(_interfaceDescription,
					name, signature, (byte)access);
			}

			public bool HasProperty(string name)
			{
				return (alljoyn_interfacedescription_hasproperty(_interfaceDescription, name) == 1 ? true : false);
			}

			public class Member
			{
				public InterfaceDescription Iface
				{
					get
					{
						return new InterfaceDescription(_member.iface);
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

			public class Property
			{
				public string Name
				{
					get
					{
						return Marshal.PtrToStringAuto(_property.name);
					}
				}

				public string Signature
				{
					get
					{
						return Marshal.PtrToStringAuto(_property.signature);
					}
				}

				public AccessFlags Access
				{
					get
					{
						return (AccessFlags)_property.access;
					}
				}

				internal Property(_Property property)
				{
					_property = property;
				}

				internal Property(IntPtr propertyPointer)
				{
					_property = (_Property)Marshal.PtrToStructure(propertyPointer, typeof(_Property));
				}

				#region Data
				internal _Property _property;
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

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasmember(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string inSig,
				[MarshalAs(UnmanagedType.LPStr)] string outSig);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_getproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				ref _Property property);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static UIntPtr alljoyn_interfacedescription_getproperties(IntPtr iface,
				IntPtr props, UIntPtr numProps);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_addproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name,
				[MarshalAs(UnmanagedType.LPStr)] string signature,
				byte access);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasproperty(IntPtr iface,
				[MarshalAs(UnmanagedType.LPStr)] string name);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_eql(IntPtr one, IntPtr other);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_hasproperties(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static IntPtr alljoyn_interfacedescription_getname(IntPtr iface);

			[DllImport(DLL_IMPORT_TARGET)]
			private extern static int alljoyn_interfacedescription_issecure(IntPtr iface);
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
#pragma warning disable 169 // Field is never used
				private IntPtr internal_member;
#pragma warning restore 169
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct _Property
			{
				public IntPtr name;
				public IntPtr signature;
				public byte access;
#pragma warning disable 169 // Field is never used
				private IntPtr internal_property;
#pragma warning restore 169
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

