using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace UnityUtils.Editor
{
    public static class ReverseAnimationContext
    {
        [MenuItem("Assets/Create Reversed Clip", false, 14)]
        private static void ReverseClip()
        {
            var directoryPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
            var fileName = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
            var fileExtension = Path.GetExtension(AssetDatabase.GetAssetPath(Selection.activeObject));
            Debug.Assert(fileName != null, nameof(fileName) + " != null");

            fileName = fileName.Split('.')[0];
            var copiedFilePath = directoryPath + Path.DirectorySeparatorChar + fileName + "_Reversed" + fileExtension;

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(Selection.activeObject), copiedFilePath);

            var clip = (AnimationClip) AssetDatabase.LoadAssetAtPath(copiedFilePath, typeof(AnimationClip));

            if (clip == null)
            {
                return;
            }
            var clipLength = clip.length;
            var curves = AnimationUtility.GetAllCurves(clip, true);
            clip.ClearCurves();
            foreach (var curve in curves)
            {
                var keys = curve.curve.keys;
                var keyCount = keys.Length;
                var postWrapMode = curve.curve.postWrapMode;
                curve.curve.postWrapMode = curve.curve.preWrapMode;
                curve.curve.preWrapMode = postWrapMode;
                for (var i = 0; i < keyCount; i++)
                {
                    var k = keys[i];
                    k.time = clipLength - k.time;
                    var tmp = -k.inTangent;
                    k.inTangent = -k.outTangent;
                    k.outTangent = tmp;
                    keys[i] = k;
                }

                curve.curve.keys = keys;
                clip.SetCurve(curve.path, curve.type, curve.propertyName, curve.curve);
            }

            var events = AnimationUtility.GetAnimationEvents(clip);
            if (events.Length > 0)
            {
                foreach (var t in events)
                    t.time = clipLength - t.time;

                AnimationUtility.SetAnimationEvents(clip, events);
            }

            UnityEngine.Debug.Log("Animation reversed!");
        }

        [MenuItem("Assets/Create Reversed Clip", true)]
        private static bool ReverseClipValidation()
        {
            return Selection.activeObject is AnimationClip;
        }

        public static AnimationClip GetSelectedClip()
        {
            var clips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Assets);
            if (clips.Length > 0)
            {
                return clips[0] as AnimationClip;
            }
            return null;
        }
    }
}