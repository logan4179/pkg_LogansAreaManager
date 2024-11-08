using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
{
    public class LAMS_Area : MonoBehaviour
    {
		private LAMS_Manager mgr;
		
		public List<LAMS_Area> AdjacentAreas = new List<LAMS_Area>();
		[Space(10f)]

		public LAMS_AirspaceManager AirspaceManager;
		[Tooltip("GameObjects that should be activated/visible even when an adjacent area is active, and deactivated when this area isn't being triggered as an adjacent area.")]
		public List<GameObject> ContainedObjects_adjacentlyVisible = new List<GameObject>();
		[Tooltip("GameObjects that should only be active when this area is occupied, and inactive otherwise.")]
		public List<GameObject> StrictlyContainedObjects = new List<GameObject>();
		[Space(10f)]

		private List<I_LAMS_Entity> entitiesInArea = new List<I_LAMS_Entity>();

		[Header("---------------[[ DEBUG ]]-----------------")]
		public bool AmDebugging = false;

		private void Awake()
		{
			//print($"area start. singleton null: '{LAMS_Manager.Instance == null}'");
			if( LAMS_Manager.Instance != null && LAMS_Manager.Instance.AutoInitializeAreas )
			{
				LAMS_Manager.Instance.RegsiterArea( this );
				RegisterManager( LAMS_Manager.Instance );
			}
		}

		public void RegisterManager( LAMS_Manager mgr_passed )
		{
			mgr = mgr_passed;
		}

		/// <summary>
		/// Call this when the player enters this area.
		/// </summary>
		public void EnteredAction()
		{
			if( AmDebugging )
			{
				print($"{name}.{nameof(EnteredAction)}");
			}
			activateContainedObjects();
			activateEntities();
		}

		/// <summary>
		/// Call this when the player enters an area adjacent to this one.
		/// </summary>
		public void EnteredAdjacentAreaAction()
		{
			if (AmDebugging)
			{
				print($"{name}.{nameof(EnteredAdjacentAreaAction)}");
			}
			activateAdjacentlyVisibleObjects();
			activateEntities();
		}

		private void activateContainedObjects()
		{
			if ( StrictlyContainedObjects != null )
			{
				foreach ( GameObject go in StrictlyContainedObjects )
				{
					go.SetActive( true );
				}
			}
		}

		private void activateAdjacentlyVisibleObjects()
		{
			if ( ContainedObjects_adjacentlyVisible != null )
			{
				foreach ( GameObject go in ContainedObjects_adjacentlyVisible )
				{
					go.SetActive( true );
				}
			}
		}

		private void activateEntities()
		{
			if ( entitiesInArea != null )
			{
				foreach ( I_LAMS_Entity entity in entitiesInArea)
				{
					entity.ActivateMeViaArea();
				}
			}
		}

		/// <summary>
		/// Call when the player occupies neither this area, nor any areas adjacent to this area.
		/// </summary>
		public void ExitedAction()
		{
			if (AmDebugging)
			{
				print($"{name}.{nameof(ExitedAction)}");
			}

			deactivateContainedObjects();
		}

		/// <summary>
		/// Call when the player exits an area that is adjacent to this area.
		/// </summary>
		public void ExitedAdjacentAreaAction()
		{
			print($"{name}.{nameof(ExitedAdjacentAreaAction)}()");
			
			deactivateAdjacentlyVisibleObjects();
			deactivateEntities();
		}

		private void deactivateContainedObjects()
		{
			if ( StrictlyContainedObjects != null )
			{
				foreach ( GameObject go in StrictlyContainedObjects )
				{
					go.SetActive(false);
				}
			}
		}

		private void deactivateAdjacentlyVisibleObjects( LAMS_Area activeArea_passed = null )
		{
			if ( ContainedObjects_adjacentlyVisible != null )
			{
				foreach ( GameObject go in ContainedObjects_adjacentlyVisible )
				{
					if ( activeArea_passed != null )
					{
						bool keepIt = false;
						foreach ( LAMS_Area area in activeArea_passed.AdjacentAreas )
						{
							if ( area.ContainedObjects_adjacentlyVisible.Contains(go) )
							{
								keepIt = true;
								break;
							}
						}

						if (!keepIt)
						{
							go.SetActive(false);
						}
					}
					else
					{
						go.SetActive(false);
					}
				}
			}
		}

		private void deactivateEntities( LAMS_Area activeArea_passed = null )
		{
			if ( entitiesInArea != null )
			{
				foreach ( I_LAMS_Entity entity in entitiesInArea )
				{
					if ( activeArea_passed != null )
					{
						bool keepIt = false;
						foreach ( LAMS_Area area in activeArea_passed.AdjacentAreas )
						{
							if ( area.entitiesInArea.Contains(entity) )
							{
								keepIt = true;
								break;
							}
						}

						if ( !keepIt )
						{
							entity.DeactivateMeViaArea();
						}
					}
					else
					{
						entity.DeactivateMeViaArea();
					}
				}
			}
		}

		public void RegisterEntity( I_LAMS_Entity entity_passed )
		{
			if ( entitiesInArea == null )
			{
				entitiesInArea = new List<I_LAMS_Entity>();
			}

			if ( !entitiesInArea.Contains(entity_passed) )
			{
				entitiesInArea.Add( entity_passed );
			}
		}

		public void UnRegisterEntity( I_LAMS_Entity entity_passed )
		{
			if ( entitiesInArea == null )
			{
				entitiesInArea = new List<I_LAMS_Entity>();
			}

			if ( entitiesInArea.Contains(entity_passed) )
			{
				entitiesInArea.Remove( entity_passed );
			}
		}
	}
}