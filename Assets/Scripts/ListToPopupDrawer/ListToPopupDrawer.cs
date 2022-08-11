using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ListToPopupAttribute : PropertyAttribute
{
    public Type myType;
    public string propertyName;

    public ListToPopupAttribute(Type myType, string propertyName)
    {
        this.myType = myType;
        this.propertyName = propertyName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListToPopupAttribute))]
public class ListToPopupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var atb = attribute as ListToPopupAttribute;
        List<string> stringList = null;

        if (atb.myType.GetField(atb.propertyName) != null)
        {
            stringList = atb.myType.GetField(atb.propertyName).GetValue(atb.myType) as List<string>;
        }

        if (stringList != null && stringList.Count != 0)
        {
            var selectedIndex = Math.Max(stringList.IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList.ToArray());
            property.stringValue = stringList[selectedIndex];
        }
        else
            EditorGUI.PropertyField(position, property, label);
    }
}
#endif