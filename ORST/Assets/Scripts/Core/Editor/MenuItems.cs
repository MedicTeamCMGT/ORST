using Oculus.Interaction.HandGrab.Visuals;
using Oculus.Interaction.Input;
using ORST.Core.Interactions;
using UnityEditor;
using UnityEngine;

namespace ORST.Core.Editor {
    public static class MenuItems {
        [MenuItem("CONTEXT/HandPuppet/Save Hand Pose (Left)")]
        private static void HandPuppet_SaveHandPoseLeft(MenuCommand command) {
            if (command.context is not HandPuppet handPuppet) {
                return;
            }

            HandPuppet_SaveHandPose(handPuppet, Handedness.Left);
        }

        [MenuItem("CONTEXT/HandPuppet/Save Hand Pose (Right)")]
        private static void HandPuppet_SaveHandPoseRight(MenuCommand command) {
            if (command.context is not HandPuppet handPuppet) {
                return;
            }

            HandPuppet_SaveHandPose(handPuppet, Handedness.Right);
        }

        private static void HandPuppet_SaveHandPose(HandPuppet handPuppet, Handedness handedness) {
            string path = EditorUtility.SaveFilePanel("Save Hand Pose Data", "", "HandPoseData", "asset");
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            string relativePath = path.Replace(Application.dataPath, "Assets");
            HandPoseData handPoseData = ScriptableObject.CreateInstance<HandPoseData>();
            handPoseData.InitializeFromJointCollection(new JointCollection(handPuppet.JointMaps), handedness);
            AssetDatabase.CreateAsset(handPoseData, relativePath);
            AssetDatabase.SaveAssets();

            EditorApplication.delayCall += () => {
                Selection.activeObject = handPoseData;
                EditorGUIUtility.PingObject(handPoseData);
            };
        }
    }
}