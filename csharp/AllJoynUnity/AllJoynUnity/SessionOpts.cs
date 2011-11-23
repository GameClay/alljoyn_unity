using System;
using System.Runtime.InteropServices;

namespace AllJoynUnity
{
	public partial class AllJoyn
	{
		public class SessionOpts : IDisposable
		{
			public enum TrafficType : byte
			{
				Messages = 0x01,
				RawUnreliable = 0x02,
				RawReliable = 0x04
			}
			
			public enum ProximityType : byte
			{
				Any = 0xFF,
				Physical = 0x01,
				Network = 0x02
			}
			
			#region Properties
			public TrafficType Traffic
			{
				get
				{
					return (TrafficType)alljoyn_sessionopts_traffic(_sessionOpts);
				}
			}
			
			public bool IsMultipoint
			{
				get
				{
					return (alljoyn_sessionopts_multipoint(_sessionOpts) == 1 ? true : false);
				}
			}
			
			public ProximityType Proximity
			{
				get
				{
					return (ProximityType)alljoyn_sessionopts_proximity(_sessionOpts);
				}
			}
			
			public TransportMask Transports
			{
				get
				{
					return (TransportMask)alljoyn_sessionopts_transports(_sessionOpts);
				}
			}
			
			internal IntPtr UnmanagedPtr
			{
				get
				{
					return _sessionOpts;
				}
			}
			#endregion
			
			public SessionOpts(TrafficType trafficType, bool isMultipoint, ProximityType proximity, TransportMask transports)
			{
				_sessionOpts = alljoyn_sessionopts_create((byte)trafficType, isMultipoint ? 1 : 0, (byte)proximity, (ushort)transports);
			}
			
			internal SessionOpts(IntPtr sessionOpts)
			{
				_sessionOpts = sessionOpts;
				_isDisposed = true;
			}
			
			public bool IsCompatible(SessionOpts other)
			{
				return (alljoyn_sessionopts_iscompatible(_sessionOpts, other._sessionOpts) == 1 ? true : false);
			}
			
			public static int Compare(SessionOpts one, SessionOpts other)
			{
				return alljoyn_sessionopts_cmp(one._sessionOpts, other._sessionOpts);
			}
			
			#region DLL Imports
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern IntPtr alljoyn_sessionopts_create(byte traffic, int isMultipoint,
				byte proximity, ushort transports);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern void alljoyn_sessionopts_destroy(IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern byte alljoyn_sessionopts_traffic(IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_sessionopts_multipoint(IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern byte alljoyn_sessionopts_proximity(IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern ushort alljoyn_sessionopts_transports(IntPtr opts);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_sessionopts_iscompatible(IntPtr one, IntPtr other);
			
			[DllImport(DLL_IMPORT_TARGET)]
			private static extern int alljoyn_sessionopts_cmp(IntPtr one, IntPtr other);
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
					alljoyn_sessionopts_destroy(_sessionOpts);
					_sessionOpts = IntPtr.Zero;
				}
				_isDisposed = true;
			}
			
			~SessionOpts()
			{
				Dispose(false);
			}
			#endregion
			
			#region Data
			IntPtr _sessionOpts;
			bool _isDisposed = false;
			#endregion
		}
	}
}
