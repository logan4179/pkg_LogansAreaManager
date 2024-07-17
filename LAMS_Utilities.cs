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
        void ActivateMeViaArea();

		void DeactivateMeViaArea();

	}
}