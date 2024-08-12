using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
{
	/// <summary>
	/// Manager class for LogansAreaManagementSystem. Typically there should be one of these per scene, as indicated by them having a singleton instance
	/// </summary>
    public class LAMS_Manager : MonoBehaviour
    {
		public static LAMS_Manager Instance;

		[Header("----------REFERENCE [AREAS]---------")]
		private List<LAMS_Area> AllAreas = new List<LAMS_Area>();
		/// <summary>The areas that are currently active in the scene.</summary>
		private List<LAMS_Area> Areas_currentlyActive = new List<LAMS_Area>();

		[SerializeField, Tooltip("If true, this causes all areas in scene to automatically performs their initialization on Awake() and Start() using this manager's singleton reference")] 
		public bool AutoInitializeAreas = true;

		private void Awake()
		{
			if( Instance == null )
			{
				Instance = this;
				Areas_currentlyActive = new List<LAMS_Area>();
			}
			else
			{
				Debug.LogError($"LAM ERROR! LAM_Manager singleton instance was not set to null on Awake. Is there more than one in the scene?");
			}
		}

		/// <summary>
		/// Turns off all registered areas except the one passed in. Calls EnteredAction() on the passed in area, making it the 
		/// 'starting area' that will begin activated. Call this method by an environmental manager, or similar script on Start()
		/// </summary>
		/// <param name="startArea"></param>
		public void InitializeSystem( LAMS_Area startArea )
		{
			foreach ( LAMS_Area area in AllAreas )
			{
				if ( area != startArea )
				{
					area.ExitedAction();
					area.ExitedAdjacentAreaAction();
					area.gameObject.SetActive(false);
				}
			}

			ChangeActiveArea(startArea);
		}

		public void RegsiterArea( LAMS_Area area_passed )
		{
			if( AllAreas == null )
			{
				AllAreas = new List<LAMS_Area>();
			}

			AllAreas.Add( area_passed );
		}

		/// <summary>
		/// Call this method when the player enters a new LAMS_Area.
		/// </summary>
		/// <param name="area_passed"></param>
		public void ChangeActiveArea( LAMS_Area area_passed )
		{
			print($"mgr.{nameof(ChangeActiveArea)}('{area_passed.name}'). currently active arreas: '{Areas_currentlyActive.Count}'");
			if ( Areas_currentlyActive != null )
			{
				foreach ( LAMS_Area area in Areas_currentlyActive )
				{
					if ( area != area_passed && !area_passed.AdjacentAreas.Contains(area) )
					{
						//s += $"'{area.name}' + ";
						print($"calling exit adjacent on '{area.name}'");
						area.ExitedAdjacentAreaAction();
						area.gameObject.SetActive(false);
					}
				}

				if ( area_passed.AdjacentAreas != null )
				{
					foreach ( LAMS_Area area in area_passed.AdjacentAreas )
					{
						area.gameObject.SetActive(true);
						area.EnteredAdjacentAreaAction();
					}
				}
			}

			area_passed.EnteredAction();
			Areas_currentlyActive = new List<LAMS_Area>( area_passed.AdjacentAreas );
			Areas_currentlyActive.Add( area_passed );
		}
	}
}