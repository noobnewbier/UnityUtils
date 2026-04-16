using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityUtils
{
    //todo: why is it not saving prefs
    //todo: accent grass thinking.
    /// <summary>
    /// Stuffs that mostly exist for editor purposes
    /// </summary>
    public partial class GizmosDrawer
    {
        internal static void DismissAll()
        {
            Requests.Clear();
        }
        
        internal static void ShowAll()
        {
            foreach (var filter in GizmosDrawerPreferences.instance.FilteredCategories)
            {
                GizmosDrawerPreferences.instance.SetFilter(filter, false);
            }
        }

        internal static void HideAll()
        {
            foreach (var request in Requests)
            {
                GizmosDrawerPreferences.instance.SetFilter(request.RequestCategory, true);
            }
        }
    }
}
#endif