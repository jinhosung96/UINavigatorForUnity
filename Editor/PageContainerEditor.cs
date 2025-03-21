#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System.Linq;
using JHS.Library.UINavigator.Runtime;
using JHS.Library.UINavigator.Runtime.Page;
using UnityEditor;
using UnityEngine;

namespace JHS.Library.UINavigator.Editor
{
    [CustomEditor(typeof(PageContainer))]
    public class PageContainerEditor : UIContainerEditor
    {
        #region Fields

        SerializedProperty instantiateType;
        SerializedProperty registerPagesByPrefab;
        SerializedProperty registerPagesByAddressable;
        SerializedProperty hasDefault;
        readonly string[] toggleArray = { "On", "Off" };

        #endregion

        #region Properties

        PageContainer Target => target as PageContainer;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(PageContainer.InstantiateType)}>k__BackingField", 
            $"<{nameof(PageContainer.RegisterPagesByPrefab)}>k__BackingField", 
#if ADDRESSABLE_SUPPORT
            $"<{nameof(PageContainer.RegisterPagesByAddressable)}>k__BackingField",
#endif
            $"<{nameof(PageContainer.HasDefault)}>k__BackingField"
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            instantiateType = serializedObject.FindProperty($"<{nameof(PageContainer.InstantiateType)}>k__BackingField");
            registerPagesByPrefab = serializedObject.FindProperty($"<{nameof(PageContainer.RegisterPagesByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            registerPagesByAddressable = serializedObject.FindProperty($"<{nameof(PageContainer.RegisterPagesByAddressable)}>k__BackingField");
#endif
            hasDefault = serializedObject.FindProperty($"<{nameof(PageContainer.HasDefault)}>k__BackingField");
        }

        #endregion

        #region GUI Process

        protected override void AdditionalGUIProcess()
        {
            var area = EditorGUILayout.BeginVertical();
            {
                GUI.Box(area, GUIContent.none);
                DrawTitleField("Initialize Setting");
                EditorGUI.indentLevel++;
                {
                    EditorGUILayout.PropertyField(instantiateType, GUIContent.none);
                    switch (Target.InstantiateType)
                    {
                        case InstantiateType.InstantiateByPrefab:
                            EditorGUILayout.PropertyField(registerPagesByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case InstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(registerPagesByAddressable);
                            break;
#endif
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("Has Default"));
                        var select = GUILayout.Toolbar(hasDefault.boolValue ? 0 : 1, toggleArray);
                        hasDefault.boolValue = select == 0;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(9);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}
#endif