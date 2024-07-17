using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
{
	public class LAMS_AirspaceManager : MonoBehaviour
	{
		[Header("[[----------- REFERENCE ------------]]")]
		public List<LAMS_Airpoint> MyAirpoints;

		[Header("[[----------- OTHER ------------]]")]
		public int ObscuringMask;
		public int Index_FocusedAirpoint = -1;
		public LAMS_Airpoint FocusedAirpoint
		{
			get
			{
				return MyAirpoints[Index_FocusedAirpoint];
			}
		}

		[Header("[[----------- DEBUG ------------]]")]
		[SerializeField] string dbgString;

		[ContextMenu("call SetupAirspace()")]
		public void SetupAirspace()
		{
			//int captureContinues = 0;
			//int pingSkips = 0;
			//StringBuilder sb = new StringBuilder();
			//sb.AppendLine($"SetupAirspace() on '{MyAirpoints.Count}' air points. Clearing visible relationships...");
			//DateTime dtStart = DateTime.Now;

			ClearVisibleIndices();
			//sb.AppendLine($"ClearVisible took: '{DateTime.Now.Subtract(dtStart)}', now capturing visible...");
			ResetIndices();

			//DateTime dtCaptureVisStart = DateTime.Now;
			foreach ( LAMS_Airpoint airpoint in MyAirpoints )
			{
				airpoint.CaptureVisibleAndObscured( this, ObscuringMask );
			}
			//sb.AppendLine($"Capture visible end. took: '{DateTime.Now.Subtract(dtCaptureVisStart)}', captureContinues: '{captureContinues}'");

			//MyAirpoints[Index_FocusedAirpoint].CaptureVisible( MyAirpoints, EnvironmentManager_cached.Mask_EnvSolid );
			//DateTime dtPingStart = DateTime.Now;
			foreach ( LAMS_Airpoint pt in MyAirpoints )
			{
				DateTime dtPt = DateTime.Now;
				pt.Ping(this);
				TimeSpan tsPt = DateTime.Now.Subtract(dtPt);
				//sb.AppendLine($"pt ping took: '{tsPt}'");

				if ( tsPt > new TimeSpan(0, 0, 6) )
				{
					//sb.AppendLine($"ping operation took too long. Breaking...");
					Debug.LogError("ping operation took too long. Breaking...");
					return;
				}

			}
			//sb.AppendLine($"entire ping operation took: '{DateTime.Now.Subtract(dtPingStart)}'. Had '{pingSkips}' skips");

			//sb.AppendLine($"End of setupAirspace(). Took: '{DateTime.Now.Subtract(dtStart)}'");
			//Debug.Log(sb.ToString());
		}

		[ContextMenu("call ResetIndices()")]
		public void ResetIndices()
		{
			for (int i = 0; i < MyAirpoints.Count; i++)
			{
				MyAirpoints[i].Index_WithinManagerList = i;
			}
		}

		[ContextMenu("call ClearVisibleIndices()")]
		public void ClearVisibleIndices()
		{
			foreach ( LAMS_Airpoint pt in MyAirpoints )
			{
				pt.Indices_VisibleAirpoint.Clear();
				pt.MyRelationships.Clear();
			}
		}

		[ContextMenu("call DeleteFocusedAirpoint()")]
		public void DeleteFocusedAirpoint()
		{
			MyAirpoints.RemoveAt(Index_FocusedAirpoint);
		}

		[ContextMenu("call ClearAirspace()")]
		public void ClearAirspace()
		{
			MyAirpoints.Clear();
		}

		public List<Vector3> FoundPath;
		public List<Vector3> FindPath( Vector3 origin_passed, Vector3 destination_passed )
		{
			//StringBuilder sb = new StringBuilder($"FindPath('{origin_passed}', '{destination_passed}')\n");
			DateTime dt_start = DateTime.Now;
			List<Vector3> constructedPath = new List<Vector3>() { origin_passed };

			if ( !Physics.Linecast(origin_passed, destination_passed, ObscuringMask) )
			{
				//sb.AppendLine("Found could linecast straight to the point.");
			}
			else
			{
				#region FIND CLOSEST AIRPOINTS -------------------------------------------
				int index_closestToOrigin = -1;
				float dist_closestToOrigin = float.MaxValue;
				int index_closestToDestination = -1;
				float dist_closestToDestination = float.MaxValue;
				for (int i = 0; i < MyAirpoints.Count; i++)
				{
					float calculatedDistToOrigin = Vector3.Distance(origin_passed, MyAirpoints[i].Position);
					if (calculatedDistToOrigin < dist_closestToOrigin)
					{
						index_closestToOrigin = i;
						dist_closestToOrigin = calculatedDistToOrigin;
					}

					float calculatedDistToDestinationn = Vector3.Distance(destination_passed, MyAirpoints[i].Position);
					if (calculatedDistToDestinationn < dist_closestToDestination)
					{
						index_closestToDestination = i;
						dist_closestToDestination = calculatedDistToDestinationn;
					}
				}
				#endregion
				//sb.AppendLine($"Found closest pt to origin: '{index_closestToOrigin}', closest pt to destination: '{index_closestToDestination}'");

				#region CALCULATE THE BEST START AND STOP INDEX ---------------------------
				List<int> pathIndices = new List<int> { index_closestToOrigin };
				pathIndices.AddRange(MyAirpoints[index_closestToOrigin].GetRelationshipUsingDestinationIndex(index_closestToDestination).Path_destinationPoint);
				//sb.AppendLine($"pathIndices from relationship has a count of: '{pathIndices.Count}'. Now deciding best index to start and end on...");

				int index_pathIndices_start = 0;
				int index_pathIndices_end = pathIndices.Count;
				bool finishedFindingStart = false;
				bool finishedFindingEnd = false;

				for ( int i = 0; i < pathIndices.Count; i++ )
				{
					//sb.AppendLine($"linecasting from pathIndices[{i}]: '{pathIndices[i]}'");

					if ( !finishedFindingStart )
					{
						if ( !Physics.Linecast(origin_passed, MyAirpoints[pathIndices[i]].Position, ObscuringMask) )
						{
							//sb.AppendLine($"linecast to origin was unobstructed. index_pathIndices_start now '{index_pathIndices_start}'");
							index_pathIndices_start = i;
						}
						else
						{
							//sb.AppendLine("linecast for index_pathIndices_start to origin was apparently obstructed.");
							finishedFindingStart = true;
						}
					}

					if ( !finishedFindingEnd )
					{
						int i_rev = pathIndices.Count - (i + 1);
						if ( !Physics.Linecast(destination_passed, MyAirpoints[pathIndices[i_rev]].Position, ObscuringMask) )
						{
							//sb.AppendLine($"linecast to destination was unobstructed. index_pathIndices_end now '{index_pathIndices_end}'");
							index_pathIndices_end = i_rev;
						}
						else
						{
							//sb.AppendLine("linecast for index_pathIndices_end to origin was apparently obstructed.");
							finishedFindingEnd = true;
						}
					}

					if ( finishedFindingStart && finishedFindingEnd )
					{
						//sb.AppendLine($"found that I've already found both the start and end at iteration: '{i}' out of: '{pathIndices.Count}'. Breaking prematurely...");
						break;
					}
				}
				#endregion
				//sb.AppendLine($"Finished finding best startpoint: '{index_pathIndices_start}', and endpoint: '{index_pathIndices_end}'");
				//print($"early printout: \n{sb}");
				#region CONSTRUCT PATH ---------------------------------------------
				for (int i = index_pathIndices_start; i < (index_pathIndices_end + 1); i++)
				{
					//print($"i: '{i}'");
					constructedPath.Add(MyAirpoints[pathIndices[i]].Position); //ERROR here. argumentoutofrangeexception
				}
				//sb.AppendLine($"constructedPath.Count: '{constructedPath.Count}'");

				#endregion
			}

			constructedPath.Add(destination_passed);
			//sb.AppendLine($"end of FindPath(). Operation took: '{DateTime.Now.Subtract(dt_start)}'");
			//print(sb);
			FoundPath = constructedPath;
			return constructedPath;
		}
	}
}