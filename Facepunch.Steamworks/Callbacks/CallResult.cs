﻿using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	/// <summary>
	/// An awaitable version of a SteamAPICall_t
	/// </summary>
	internal struct CallResult<T> : INotifyCompletion where T : struct, ICallbackData
	{
		SteamAPICall_t call;

		public CallResult( SteamAPICall_t call )
		{
			this.call = call;
		}

		/// <summary>
		/// This gets called if IsComplete returned false on the first call.
		/// The Action "continues" the async call. We pass it to the Dispatch
		/// to be called when the callback returns.
		/// </summary>
		public void OnCompleted( Action continuation )
		{
			Dispatch.OnCallComplete( call, continuation );
		}

		/// <summary>
		/// Gets the result. This is called internally by the async shit.
		/// </summary>
		public T? GetResult()
		{
			if ( !SteamUtils.IsCallComplete( call, out var failed ) || failed )
				return null;

			var t = default( T );
			var size = t.DataSize;
			var ptr = Marshal.AllocHGlobal( size );

			try
			{
				if ( !SteamUtils.Internal.GetAPICallResult( call, ptr, size, (int) t.CallbackType, ref failed ) || failed )
					return null;

				return ((T)Marshal.PtrToStructure( ptr, typeof( T ) ));
			}
			finally
			{
				Marshal.FreeHGlobal( ptr );
			}
		}

		/// <summary>
		/// Return true if complete or failed
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				if ( SteamUtils.IsCallComplete( call, out var failed ) || failed )
					return true;

				return false;
			}
		}

		/// <summary>
		/// This is what makes this struct awaitable
		/// </summary>
		internal CallResult<T> GetAwaiter()
		{
			return this;
		}
	}
}