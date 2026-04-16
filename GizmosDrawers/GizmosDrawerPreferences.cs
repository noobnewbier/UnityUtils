using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Unity doesn't like you nesting the preference class, it just won't load.
    /// It does fucking save the file though, why it doesn't load it is beyond me.
    /// 
    /// Oh wait! Omg you are asking Unity for reasons, that's not a good sign.
    /// </summary>
    [FilePath("NonebNi/GizmosDrawerPreferences.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class GizmosDrawerPreferences : ScriptableSingleton<GizmosDrawerPreferences>
    {
        [SerializeField] private List<string> filteredCategories = new ();
        [SerializeField] private bool isDrawLabel;

         public bool IsDrawLabel
         {
             get => isDrawLabel;
             set
             {
                 if (value == isDrawLabel)
                 {
                     return;
                 }
                 
                 isDrawLabel = value; 
                 Save(true);
             }
         }

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