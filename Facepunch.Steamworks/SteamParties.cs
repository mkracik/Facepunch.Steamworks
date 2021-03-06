﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamParties : SteamClass
	{
		internal static ISteamParties Internal;
		internal override SteamInterface Interface => Internal;

		internal override void InitializeInterface( bool server )
		{
			Internal = new ISteamParties( server );
			InstallEvents();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install<AvailableBeaconLocationsUpdated_t>( x => OnBeaconLocationsUpdated?.Invoke() );
			Dispatch.Install<ActiveBeaconsUpdated_t>( x => OnActiveBeaconsUpdated?.Invoke() );
		}

		/// <summary>
		/// The list of possible Party beacon locations has changed
		/// </summary>
		public static event Action OnBeaconLocationsUpdated;

		/// <summary>
		/// The list of active beacons may have changed
		/// </summary>
		public static event Action OnActiveBeaconsUpdated;


		public static int ActiveBeaconCount => (int) Internal.GetNumActiveBeacons();

		public static IEnumerable<PartyBeacon> ActiveBeacons
		{
			get
			{
				for ( uint i = 0; i < ActiveBeaconCount; i++ )
				{
					yield return new PartyBeacon
					{
						Id = Internal.GetBeaconByIndex( i )
					};
				}
			}
		}

		/// <summary>
		///  Create a new party beacon and activate it in the selected location.
		/// When people begin responding to your beacon, Steam will send you
		/// OnPartyReservation callbacks to let you know who is on the way.
		/// </summary>
		//public async Task<PartyBeacon?> CreateBeacon( int slots, string connectString, string meta )
		//{
		//	var result = await Internal.CreateBeacon( (uint)slots, null, connectString, meta );
		//	if ( !result.HasValue ) return null;
		//}

		// TODO - is this useful to anyone, or is it a load of shit?
	}
}