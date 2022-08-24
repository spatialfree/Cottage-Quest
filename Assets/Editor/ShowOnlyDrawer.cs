using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
    string valueStr;

    // if editor is not running, show default property
    if (!EditorApplication.isPlaying) {
      EditorGUI.LabelField(position, label.text, "...");
      return;
    }

    switch (prop.propertyType) {
      case SerializedPropertyType.Boolean:
        valueStr = prop.boolValue ? "true" : "false";
        break;
      case SerializedPropertyType.Integer:
        valueStr = prop.intValue.ToString();
        break;
      case SerializedPropertyType.Float:
        valueStr = prop.floatValue.ToString("0.00");
        break;
      case SerializedPropertyType.String:
        valueStr = prop.stringValue;
        break;
      default:
        valueStr = "(need more code)";
        break;
    }
    EditorGUI.LabelField(position, label.text, valueStr);
  }
}