using UnityEditor;

[CustomEditor(typeof(ButtonWithDoubleCost))]
public class ButtonWithDoubleCostEditor : Editor
{
    private SerializedProperty _resourceCost;
    private SerializedProperty _iconImage;
    private SerializedProperty _amountLabel;
    private SerializedProperty _associatedResource;

    private SerializedProperty _resourceCost2;
    private SerializedProperty _iconImage2;
    private SerializedProperty _amountLabel2;
    private SerializedProperty _associatedResource2;

    private void OnEnable()
    {
        _resourceCost = serializedObject.FindProperty("ResourceCost");
        _iconImage = serializedObject.FindProperty("IconImage");
        _amountLabel = serializedObject.FindProperty("AmountLabel");
        _associatedResource = serializedObject.FindProperty("AssociatedResource");
        _resourceCost2 = serializedObject.FindProperty("ResourceCost2");
        _iconImage2 = serializedObject.FindProperty("IconImage2");
        _amountLabel2 = serializedObject.FindProperty("AmountLabel2");
        _associatedResource2 = serializedObject.FindProperty("AssociatedResource2");
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        //EditorGUILayout.PropertyField(_resourceCost);
        //EditorGUILayout.PropertyField(_iconImage);
        //EditorGUILayout.PropertyField(_amountLabel);
        //EditorGUILayout.PropertyField(_associatedResource);
        //EditorGUILayout.Space(10);

        //EditorGUILayout.PropertyField(_resourceCost2);
        //EditorGUILayout.PropertyField(_iconImage2);
        //EditorGUILayout.PropertyField(_amountLabel2);
        //EditorGUILayout.PropertyField(_associatedResource2);
        //EditorGUILayout.Space(10);

        //serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}
