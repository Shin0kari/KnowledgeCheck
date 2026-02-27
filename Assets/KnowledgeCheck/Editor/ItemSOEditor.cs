using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemSO), true)]
public class ItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemSO itemSO = (ItemSO)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"));

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemType"));


        if (itemSO is EquippableItemSO equippableItem)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxStackQuantity"));
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Equippable Properties", EditorStyles.boldLabel);


            if (itemSO is ContainerItemSO containerItem)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_equipSlot"));
                GUI.enabled = true;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Container Properties", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("containerItems"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_equipSlot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxDurability"));
            }
        }
        else if (itemSO is ConsumableItemSO consumableItem)
        {
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxStackQuantity"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Consumable Properties", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_quantity"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemStats"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_itemAffects"));

        serializedObject.ApplyModifiedProperties();
    }
}