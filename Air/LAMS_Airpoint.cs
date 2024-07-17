using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
	{
	[System.Serializable]
	public class LAMS_Airpoint
	{
		public Vector3 Position;
		public int Index_WithinManagerList;
		public List<LAMS_AirpointRelationship> MyRelationships;
		public List<int> Indices_VisibleAirpoint;

		[Header("[[----------- DEBUG ------------]]")]
		public bool AmHighlighted;
		public bool HelpLocate;
		public bool HighlightVisible;
		public int Index_DebugRelationship = -1;

		public LAMS_Airpoint(Vector3 pos_passed, int index_passed)
		{
			Position = pos_passed;
			Index_WithinManagerList = index_passed;
			AmHighlighted = false;
			HelpLocate = false;
			MyRelationships = new List<LAMS_AirpointRelationship>();
			Indices_VisibleAirpoint = new List<int>();
		}

		public void CaptureVisibleAndObscured( LAMS_AirspaceManager mgr_passed, int mask_passed )
		{
			//StringBuilder sb = new StringBuilder($"point.CaptureAllVisible('{mgr_passed.MyAirpoints.Count}', mask: '{mask_passed}') report ----------\n");

			if ( Indices_VisibleAirpoint == null )
			{
				Indices_VisibleAirpoint = new List<int>();
			}

			for ( int i = 0; i < mgr_passed.MyAirpoints.Count; i++ )
			{
				LAMS_Airpoint pt = mgr_passed.MyAirpoints[i];

				if ( i == Index_WithinManagerList || Indices_VisibleAirpoint.Contains(i) || HaveRelationshipWithDestinationIndex(i) )
				{
					//sb.AppendLine($"'{i}' was either myself or already logged.");
					//mgr_passed.captureContinues++;
					continue;
				}
				else if ( !Physics.Linecast(Position, pt.Position, mask_passed) )
				{
					//sb.AppendLine($"Succesful cast with '{i}'.");

					Indices_VisibleAirpoint.Add(i);
					LAMS_AirpointRelationship rel = new LAMS_AirpointRelationship( i, Vector3.Distance(Position, pt.Position), true );
					rel.Path_destinationPoint.Add(pt.Index_WithinManagerList);
					MyRelationships.Add(rel);

					pt.Indices_VisibleAirpoint.Add(Index_WithinManagerList);
					LAMS_AirpointRelationship rel_other = new LAMS_AirpointRelationship( Index_WithinManagerList, rel.Distance, true );
					rel_other.Path_destinationPoint.Add(Index_WithinManagerList);
					pt.MyRelationships.Add(rel_other);

				}
				else
				{
					MyRelationships.Add( new LAMS_AirpointRelationship(i, float.MaxValue, false) );
					pt.MyRelationships.Add( new LAMS_AirpointRelationship(Index_WithinManagerList, float.MaxValue, false) );
				}
			}
			//Debug.Log(sb.ToString());
		}

		public void Ping( LAMS_AirspaceManager mgr_passed )
		{
			//StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Indices_VisibleAirpoint.Count; i++)
			{
				//sb.AppendLine($"for: '{i}'");
				LAMS_Airpoint visiblePt = mgr_passed.MyAirpoints[Indices_VisibleAirpoint[i]];
				LAMS_AirpointRelationship rel_meToVisible = GetRelationshipUsingDestinationIndex( Indices_VisibleAirpoint[i] );

				foreach ( LAMS_AirpointRelationship rel_visblToObscured in visiblePt.MyRelationships )
				{
					if ( rel_visblToObscured.Index_destinationPoint == Index_WithinManagerList || Indices_VisibleAirpoint.Contains(rel_visblToObscured.Index_destinationPoint))
					{
						//mgr_passed.pingSkips++;
						continue;
					}

					int index_rel_meToObscured = GetRelationshipIndexWithDestinationIndex(rel_visblToObscured.Index_destinationPoint);
					LAMS_AirpointRelationship rel_meToObscured = MyRelationships[index_rel_meToObscured];
					float calculatedDistance = rel_meToVisible.Distance + rel_visblToObscured.Distance;

					if ( calculatedDistance < rel_meToObscured.Distance )
					{
						rel_meToObscured.Distance = calculatedDistance;
						rel_meToObscured.MakePathFromExisting(visiblePt, rel_visblToObscured.Path_destinationPoint);
						MyRelationships[index_rel_meToObscured] = rel_meToObscured;

						LAMS_Airpoint obscuredPt = mgr_passed.MyAirpoints[rel_visblToObscured.Index_destinationPoint];
						LAMS_AirpointRelationship rel_obscuredToVisible = obscuredPt.GetRelationshipUsingDestinationIndex(visiblePt.Index_WithinManagerList);
						int index_rel_obscuredToMe = obscuredPt.GetRelationshipIndexWithDestinationIndex(Index_WithinManagerList);
						LAMS_AirpointRelationship rel_obscuredToMe = obscuredPt.MyRelationships[index_rel_obscuredToMe];
						rel_obscuredToMe.Distance = calculatedDistance;
						rel_obscuredToMe.MakePathFromExisting(rel_obscuredToVisible.Path_destinationPoint, this);
						obscuredPt.MyRelationships[index_rel_obscuredToMe] = rel_obscuredToMe;
					}
				}
			}
		}

		public bool HaveRelationshipWithDestinationIndex( int index_passed )
		{
			if ( MyRelationships == null || MyRelationships.Count == 0 )
			{
				return false;
			}

			foreach ( LAMS_AirpointRelationship rel in MyRelationships )
			{
				if ( rel.Index_destinationPoint == index_passed )
				{
					return true;
				}
			}

			return false;
		}

		public LAMS_AirpointRelationship GetRelationshipUsingDestinationIndex( int index_passed )
		{
			for ( int i = 0; i < MyRelationships.Count; i++ )
			{
				if ( MyRelationships[i].Index_destinationPoint == index_passed )
				{
					return MyRelationships[i];
				}
			}

			Debug.LogError($"ERROR! {Index_WithinManagerList}.GetRelationshipIndexWithDestinationIndex('{index_passed}') wasn't able to find a relationship. Returning relationship.None...");
			return LAMS_AirpointRelationship.None;
		}

		public int GetRelationshipIndexWithDestinationIndex(int index_passed)
		{
			for ( int i = 0; i < MyRelationships.Count; i++ )
			{
				if ( MyRelationships[i].Index_destinationPoint == index_passed )
				{
					return i;
				}
			}

			Debug.LogError($"ERROR! {Index_WithinManagerList}.GetRelationshipIndexWithDestinationIndex('{index_passed}') wasn't able to find a relationship. Returning -1...");
			return -1;
		}
	}
}