using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogansAreaManagementSystem
{
    public static class LAMS_Utilities
    {

    }

    public interface I_LAMS_Entity
    {
        /// <summary>
        /// This method allows you to call any logic needed for when an entity becomes activated via an area. This is useful for 
        /// things like adding an entity to a manager's list of active entities once it's activated, for example. You can just  
        /// leave this method blank if nothing like this is needed, and it won't hurt the functioning of this system.
        /// </summary>
        void ActivateMeViaArea();

		/// <summary>
		/// This method allows you to call any logic needed for when an entity becomes deactivated via an area. This is useful for 
		/// things like removing an entity from a manager's list of active entities once it's area is deactivated, for example. You 
        /// can just leave this method blank if nothing like this is needed, and it won't hurt the functioning of this system.
		/// </summary>
		void DeactivateMeViaArea();

	}
}