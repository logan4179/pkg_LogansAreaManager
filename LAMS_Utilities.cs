using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
{
    //NOTE: Hello from new project!

    public static class LAMS_Utilities
    {

    }

    /// <summary>
    /// Base interface for entities (enemies and npcs) that are part of the LogansAreaManagementSystem.  Implementing this interface 
    /// allows LAMS to correctly activate/deactivate entities based on 
    /// </summary>
    public interface I_LAMS_Entity
    {
        /// <summary>
        /// This method allows you to call any logic needed for when an entity becomes activated via an area. This is where you 
        /// can do your logic for how an enemy or npc gets activated when the player enters their room or an adjacent room.
        /// </summary>
        void ActivateMeViaArea();

		/// <summary>
		/// This method allows you to call any logic needed for when an entity becomes deactivated via an area. This is where you 
		/// can do your logic for how an enemy or npc gets deactivated when the player leaves their room or an adjacent room.
		/// </summary>
		void DeactivateMeViaArea();

	}
}