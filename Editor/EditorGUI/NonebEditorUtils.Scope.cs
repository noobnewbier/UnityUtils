using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static partial class NonebEditorGUI
    {
        public class HandlesColorScope : IDisposable
        {
            private readonly Color _cacheColor;
            private bool _disposed;

            public HandlesColorScope(Color color)
            {
                _cacheColor = Handles.color;
                Handles.color = color;
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private void CloseScope()
            {
                Handles.color = _cacheColor;
            }

            ~HandlesColorScope()
            {
                DoDispose(false);
            }
        }

        public class ErrorColorScope : IDisposable
        {
            private readonly Color _cacheColor;
            private readonly bool _hasError;
            private bool _disposed;

            public ErrorColorScope(bool hasError = true)
            {
                _hasError = hasError;
                _cacheColor = GUI.color;
                if (_hasError) GUI.color = Color.red;
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private void CloseScope()
            {
                if (_hasError) GUI.color = _cacheColor;
            }

            ~ErrorColorScope()
            {
                DoDispose(false);
            }
        }

        public class GizmosColorScope : IDisposable
        {
            private readonly Color _cacheGizmosColor;
            private readonly Color _cacheHandleColor;
            private bool _disposed;

            public GizmosColorScope(Color color)
            {
                _cacheGizmosColor = Gizmos.color;
                _cacheHandleColor = Handles.color;
                Gizmos.color = color;
                Handles.color = color;
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private void CloseScope()
            {
                Gizmos.color = _cacheGizmosColor;
                Handles.color = _cacheHandleColor;
            }

            ~GizmosColorScope()
            {
                DoDispose(false);
            }
        }

        public class EditorLabelWidthScope : IDisposable
        {
            private readonly float _cacheLabelWidth;
            private bool _disposed;

            public EditorLabelWidthScope(string targetLabel) : this(EditorStyles.label.CalcSize(new GUIContent(targetLabel)).x) { }

            public EditorLabelWidthScope(float targetWidth)
            {
                _cacheLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = targetWidth;
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private void CloseScope()
            {
                EditorGUIUtility.labelWidth = _cacheLabelWidth;
            }

            ~EditorLabelWidthScope()
            {
                DoDispose(false);
            }
        }

        public class AssetDatabaseEditingScope : IDisposable
        {
            private bool _disposed;

            public AssetDatabaseEditingScope()
            {
                AssetDatabase.StartAssetEditing();
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private static void CloseScope()
            {
                AssetDatabase.StopAssetEditing();
            }

            ~AssetDatabaseEditingScope()
            {
                DoDispose(false);
            }
        }
    }
}