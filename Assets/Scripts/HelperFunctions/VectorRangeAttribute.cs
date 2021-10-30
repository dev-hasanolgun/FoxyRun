using UnityEngine;
using UnityEditor;

public class VectorRangeAttribute : PropertyAttribute 
{
    public readonly int fMinX, fMaxX, fMinY, fMaxY;
    public VectorRangeAttribute(int fMinX, int fMaxX, int fMinY, int fMaxY)
    {
        this.fMinX = fMinX;
        this.fMaxX = fMaxX;
        this.fMinY = fMinY;
        this.fMaxY = fMaxY;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VectorRangeAttribute))]
public class VectorRangeAttributeDrawer : PropertyDrawer
{
    VectorRangeAttribute rangeAttribute => (VectorRangeAttribute)attribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect textFieldPosition = position;
        textFieldPosition.width = position.width;
        textFieldPosition.height = position.height;
        EditorGUI.BeginChangeCheck();
        Vector2Int val = EditorGUILayout.Vector2IntField(label, property.vector2IntValue);
        if (EditorGUI.EndChangeCheck())
        {
            val.x = Mathf.Clamp(val.x, rangeAttribute.fMinX, rangeAttribute.fMaxX);
            val.y = Mathf.Clamp(val.y, rangeAttribute.fMinY, rangeAttribute.fMaxY);
            property.vector2IntValue = val;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsValid(property))
        {
            return 32;
        }
        return base.GetPropertyHeight(property, label);
    }
    bool IsValid(SerializedProperty prop)
    {
        Vector2Int vector = prop.vector2IntValue;
        return vector.x >= rangeAttribute.fMinX && vector.x <= rangeAttribute.fMaxX && vector.y >= rangeAttribute.fMinY && vector.y <= rangeAttribute.fMaxY;
    }
}
#endif