using UnityEditor;

[CustomEditor(typeof(ButtonWithCost))]
public class ButtonWithCostEditor : Editor
{
    private SerializedProperty _resourceCost;
    private SerializedProperty _iconImage;
    private SerializedProperty _amountLabel;
    private SerializedProperty _associatedResource;

    private void OnEnable()
    {
        _resourceCost = serializedObject.FindProperty("ResourceCost");
        _iconImage = serializedObject.FindProperty("IconImage");
        _amountLabel = serializedObject.FindProperty("AmountLabel");
        _associatedResource = serializedObject.FindProperty("AssociatedResource");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_resourceCost);
        EditorGUILayout.PropertyField(_iconImage);
        EditorGUILayout.PropertyField(_amountLabel);
        EditorGUILayout.PropertyField(_associatedResource);
        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}
