using System;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace UnityUtils
{
    /// <summary>
    /// Stuffs that mostly exist for editor purposes
    /// </summary>
    public partial class GizmosDrawer
    {
        internal static void ClearRequests()
        {
            Requests.Clear();
        }

        internal static void ShowAll()
        {
            FilteredCategories.Clear();
        }

        internal static void HideAll()
        {
            FilteredCategories.Clear();
            foreach (var request in Requests)
            {
                FilteredCategories.Add(request.RequestCategory);
            }
        }

        internal static void DismissAll()
        {
            Requests.Clear();
        }
    }
}
#endif