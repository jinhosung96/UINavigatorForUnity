#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System;
using System.Linq;
using JHS.Library.UINavigator.Runtime;
using JHS.Library.UINavigator.Runtime.Animation;
using JHS.Library.UINavigator.Runtime.Util;
using UnityEditor;
using UnityEngine;

namespace JHS.Library.UINavigator.Editor
{
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : UnityEditor.Editor
    {
        #region Fields

        SerializedProperty script;
        SerializedProperty animationSetting;
        SerializedProperty showAnimation;
        SerializedProperty hideAnimation;
        SerializedProperty isRecycle;
        SerializedProperty enableBackdropButton;

        readonly string[] animationTabArray = { "Move", "Rotate", "Scale", "Fade" };
        readonly string[] toggleArray = { "On", "Off" };
        int animationSelectionValue;

        #endregion

        #region Properties

        UIView Target => target as UIView;

        protected virtual string[] PropertyToExclude() => new[]
        {
            "m_Script", 
            "showAnimation", 
            "hideAnimation", 
            "animationSetting",
            "<IsRecycle>k__BackingField",
            "<EnableBackdropButton>k__BackingField"
        };

        SerializedProperty ShowAnimation
        {
            get
            {
                SerializedProperty targetAnimation;
                if (animationSetting.enumValueIndex == 0)
                {
                    var parentContainer = Target.GetComponentInParent<UIViewArea>();
                    if (parentContainer)
                    {
                        var container = new SerializedObject(parentContainer);
                        targetAnimation = container.FindProperty($"<{nameof(UIViewArea.ShowAnimation)}>k__BackingField");
                    }
                    else return null;
                }
                else targetAnimation = showAnimation;

                return targetAnimation;
            }
        }

        SerializedProperty HideAnimation
        {
            get
            {
                SerializedProperty targetAnimation;
                if (animationSetting.enumValueIndex == 0)
                {
                    var parentContainer = Target.GetComponentInParent<UIViewArea>();
                    if (parentContainer)
                    {
                        var container = new SerializedObject(parentContainer);
                        targetAnimation = container.FindProperty($"<{nameof(UIViewArea.HideAnimation)}>k__BackingField");
                    }
                    else return null;
                }
                else targetAnimation = hideAnimation;

                return targetAnimation;
            }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            animationSetting = serializedObject.FindProperty("animationSetting");
            showAnimation = serializedObject.FindProperty("showAnimation");
            hideAnimation = serializedObject.FindProperty("hideAnimation");
            isRecycle = serializedObject.FindProperty("<IsRecycle>k__BackingField");
            enableBackdropButton = serializedObject.FindProperty("<EnableBackdropButton>k__BackingField");
        }

        #endregion

        #region GUI Process

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(script);
            GUI.enabled = true;
            
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Animation Setting");
                var settingTabArea = EditorGUILayout.BeginVertical();
                {
                    settingTabArea = new Rect(settingTabArea) { xMin = 18, height = 18 };
                    GUI.Box(settingTabArea, GUIContent.none, GUI.skin.window);
                    animationSetting.enumValueIndex = GUI.Toolbar(settingTabArea, animationSetting.enumValueIndex, animationSetting.enumNames);
                    EditorGUILayout.Space(settingTabArea.height);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);

                var animationTabArea = EditorGUILayout.BeginVertical();
                {
                    animationTabArea = new Rect(animationTabArea) { xMin = 18, height = 24 };
                    GUI.Box(animationTabArea, GUIContent.none, GUI.skin.window);
                    animationSelectionValue = GUI.Toolbar(animationTabArea, animationSelectionValue, animationTabArray);
                    EditorGUILayout.Space(animationTabArea.height);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
                switch (animationSelectionValue)
                {
                    case 0:
                        DrawAnimationSetting<MoveShowAnimation, MoveHideAnimation>("moveAnimation");
                        break;
                    case 1:
                        DrawAnimationSetting<RotateShowAnimation, RotateHideAnimation>("rotateAnimation");
                        break;
                    case 2:
                        DrawAnimationSetting<ScaleShowAnimation, ScaleHideAnimation>("scaleAnimation");
                        break;
                    case 3:
                        DrawAnimationSetting<FadeShowAnimation, FadeHideAnimation>("fadeAnimation");
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            if (isRecycle != null)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Is Recycle"));
                    var select = GUILayout.Toolbar(isRecycle.boolValue ? 0 : 1, toggleArray);
                    isRecycle.boolValue = select == 0;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (enableBackdropButton != null)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Enable Backdrop Button"));
                    var select = GUILayout.Toolbar(enableBackdropButton.boolValue ? 0 : 1, toggleArray);
                    enableBackdropButton.boolValue = select == 0;
                }
                EditorGUILayout.EndHorizontal();
            }

            DrawPropertiesExcluding(serializedObject, PropertyToExclude());

            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion

        #region Private Methods

        void DrawAnimationSetting<TShow, THide>(string propertyRelative) where TShow : class where THide : class
        {
            var showArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(showArea, GUIContent.none);
                DrawReferenceField<TShow>(ShowAnimation?.FindPropertyRelative(propertyRelative), "Show Animation");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(9);
            var hideArea = EditorGUILayout.BeginVertical();
            {
                GUI.Box(hideArea, GUIContent.none);
                DrawReferenceField<THide>(HideAnimation?.FindPropertyRelative(propertyRelative), "Hide Animation");
            }
            EditorGUILayout.EndVertical();
        }

        void DrawReferenceField<T>(SerializedProperty target, string title = null) where T : class
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);

                if (target != null)
                {
                    EditorGUILayout.Space(6);
                    EditorGUI.indentLevel++;
                    {
                        target.isExpanded = true;
                        EditorGUILayout.PropertyField(target, GUIContent.none, true);
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.Space(27);
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.LabelField(new GUIContent("Failed to find parent container"), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(9);
                }

                if (title != null) DrawTitleField(title, new Rect(area) { yMin = area.yMin, height = 24 });

                // 드랍다운 처리
                if (target != null)
                {
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
            }
            EditorGUILayout.EndVertical();
        }

        static void DrawTitleField(string title, Rect rect = default)
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
    }
}
#endif