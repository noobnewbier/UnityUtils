using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityUtils.Editor
{
    /// <summary>
    /// Smashing the following two repository together to make this. Did some fixing to make it usable but it's by no means meant to be maintainable/readable:
    /// 
    /// UI - https://github.com/marijnz/unity-autocomplete-search-field/blob/master/Assets/AutocompleteSearchField/Scripts/Editor/AutocompleteSearchField.cs
    /// Backend Logic -  https://www.clonefactor.com/wordpress/program/c/1809/
    /// </summary>
    public class AutoCompleteField
    {
        private static class Styles
        {
            public const float ResultHeight = 20f;
            public const float ResultsBorderWidth = 2f;
            public const float ResultsMargin = 15f;
            public const float ResultsLabelOffset = 2f;

            public static readonly GUIStyle EntryEven;
            public static readonly GUIStyle EntryOdd;
            public static readonly GUIStyle LabelStyle;
            public static readonly GUIStyle ResultsBorderStyle;

            static Styles()
            {
                EntryOdd = new GUIStyle("CN EntryBackOdd");
                EntryEven = new GUIStyle("CN EntryBackEven");
                ResultsBorderStyle = new GUIStyle("hostview");

                LabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    richText = true
                };
            }
        }

        private readonly Action<string>? _onInputChangedCallback;
        private readonly Action<string>? _onConfirmCallback;
        private readonly Func<IEnumerable<string>> _optionsFactory;
        private bool _isOptionValid;
        private List<string> _options = new();
        private string _value;

        public AutoCompleteField(SerializedProperty property,
            Func<IEnumerable<string>> optionsFactory,
            Action<string>? onValueChangedCallback = null) : this(s =>
            {
                property.stringValue = s;
                property.serializedObject.ApplyModifiedProperties();
                onValueChangedCallback?.Invoke(s);
            }, s =>
            {
                property.serializedObject.ApplyModifiedProperties();
                onValueChangedCallback?.Invoke(s);
            },
            optionsFactory,
            property.displayName,
            property.stringValue) { }

        private AutoCompleteField(Action<string>? onInputChangedCallback,
            Action<string>? onConfirmCallback,
            Func<IEnumerable<string>> optionsFactory,
            string label,
            string value)
        {
            _onInputChangedCallback = onInputChangedCallback;
            _onConfirmCallback = onConfirmCallback;
            _searchField = new SearchField();
            _optionsFactory = optionsFactory;
            _label = label;
            _value = value;

            _searchField.downOrUpArrowKeyPressed += OnDownOrUpArrowKeyPressed;
        }

        private static int MaxResults => 15;

        private readonly List<string> _candidates = new();
        private int _selectedIndex = -1;

        private readonly SearchField _searchField;

        private Vector2 _previousMousePosition;
        private bool _selectedIndexByMouse;

        private bool _shouldDrawCandidates;
        private readonly string _label;

        public void OnGUI()
        {
            var rect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));

            using (new GUILayout.HorizontalScope())
            {
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(_label));
                DoSearchField(rect);
            }

            DoCandidates(rect);
        }

        private void DoSearchField(Rect rect)
        {
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                _value = _searchField.OnGUI(rect, _value);

                if (changeCheck.changed)
                {
                    _onInputChangedCallback?.Invoke(_value);

                    _selectedIndex = -1;
                    _shouldDrawCandidates = true;
                }
            }

            if (HasSearchbarFocused())
            {
                RepaintFocusedWindow();
            }
        }

        private void OnDownOrUpArrowKeyPressed()
        {
            var current = Event.current;

            if (current.keyCode == KeyCode.UpArrow)
            {
                current.Use();
                _selectedIndex++;
                _selectedIndexByMouse = false;
            }
            else
            {
                current.Use();
                _selectedIndex--;
                _selectedIndexByMouse = false;
            }

            if (_selectedIndex >= _candidates.Count) _selectedIndex = _candidates.Count - 1;
            else if (_selectedIndex < 0) _selectedIndex = -1;
        }

        private void DoCandidates(Rect rect)
        {
            RefreshCandidates();
            if (_candidates.Count <= 0 || !_shouldDrawCandidates) return;

            var current = Event.current;
            rect.height = Styles.ResultHeight * Mathf.Min(MaxResults, _candidates.Count);
            rect.y -= rect.height + Styles.ResultsLabelOffset;
            rect.x += Styles.ResultsMargin;
            rect.width -= Styles.ResultsMargin * 2;

            var elementRect = rect;

            rect.height += Styles.ResultsBorderWidth;
            GUI.Label(rect, string.Empty, Styles.ResultsBorderStyle);

            var mouseIsInResultsRect = rect.Contains(current.mousePosition);

            if (mouseIsInResultsRect)
            {
                RepaintFocusedWindow();
            }

            var movedMouseInRect = _previousMousePosition != current.mousePosition;

            elementRect.x += Styles.ResultsBorderWidth;
            elementRect.y += rect.height - Styles.ResultsLabelOffset - 18;
            elementRect.width -= Styles.ResultsBorderWidth * 2;
            elementRect.height = Styles.ResultHeight;

            var didJustSelectIndex = false;

            for (var i = 0; i < _candidates.Count; i++)
            {
                if (current.type == EventType.Repaint)
                {
                    var style = i % 2 == 0 ?
                        Styles.EntryOdd :
                        Styles.EntryEven;

                    style.Draw(elementRect, false, false, i == _selectedIndex, false);

                    var labelRect = elementRect;
                    labelRect.x += Styles.ResultsLabelOffset;
                    GUI.Label(labelRect, _candidates[i], Styles.LabelStyle);
                }

                if (elementRect.Contains(current.mousePosition))
                {
                    if (movedMouseInRect)
                    {
                        _selectedIndex = i;
                        _selectedIndexByMouse = true;
                        didJustSelectIndex = true;
                    }

                    if (current.type == EventType.MouseDown)
                    {
                        OnConfirm(_candidates[i]);
                    }
                }

                elementRect.y -= Styles.ResultHeight;
            }

            if (current.type == EventType.Repaint && !didJustSelectIndex && !mouseIsInResultsRect && _selectedIndexByMouse)
            {
                _selectedIndex = -1;
            }

            if (GUIUtility.hotControl != _searchField.searchFieldControlID && GUIUtility.hotControl > 0
                || current.rawType == EventType.MouseDown && !mouseIsInResultsRect)
            {
                _shouldDrawCandidates = false;
            }
            
            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Escape)
            {
                _shouldDrawCandidates = false;
            }

            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Return && _selectedIndex >= 0)
            {
                OnConfirm(_candidates[_selectedIndex]);
            }

            if (current.type == EventType.Repaint)
            {
                _previousMousePosition = current.mousePosition;
            }
        }

        private void OnConfirm(string result)
        {
            _value = result;
            _onConfirmCallback?.Invoke(result);
            _onInputChangedCallback?.Invoke(result);
            _shouldDrawCandidates = false;
            RepaintFocusedWindow();
            GUIUtility.keyboardControl = 0; // To avoid Unity sometimes not updating the search field text
        }

        private bool HasSearchbarFocused()
        {
            return GUIUtility.keyboardControl == _searchField.searchFieldControlID;
        }

        private static void RepaintFocusedWindow()
        {
            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }
        }

        public void DirtyOption()
        {
            _isOptionValid = false;
        }

        /// <summary>
        /// Scraped from here, clean up somewhat to make it bottom line understandable
        /// </summary>
        private void RefreshCandidates()
        {
            if (!_isOptionValid)
            {
                _options = _optionsFactory().ToHashSet().ToList();
                _isOptionValid = true;
            }

            _candidates.Clear();

            // Start with - slow
            for (var i = 0; i < _options.Count && _candidates.Count < MaxResults; i++)
            {
                if (_options[i].ToLower().StartsWith(_value.ToLower()) && !_candidates.Contains(_options[i]))
                {
                    _candidates.Add(_options[i]);
                }
            }

            // Contains - very slow
            for (var i = 0; i < _options.Count && _candidates.Count < MaxResults; i++)
            {
                if (_options[i].ToLower().Contains(_value.ToLower()) && !_candidates.Contains(_options[i]))
                {
                    _candidates.Add(_options[i]);
                }
            }

            // Levenshtein Distance - very very slow.
            const int fuzzyMatchBias = 3;
            const float fuzzyThreshold = 0.5f;

            if (_value.Length >
                fuzzyMatchBias) // bias on input, hidden value to avoid doing it too early.) // have some empty space for matching.
            {
                var keywords = _value.ToLower();
                for (var i = 0; i < _options.Count && _candidates.Count < MaxResults; i++)
                {
                    var distance = LevenshteinDistance(_options[i], keywords);
                    var closeEnough = (int)(fuzzyThreshold * _options[i].Length) > distance;
                    if (closeEnough && !_candidates.Contains(_options[i]))
                    {
                        _candidates.Add(_options[i]);
                    }
                }
            }
        }

        /// <summary>Computes the Levenshtein Edit Distance between two enumerables.</summary>
        /// <param name="lhs">The first enumerable.</param>
        /// <param name="rhs">The second enumerable.</param>
        /// <returns>The edit distance.</returns>
        /// <see cref="https://en.wikipedia.org/wiki/Levenshtein_distance"/>
        public static int LevenshteinDistance(string lhs, string rhs)
        {
            lhs = lhs.ToLower();
            rhs = rhs.ToLower();

            var first = lhs.ToCharArray();
            var second = rhs.ToCharArray();
            return LevenshteinDistance<char>(first, second);
        }

        /// <summary>Computes the Levenshtein Edit Distance between two enumerables.</summary>
        /// <typeparam name="T">The type of the items in the enumerables.</typeparam>
        /// <param name="lhs">The first enumerable.</param>
        /// <param name="rhs">The second enumerable.</param>
        /// <returns>The edit distance.</returns>
        /// <see cref="https://blogs.msdn.microsoft.com/toub/2006/05/05/generic-levenshtein-edit-distance-with-c/"/>
        public static int LevenshteinDistance<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) where T : IEquatable<T>
        {
            // Convert the parameters into IList instances
            // in order to obtain indexing capabilities
            var first = lhs as IList<T> ?? new List<T>(lhs);
            var second = rhs as IList<T> ?? new List<T>(rhs);
            // Get the length of both.  If either is 0, return
            // the length of the other, since that number of insertions
            // would be required.
            int n = first.Count, m = second.Count;
            if (n == 0) return m;
            if (m == 0) return n;
            // Rather than maintain an entire matrix (which would require O(n*m) space),
            // just store the current row and the next row, each of which has a length m+1,
            // so just O(m) space. Initialize the current row.
            int curRow = 0, nextRow = 1;
            var rows = new[] { new int[m + 1], new int[m + 1] };
            for (var j = 0; j <= m; ++j)
                rows[curRow][j] = j;
            // For each virtual row (since we only have physical storage for two)
            for (var i = 1; i <= n; ++i)
            {
                // Fill in the values in the row
                rows[nextRow][0] = i;
                for (var j = 1; j <= m; ++j)
                {
                    var dist1 = rows[curRow][j] + 1;
                    var dist2 = rows[nextRow][j - 1] + 1;
                    var dist3 = rows[curRow][j - 1] +
                                (first[i - 1].Equals(second[j - 1]) ?
                                    0 :
                                    1);
                    rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
                }

                // Swap the current and next rows
                if (curRow == 0)
                {
                    curRow = 1;
                    nextRow = 0;
                }
                else
                {
                    curRow = 0;
                    nextRow = 1;
                }
            }

            // Return the computed edit distance
            return rows[curRow][m];
        }
    }
}