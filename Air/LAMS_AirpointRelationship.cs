using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LogansAreaManagementSystem
{
	[System.Serializable]
	public struct LAMS_AirpointRelationship
	{
		public int Index_destinationPoint;

		public List<int> Path_destinationPoint;

		public float Distance;

		public bool AmVisible;

		private static readonly LAMS_AirpointRelationship noneRelationship = new LAMS_AirpointRelationship(-1, -1f, false);

		public LAMS_AirpointRelationship(int index_passed, float dist_passed, bool amVisbl_passed)
		{
			Index_destinationPoint = index_passed;
			Path_destinationPoint = new List<int>();
			Distance = dist_passed;
			AmVisible = amVisbl_passed;
		}

		public void MakePathFromExisting( LAMS_Airpoint startPt_passed, List<int> path_passed )
		{
			Path_destinationPoint.Clear();
			Path_destinationPoint.Add(startPt_passed.Index_WithinManagerList);
			Path_destinationPoint.AddRange(path_passed);
		}

		public void MakePathFromExisting( List<int> path_passed, LAMS_Airpoint endPt_passed )
		{
			Path_destinationPoint.Clear();
			Path_destinationPoint.AddRange(path_passed);
			Path_destinationPoint.Add(endPt_passed.Index_WithinManagerList);
		}


		public static LAMS_AirpointRelationship None
		{
			get
			{
				return noneRelationship;
			}
		}
	}
}