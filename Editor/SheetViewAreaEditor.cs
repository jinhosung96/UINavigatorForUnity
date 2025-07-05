#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System.Linq;
using JHS.Library.UINavigator.Runtime;
using JHS.Library.UINavigator.Runtime.Sheet;
using UnityEditor;
using UnityEngine;

namespace JHS.Library.UINavigator.Editor
{
    [CustomEditor(typeof(SheetViewArea))]
    public class SheetViewAreaEditor : UIViewAreaEditor
    {
        #region Fields

        SerializedProperty instantiateType;
        SerializedProperty registerSheetsByPrefab;
        SerializedProperty registerSheetsByAddressable;
        SerializedProperty hasDefault;
        readonly string[] toggleArray = { "On", "Off" };

        #endregion

        #region Properties
        
        SheetViewArea Target => target as SheetViewArea;
        protected override string[] PropertyToExclude() => base.PropertyToExclude().Concat(new[]
        {
            $"<{nameof(SheetViewArea.InstantiateType)}>k__BackingField", 
            $"<{nameof(SheetViewArea.RegisterSheetsByPrefab)}>k__BackingField", 
#if ADDRESSABLE_SUPPORT
            $"<{nameof(SheetViewArea.RegisterSheetsByAddressable)}>k__BackingField",
#endif
            $"<{nameof(SheetViewArea.HasDefault)}>k__BackingField"
        }).ToArray();

        #endregion

        #region Unity Lifecycle

        protected override void OnEnable()
        {
            base.OnEnable();
            instantiateType = serializedObject.FindProperty($"<{nameof(SheetViewArea.InstantiateType)}>k__BackingField");
            registerSheetsByPrefab = serializedObject.FindProperty($"<{nameof(SheetViewArea.RegisterSheetsByPrefab)}>k__BackingField");
#if ADDRESSABLE_SUPPORT
            registerSheetsByAddressable = serializedObject.FindProperty($"<{nameof(SheetViewArea.RegisterSheetsByAddressable)}>k__BackingField");
#endif
            hasDefault = serializedObject.FindProperty($"<{nameof(SheetViewArea.HasDefault)}>k__BackingField");
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
                            EditorGUILayout.PropertyField(registerSheetsByPrefab);
                            break;
#if ADDRESSABLE_SUPPORT
                        case InstantiateType.InstantiateByAddressable:
                            EditorGUILayout.PropertyField(registerSheetsByAddressable);
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