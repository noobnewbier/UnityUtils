using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityUtils
{
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
            foreach (var filter in Preferences.instance.FilteredCategories)
            {
                Preferences.instance.SetFilter(filter, false);
            }
        }

        internal static void HideAll()
        {
            foreach (var request in Requests)
            {
                Preferences.instance.SetFilter(request.RequestCategory, true);
            }
        }

        [FilePath("NonebNi/GizmosDrawer.Preferences.asset", FilePathAttribute.Location.PreferencesFolder)]
        internal class Preferences : ScriptableSingleton<Preferences>
        {
            [SerializeField] private List<string> filteredCategories = new ();

            internal IEnumerable<string> FilteredCategories => filteredCategories;

            internal void SetFilter(string filterString, bool isFilter)
            {
                var isDirty = false;
                if (isFilter)
                {
                    if (!filteredCategories.Contains(filterString))
                    {
                        filteredCategories.Add(filterString);
                        isDirty = true;
                    }
                }
                else
                {
                    isDirty = filteredCategories.Remove(filterString);
                }

                if (isDirty)
                {
                    EditorUtility.SetDirty(this);
                    Save(true);
                }
            }
        }
    }
}
#endif