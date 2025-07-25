#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using JHS.Library.UINavigator.Runtime;
using JHS.Library.UINavigator.Runtime.Animation;
using JHS.Library.UINavigator.Runtime.Modal;
using JHS.Library.UINavigator.Runtime.Page;
using JHS.Library.UINavigator.Runtime.Sheet;
using JHS.Library.UINavigator.Runtime.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace JHS.Library.UINavigator.Editor
{
    [CustomEditor(typeof(UIViewArea), true)]
    public class UIViewAreaEditor : UnityEditor.Editor
    {
        #region Fields

        SerializedProperty script;
        SerializedProperty viewShowAnimation;
        SerializedProperty viewHideAnimation;
        SerializedProperty viewAreaName;
        SerializedProperty isDontDestroyOnLoad;

        readonly string[] tabArray = { "Move", "Rotate", "Scale", "Fade" };
        readonly string[] toggleArray = { "On", "Off" };
        int selectionValue;

        #endregion

        #region Properties

        UIViewArea Target => target as UIViewArea;
        protected virtual string[] PropertyToExclude() => new[]
        {
            "m_Script", 
            $"<{nameof(UIViewArea.ShowAnimation)}>k__BackingField", 
            $"<{nameof(UIViewArea.HideAnimation)}>k__BackingField",
            $"<{nameof(UIViewArea.ContainerName)}>k__BackingField",
            "isDontDestroyOnLoad"
        };

        #endregion

        #region Unity Lifecycle

        protected virtual void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            viewShowAnimation = serializedObject.FindProperty($"<{nameof(UIViewArea.ShowAnimation)}>k__BackingField");
            viewHideAnimation = serializedObject.FindProperty($"<{nameof(UIViewArea.HideAnimation)}>k__BackingField");
            viewAreaName = serializedObject.FindProperty($"<{nameof(UIViewArea.ContainerName)}>k__BackingField");
            isDontDestroyOnLoad = serializedObject.FindProperty("isDontDestroyOnLoad");
        }

        #endregion

        #region GUI Process

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(script);
            GUI.enabled = true;

            string prefix = $"Container - {(string.IsNullOrWhiteSpace(Target.ContainerName) ? "" : Target.ContainerName + " ")}";
            if (Target is SheetViewArea && Target.name !=  $"{prefix}Sheet" && GUILayout.Button($"Rename to '{prefix}Sheet'"))
            {
                Target.name = $"{prefix}Sheet";
            }
            else if (Target is PageViewArea && Target.name !=  $"{prefix}Page" && GUILayout.Button($"Rename to '{prefix}Page'"))
            {
                Target.name = $"{prefix}Page";
            }
            else if (Target is ModalViewArea && Target.name !=  $"{prefix}Modal" && GUILayout.Button($"Rename to '{prefix}Modal'"))
            {
                Target.name = $"{prefix}Modal";
            }
            
            DrawAnimationSetting();
            DrawViewAreaSetting();
            
            EditorGUILayout.Space(9);
            AdditionalGUIProcess();

            DrawPropertiesExcluding(serializedObject, PropertyToExclude());

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        void DrawAnimationSetting()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Animation Setting");
                var tabArea = EditorGUILayout.BeginVertical();
                {
                    tabArea = new Rect(tabArea) { xMin = 18, height = 24 };
                    GUI.Box(tabArea, GUIContent.none, GUI.skin.window);
                    selectionValue = GUI.Toolbar(tabArea, selectionValue, tabArray);
                    EditorGUILayout.Space(24);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(9);
                switch (selectionValue)
                {
                    case 0:
                        DrawShowAndHideAnimationSetting<MoveShowAnimation, MoveHideAnimation>("moveAnimation");
                        break;
                    case 1:
                        DrawShowAndHideAnimationSetting<RotateShowAnimation, RotateHideAnimation>("rotateAnimation");
                        break;
                    case 2:
                        DrawShowAndHideAnimationSetting<ScaleShowAnimation, ScaleHideAnimation>("scaleAnimation");
                        break;
                    case 3:
                        DrawShowAndHideAnimationSetting<FadeShowAnimation, FadeHideAnimation>("fadeAnimation");
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(9);
        }

        void DrawViewAreaSetting()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Container Setting");
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(viewAreaName);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("IsDontDestroyOnLoad"));
                        var select = GUILayout.Toolbar(isDontDestroyOnLoad.boolValue ? 0 : 1, toggleArray);
                        isDontDestroyOnLoad.boolValue = select == 0;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        void DrawShowAndHideAnimationSetting<TShow, THide>(string propertyRelative) where TShow : class where THide : class
        {
            var showArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(showArea, GUIContent.none);
                DrawReferenceField<TShow>(viewShowAnimation.FindPropertyRelative(propertyRelative), "Show Animation");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(9);
            var hideArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(hideArea, GUIContent.none);
                DrawReferenceField<THide>(viewHideAnimation.FindPropertyRelative(propertyRelative), "Hide Animation");
            }
            EditorGUILayout.EndVertical();
        }

        void DrawReferenceField<T>(SerializedProperty target, string title = null) where T : class
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);

                EditorGUILayout.Space(7);
                EditorGUI.indentLevel++;
                {
                    target.isExpanded = true;
                    EditorGUILayout.PropertyField(target, GUIContent.none, true, GUILayout.ExpandHeight(false));
                }
                EditorGUI.indentLevel--;

                if (title != null) DrawTitleField(title, new Rect(area) { yMin = area.yMin, height = 24 });

                // 드랍다운 처리
                var dropdownArea = new Rect(GUILayoutUtility.GetLastRect()) { xMin = 18, height = 24 };
                GUI.Box(dropdownArea, GUIContent.none, GUI.skin.window);

                EditorGUILayout.Space(1);
                var options = ReflectionUtility.GetDerivedTypes<T>().Select(x => x.Name).Prepend("None").ToArray();
                int currentIndex = target.managedReferenceValue != null ? Array.FindIndex(options, option => option == target.managedReferenceValue.GetType().Name) : 0;
                int selectedIndex = EditorGUILayout.Popup(currentIndex, options);
                if (currentIndex != selectedIndex)
                {
                    target.managedReferenceValue = selectedIndex == 0 ? null : Activator.CreateInstance(ReflectionUtility.GetDerivedTypes<T>()[selectedIndex - 1]) as T;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected static void DrawTitleField(string title, Rect rect = default)
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(rect == default ? new Rect(area) { xMin = 18 } : rect, GUIContent.none, GUI.skin.window);
                var targetStyle = new GUIStyle();
                targetStyle.fontSize = 15;
                targetStyle.padding.left = 10;
                targetStyle.normal.textColor = Color.white;
                targetStyle.alignment = TextAnchor.MiddleLeft;
                targetStyle.fontStyle = FontStyle.Bold;
                if (rect == default) EditorGUILayout.LabelField(title, targetStyle, GUILayout.Height(25));
                else EditorGUI.LabelField(rect, title, targetStyle);
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Virtual Methods

        protected virtual void AdditionalGUIProcess()
        {
        }

        #endregion
    }
}
#endif