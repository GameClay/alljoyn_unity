using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		// DLL name for externs
		private const string DLL_IMPORT_TARGET = "alljoyn_unity";

		public static string GetVersion()
		{
			return Marshal.PtrToStringAuto(alljoyn_getversion());
		}

		public static string GetBuildInfo()
		{
			return Marshal.PtrToStringAuto(alljoyn_getbuildinfo());
		}

		[Flags]
		public enum TransportMask : ushort
		{
			None = 0x0000,
			Any = 0xFFFF,
			Local = 0x0001,
			Bluetooth = 0x0002,
			WLAN = 0x0004,
			WWAN = 0x0008,
			LAN = 0x0010
		}

		public class QStatus
		{
			private QStatus(int x)
			{
				value = x;
			}

			public static implicit operator QStatus(int x)
			{
				return new QStatus(x);
			}

			public static implicit operator int(QStatus x)
			{
				return x.value;
			}

			public static bool operator true(QStatus x)
			{
				return (x == OK);
			}

			public static bool operator false(QStatus x)
			{
				return (x != OK);
			}

			public static bool operator ==(QStatus x, QStatus y)
			{
				return x.value == y.value;
			}

			public override bool Equals(object o) 
			{
				try
				{
					return (this == (QStatus)o);
				}
				catch
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return value;
			}

			public override string ToString()
			{
				return Marshal.PtrToStringAuto(QCC_StatusText(value));
			}

			public static implicit operator string(QStatus x)
			{
				return x.value.ToString();
			}

			public static bool operator !=(QStatus x, QStatus y)
			{
				return x.value != y.value;
			}

			public static bool operator !(QStatus x)
			{
				return (x != OK);
			}

			internal int value;

			public static readonly QStatus OK = new QStatus(0x0);
			public static readonly QStatus FAIL = new QStatus(0x1);
			public static readonly QStatus UTF_CONVERSION_FAILED = new QStatus(0x2);
			public static readonly QStatus BUFFER_TOO_SMALL = new QStatus(0x3);
			public static readonly QStatus OS_ERROR = new QStatus(0x4);
			public static readonly QStatus OUT_OF_MEMORY = new QStatus(0x5);
			public static readonly QStatus SOCKET_BIND_ERROR = new QStatus(0x6);
			public static readonly QStatus INIT_FAILED = new QStatus(0x7);
			public static readonly QStatus WOULDBLOCK = new QStatus(0x8);
			public static readonly QStatus NOT_IMPLEMENTED = new QStatus(0x9);
			public static readonly QStatus TIMEOUT = new QStatus(0xa);
			public static readonly QStatus SOCK_OTHER_END_CLOSED = new QStatus(0xb);
			public static readonly QStatus BAD_ARG_1 = new QStatus(0xc);
			public static readonly QStatus BAD_ARG_2 = new QStatus(0xd);
			public static readonly QStatus BAD_ARG_3 = new QStatus(0xe);
			public static readonly QStatus BAD_ARG_4 = new QStatus(0xf);
			public static readonly QStatus BAD_ARG_5 = new QStatus(0x10);
			public static readonly QStatus BAD_ARG_6 = new QStatus(0x11);
			public static readonly QStatus BAD_ARG_7 = new QStatus(0x12);
			public static readonly QStatus BAD_ARG_8 = new QStatus(0x13);
			public static readonly QStatus INVALID_ADDRESS = new QStatus(0x14);
			public static readonly QStatus INVALID_DATA = new QStatus(0x15);
			public static readonly QStatus READ_ERROR = new QStatus(0x16);
			public static readonly QStatus WRITE_ERROR = new QStatus(0x17);
			public static readonly QStatus OPEN_FAILED = new QStatus(0x18);
			public static readonly QStatus PARSE_ERROR = new QStatus(0x19);
			public static readonly QStatus END_OF_DATA = new QStatus(0x1A);
			public static readonly QStatus CONN_REFUSED = new QStatus(0x1B);
			public static readonly QStatus BAD_ARG_COUNT = new QStatus(0x1C);
			public static readonly QStatus COMMON_ERRORS = new QStatus(0x1000);
			public static readonly QStatus STOPPING_THREAD = new QStatus(0x1001);
			public static readonly QStatus ALERTED_THREAD = new QStatus(0x1002);
			public static readonly QStatus XML_MALFORMED = new QStatus(0x1003);
			public static readonly QStatus AUTH_FAIL = new QStatus(0x1004);
			public static readonly QStatus AUTH_USER_REJECT = new QStatus(0x1005);
			public static readonly QStatus NO_SUCH_ALARM = new QStatus(0x1006);
			public static readonly QStatus TIMER_FALLBEHIND = new QStatus(0x1007);
			public static readonly QStatus SSL_ERRORS = new QStatus(0x1008);
			public static readonly QStatus SSL_INIT = new QStatus(0x1009);
			public static readonly QStatus SSL_CONNECT = new QStatus(0x100a);
			public static readonly QStatus SSL_VERIFY = new QStatus(0x100b);
			public static readonly QStatus EXTERNAL_THREAD = new QStatus(0x100c);
			public static readonly QStatus CRYPTO_ERROR = new QStatus(0x100d);
			public static readonly QStatus CRYPTO_TRUNCATED = new QStatus(0x100e);
			public static readonly QStatus CRYPTO_KEY_UNAVAILABLE = new QStatus(0x100f);
			public static readonly QStatus BAD_HOSTNAME = new QStatus(0x1010);
			public static readonly QStatus CRYPTO_KEY_UNUSABLE = new QStatus(0x1011);
			public static readonly QStatus EMPTY_KEY_BLOB = new QStatus(0x1012);
			public static readonly QStatus CORRUPT_KEYBLOB = new QStatus(0x1013);
			public static readonly QStatus INVALID_KEY_ENCODING = new QStatus(0x1014);
			public static readonly QStatus DEAD_THREAD = new QStatus(0x1015);
			public static readonly QStatus THREAD_RUNNING = new QStatus(0x1016);
			public static readonly QStatus THREAD_STOPPING = new QStatus(0x1017);
			public static readonly QStatus BAD_STRING_ENCODING = new QStatus(0x1018);
			public static readonly QStatus CRYPTO_INSUFFICIENT_SECURITY = new QStatus(0x1019);
			public static readonly QStatus CRYPTO_ILLEGAL_PARAMETERS = new QStatus(0x101a);
			public static readonly QStatus CRYPTO_HASH_UNINITIALIZED = new QStatus(0x101b);
			public static readonly QStatus THREAD_NO_WAIT = new QStatus(0x101c);
			public static readonly QStatus TIMER_EXITING = new QStatus(0x101d);
			public static readonly QStatus INVALID_GUID = new QStatus(0x101e);
			public static readonly QStatus NONE = new QStatus(0xffff);
			public static readonly QStatus BUS_ERRORS = new QStatus(0x9000);
			public static readonly QStatus BUS_READ_ERROR = new QStatus(0x9001);
			public static readonly QStatus BUS_WRITE_ERROR = new QStatus(0x9002);
			public static readonly QStatus BUS_BAD_VALUE_TYPE = new QStatus(0x9003);
			public static readonly QStatus BUS_BAD_HEADER_FIELD = new QStatus(0x9004);
			public static readonly QStatus BUS_BAD_SIGNATURE = new QStatus(0x9005);
			public static readonly QStatus BUS_BAD_OBJ_PATH = new QStatus(0x9006);
			public static readonly QStatus BUS_BAD_MEMBER_NAME = new QStatus(0x9007);
			public static readonly QStatus BUS_BAD_INTERFACE_NAME = new QStatus(0x9008);
			public static readonly QStatus BUS_BAD_ERROR_NAME = new QStatus(0x9009);
			public static readonly QStatus BUS_BAD_BUS_NAME = new QStatus(0x900a);
			public static readonly QStatus BUS_NAME_TOO_LONG = new QStatus(0x900b);
			public static readonly QStatus BUS_BAD_LENGTH = new QStatus(0x900c);
			public static readonly QStatus BUS_BAD_VALUE = new QStatus(0x900d);
			public static readonly QStatus BUS_BAD_HDR_FLAGS = new QStatus(0x900e);
			public static readonly QStatus BUS_BAD_BODY_LEN = new QStatus(0x900f);
			public static readonly QStatus BUS_BAD_HEADER_LEN = new QStatus(0x9010);
			public static readonly QStatus BUS_UNKNOWN_SERIAL = new QStatus(0x9011);
			public static readonly QStatus BUS_UNKNOWN_PATH = new QStatus(0x9012);
			public static readonly QStatus BUS_UNKNOWN_INTERFACE = new QStatus(0x9013);
			public static readonly QStatus BUS_ESTABLISH_FAILED = new QStatus(0x9014);
			public static readonly QStatus BUS_UNEXPECTED_SIGNATURE = new QStatus(0x9015);
			public static readonly QStatus BUS_INTERFACE_MISSING = new QStatus(0x9016);
			public static readonly QStatus BUS_PATH_MISSING = new QStatus(0x9017);
			public static readonly QStatus BUS_MEMBER_MISSING = new QStatus(0x9018);
			public static readonly QStatus BUS_REPLY_SERIAL_MISSING = new QStatus(0x9019);
			public static readonly QStatus BUS_ERROR_NAME_MISSING = new QStatus(0x901a);
			public static readonly QStatus BUS_INTERFACE_NO_SUCH_MEMBER = new QStatus(0x901b);
			public static readonly QStatus BUS_NO_SUCH_OBJECT = new QStatus(0x901c);
			public static readonly QStatus BUS_OBJECT_NO_SUCH_MEMBER = new QStatus(0x901d);
			public static readonly QStatus BUS_OBJECT_NO_SUCH_INTERFACE = new QStatus(0x901e);
			public static readonly QStatus BUS_NO_SUCH_INTERFACE = new QStatus(0x901f);
			public static readonly QStatus BUS_MEMBER_NO_SUCH_SIGNATURE = new QStatus(0x9020);
			public static readonly QStatus BUS_NOT_NUL_TERMINATED = new QStatus(0x9021);
			public static readonly QStatus BUS_NO_SUCH_PROPERTY = new QStatus(0x9022);
			public static readonly QStatus BUS_SET_WRONG_SIGNATURE = new QStatus(0x9023);
			public static readonly QStatus BUS_PROPERTY_VALUE_NOT_SET = new QStatus(0x9024);
			public static readonly QStatus BUS_PROPERTY_ACCESS_DENIED = new QStatus(0x9025);
			public static readonly QStatus BUS_NO_TRANSPORTS = new QStatus(0x9026);
			public static readonly QStatus BUS_BAD_TRANSPORT_ARGS = new QStatus(0x9027);
			public static readonly QStatus BUS_NO_ROUTE = new QStatus(0x9028);
			public static readonly QStatus BUS_NO_ENDPOINT = new QStatus(0x9029);
			public static readonly QStatus BUS_BAD_SEND_PARAMETER = new QStatus(0x902a);
			public static readonly QStatus BUS_UNMATCHED_REPLY_SERIAL = new QStatus(0x902b);
			public static readonly QStatus BUS_BAD_SENDER_ID = new QStatus(0x902c);
			public static readonly QStatus BUS_TRANSPORT_NOT_STARTED = new QStatus(0x902d);
			public static readonly QStatus BUS_EMPTY_MESSAGE = new QStatus(0x902e);
			public static readonly QStatus BUS_NOT_OWNER = new QStatus(0x902f);
			public static readonly QStatus BUS_SET_PROPERTY_REJECTED = new QStatus(0x9030);
			public static readonly QStatus BUS_CONNECT_FAILED = new QStatus(0x9031);
			public static readonly QStatus BUS_REPLY_IS_ERROR_MESSAGE = new QStatus(0x9032);
			public static readonly QStatus BUS_NOT_AUTHENTICATING = new QStatus(0x9033);
			public static readonly QStatus BUS_NO_LISTENER = new QStatus(0x9034);
			public static readonly QStatus BUS_BT_TRANSPORT_ERROR = new QStatus(0x9035);
			public static readonly QStatus BUS_NOT_ALLOWED = new QStatus(0x9036);
			public static readonly QStatus BUS_WRITE_QUEUE_FULL = new QStatus(0x9037);
			public static readonly QStatus BUS_ENDPOINT_CLOSING = new QStatus(0x9038);
			public static readonly QStatus BUS_INTERFACE_MISMATCH = new QStatus(0x9039);
			public static readonly QStatus BUS_MEMBER_ALREADY_EXISTS = new QStatus(0x903a);
			public static readonly QStatus BUS_PROPERTY_ALREADY_EXISTS = new QStatus(0x903b);
			public static readonly QStatus BUS_IFACE_ALREADY_EXISTS = new QStatus(0x903c);
			public static readonly QStatus BUS_ERROR_RESPONSE = new QStatus(0x903d);
			public static readonly QStatus BUS_BAD_XML = new QStatus(0x903e);
			public static readonly QStatus BUS_BAD_CHILD_PATH = new QStatus(0x903f);
			public static readonly QStatus BUS_OBJ_ALREADY_EXISTS = new QStatus(0x9040);
			public static readonly QStatus BUS_OBJ_NOT_FOUND = new QStatus(0x9041);
			public static readonly QStatus BUS_CANNOT_EXPAND_MESSAGE = new QStatus(0x9042);
			public static readonly QStatus BUS_NOT_COMPRESSED = new QStatus(0x9043);
			public static readonly QStatus BUS_ALREADY_CONNECTED = new QStatus(0x9044);
			public static readonly QStatus BUS_NOT_CONNECTED = new QStatus(0x9045);
			public static readonly QStatus BUS_ALREADY_LISTENING = new QStatus(0x9046);
			public static readonly QStatus BUS_KEY_UNAVAILABLE = new QStatus(0x9047);
			public static readonly QStatus BUS_TRUNCATED = new QStatus(0x9048);
			public static readonly QStatus BUS_KEY_STORE_NOT_LOADED = new QStatus(0x9049);
			public static readonly QStatus BUS_NO_AUTHENTICATION_MECHANISM = new QStatus(0x904a);
			public static readonly QStatus BUS_BUS_ALREADY_STARTED = new QStatus(0x904b);
			public static readonly QStatus BUS_BUS_NOT_STARTED = new QStatus(0x904c);
			public static readonly QStatus BUS_KEYBLOB_OP_INVALID = new QStatus(0x904d);
			public static readonly QStatus BUS_INVALID_HEADER_CHECKSUM = new QStatus(0x904e);
			public static readonly QStatus BUS_MESSAGE_NOT_ENCRYPTED = new QStatus(0x904f);
			public static readonly QStatus BUS_INVALID_HEADER_SERIAL = new QStatus(0x9050);
			public static readonly QStatus BUS_TIME_TO_LIVE_EXPIRED = new QStatus(0x9051);
			public static readonly QStatus BUS_HDR_EXPANSION_INVALID = new QStatus(0x9052);
			public static readonly QStatus BUS_MISSING_COMPRESSION_TOKEN = new QStatus(0x9053);
			public static readonly QStatus BUS_NO_PEER_GUID = new QStatus(0x9054);
			public static readonly QStatus BUS_MESSAGE_DECRYPTION_FAILED = new QStatus(0x9055);
			public static readonly QStatus BUS_SECURITY_FATAL = new QStatus(0x9056);
			public static readonly QStatus BUS_KEY_EXPIRED = new QStatus(0x9057);
			public static readonly QStatus BUS_CORRUPT_KEYSTORE = new QStatus(0x9058);
			public static readonly QStatus BUS_NO_CALL_FOR_REPLY = new QStatus(0x9059);
			public static readonly QStatus BUS_NOT_A_COMPLETE_TYPE = new QStatus(0x905a);
			public static readonly QStatus BUS_POLICY_VIOLATION = new QStatus(0x905b);
			public static readonly QStatus BUS_NO_SUCH_SERVICE = new QStatus(0x905c);
			public static readonly QStatus BUS_TRANSPORT_NOT_AVAILABLE = new QStatus(0x905d);
			public static readonly QStatus BUS_INVALID_AUTH_MECHANISM = new QStatus(0x905e);
			public static readonly QStatus BUS_KEYSTORE_VERSION_MISMATCH = new QStatus(0x905f);
			public static readonly QStatus BUS_BLOCKING_CALL_NOT_ALLOWED = new QStatus(0x9060);
			public static readonly QStatus BUS_SIGNATURE_MISMATCH = new QStatus(0x9061);
			public static readonly QStatus BUS_STOPPING = new QStatus(0x9062);
			public static readonly QStatus BUS_METHOD_CALL_ABORTED = new QStatus(0x9063);
			public static readonly QStatus BUS_CANNOT_ADD_INTERFACE = new QStatus(0x9064);
			public static readonly QStatus BUS_CANNOT_ADD_HANDLER = new QStatus(0x9065);
			public static readonly QStatus BUS_KEYSTORE_NOT_LOADED = new QStatus(0x9066);
			public static readonly QStatus BUS_NO_SUCH_HANDLE = new QStatus(0x906b);
			public static readonly QStatus BUS_HANDLES_NOT_ENABLED = new QStatus(0x906c);
			public static readonly QStatus BUS_HANDLES_MISMATCH = new QStatus(0x906d);
			public static readonly QStatus BT_MAX_CONNECTIONS_USED = new QStatus(0x906e);
			public static readonly QStatus BUS_NO_SESSION = new QStatus(0x906f);
			public static readonly QStatus BUS_ELEMENT_NOT_FOUND = new QStatus(0x9070);
			public static readonly QStatus BUS_NOT_A_DICTIONARY = new QStatus(0x9071);
			public static readonly QStatus BUS_WAIT_FAILED = new QStatus(0x9072);
			public static readonly QStatus BUS_BAD_SESSION_OPTS = new QStatus(0x9074);
			public static readonly QStatus BUS_CONNECTION_REJECTED = new QStatus(0x9075);
			public static readonly QStatus DBUS_REQUEST_NAME_REPLY_PRIMARY_OWNER = new QStatus(0x9076);
			public static readonly QStatus DBUS_REQUEST_NAME_REPLY_IN_QUEUE = new QStatus(0x9077);
			public static readonly QStatus DBUS_REQUEST_NAME_REPLY_EXISTS = new QStatus(0x9078);
			public static readonly QStatus DBUS_REQUEST_NAME_REPLY_ALREADY_OWNER = new QStatus(0x9079);
			public static readonly QStatus DBUS_RELEASE_NAME_REPLY_RELEASED = new QStatus(0x907a);
			public static readonly QStatus DBUS_RELEASE_NAME_REPLY_NON_EXISTENT = new QStatus(0x907b);
			public static readonly QStatus DBUS_RELEASE_NAME_REPLY_NOT_OWNER = new QStatus(0x907c);
			public static readonly QStatus DBUS_START_REPLY_ALREADY_RUNNING = new QStatus(0x907e);
			public static readonly QStatus ALLJOYN_BINDSESSIONPORT_REPLY_ALREADY_EXISTS = new QStatus(0x9080);
			public static readonly QStatus ALLJOYN_BINDSESSIONPORT_REPLY_FAILED = new QStatus(0x9081);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_NO_SESSION = new QStatus(0x9083);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_UNREACHABLE = new QStatus(0x9084);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_CONNECT_FAILED = new QStatus(0x9085);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_REJECTED = new QStatus(0x9086);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_BAD_SESSION_OPTS = new QStatus(0x9087);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_FAILED = new QStatus(0x9088);
			public static readonly QStatus ALLJOYN_LEAVESESSION_REPLY_NO_SESSION = new QStatus(0x908a);
			public static readonly QStatus ALLJOYN_LEAVESESSION_REPLY_FAILED = new QStatus(0x908b);
			public static readonly QStatus ALLJOYN_ADVERTISENAME_REPLY_ALREADY_ADVERTISING = new QStatus(0x908d);
			public static readonly QStatus ALLJOYN_ADVERTISENAME_REPLY_FAILED = new QStatus(0x908e);
			public static readonly QStatus ALLJOYN_CANCELADVERTISENAME_REPLY_FAILED = new QStatus(0x9090);
			public static readonly QStatus ALLJOYN_FINDADVERTISEDNAME_REPLY_ALREADY_DISCOVERING = new QStatus(0x9092);
			public static readonly QStatus ALLJOYN_FINDADVERTISEDNAME_REPLY_FAILED = new QStatus(0x9093);
			public static readonly QStatus ALLJOYN_CANCELFINDADVERTISEDNAME_REPLY_FAILED = new QStatus(0x9095);
			public static readonly QStatus BUS_UNEXPECTED_DISPOSITION = new QStatus(0x9096);
			public static readonly QStatus BUS_INTERFACE_ACTIVATED = new QStatus(0x9097);
			public static readonly QStatus ALLJOYN_UNBINDSESSIONPORT_REPLY_BAD_PORT = new QStatus(0x9098);
			public static readonly QStatus ALLJOYN_UNBINDSESSIONPORT_REPLY_FAILED = new QStatus(0x9099);
			public static readonly QStatus ALLJOYN_BINDSESSIONPORT_REPLY_INVALID_OPTS = new QStatus(0x909a);
			public static readonly QStatus ALLJOYN_JOINSESSION_REPLY_ALREADY_JOINED = new QStatus(0x909b);
			public static readonly QStatus BUS_SELF_CONNECT = new QStatus(0x909c);
			public static readonly QStatus BUS_SECURITY_NOT_ENABLED = new QStatus(0x909d);
			public static readonly QStatus BUS_LISTENER_ALREADY_SET = new QStatus(0x909e);
			public static readonly QStatus BUS_PEER_AUTH_VERSION_MISMATCH = new QStatus(0x909f);
			public static readonly QStatus ALLJOYN_SETLINKTIMEOUT_REPLY_NOT_SUPPORTED = new QStatus(0x90a0);
			public static readonly QStatus ALLJOYN_SETLINKTIMEOUT_REPLY_NO_DEST_SUPPORT = new QStatus(0x90a1);
			public static readonly QStatus ALLJOYN_SETLINKTIMEOUT_REPLY_FAILED = new QStatus(0x90a2);
			public static readonly QStatus ALLJOYN_ACCESS_PERMISSION_WARNING = new QStatus(0x90a3);
			public static readonly QStatus ALLJOYN_ACCESS_PERMISSION_ERROR = new QStatus(0x90a4);
		}

		#region DLL Imports
		[DllImport(DLL_IMPORT_TARGET)]
		private extern static IntPtr alljoyn_getversion();

		[DllImport(DLL_IMPORT_TARGET)]
		private extern static IntPtr alljoyn_getbuildinfo();

		[DllImport(DLL_IMPORT_TARGET)]
		private extern static IntPtr QCC_StatusText(int status);
		#endregion
	}
}

