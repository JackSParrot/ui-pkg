﻿using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEngine;
using JackSParrot.UI;

[CustomEditor(typeof(ScrollList), true)]
[CanEditMultipleObjects]
public class ScrollListEditor : Editor
{
    SerializedProperty m_Content;
    SerializedProperty m_MovementType;
    SerializedProperty m_Elasticity;
    SerializedProperty m_Inertia;
    SerializedProperty m_DecelerationRate;
    SerializedProperty m_ScrollSensitivity;
    SerializedProperty m_Viewport;
    SerializedProperty m_OnValueChanged;
    SerializedProperty _protoTypeCell;
    SerializedProperty _direction;
    SerializedProperty _type;

    AnimBool m_ShowElasticity;
    AnimBool m_ShowDecelerationRate;

    ScrollList _script;
    protected virtual void OnEnable()
    {
        _script = (ScrollList)target;

        m_Content = serializedObject.FindProperty("m_Content");
        m_MovementType = serializedObject.FindProperty("m_MovementType");
        m_Elasticity = serializedObject.FindProperty("m_Elasticity");
        m_Inertia = serializedObject.FindProperty("m_Inertia");
        m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
        m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
        m_Viewport = serializedObject.FindProperty("m_Viewport");
        m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

        _protoTypeCell = serializedObject.FindProperty("PrototypeCell");
        _direction = serializedObject.FindProperty("Direction");
        _type = serializedObject.FindProperty("IsGrid");

        m_ShowElasticity = new AnimBool(Repaint);
        m_ShowDecelerationRate = new AnimBool(Repaint);
        SetAnimBools(true);
    }

    protected virtual void OnDisable()
    {
        m_ShowElasticity.valueChanged.RemoveListener(Repaint);
        m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
    }

    void SetAnimBools(bool instant)
    {
        SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
        SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue, instant);
    }

    void SetAnimBool(AnimBool a, bool value, bool instant)
    {
        if (instant)
            a.value = value;
        else
            a.target = value;
    }

    public override void OnInspectorGUI()
    {
        SetAnimBools(false); 
        serializedObject.Update();
          
        EditorGUILayout.PropertyField(_direction);
        EditorGUILayout.PropertyField(_type, new GUIContent("Grid"));
        if (_type.boolValue)
        {
            string title = _direction.enumValueIndex == (int)ScrollList.EDirectionType.Vertical ? "Coloumns" : "Rows";
            _script.Segments =  EditorGUILayout.IntField(title, _script.Segments);
        }

        EditorGUILayout.PropertyField(m_Viewport);
        EditorGUILayout.PropertyField(m_Content);
        EditorGUILayout.PropertyField(_protoTypeCell);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_MovementType);
        if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_Elasticity);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.PropertyField(m_Inertia);
        if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_DecelerationRate);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.PropertyField(m_ScrollSensitivity);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_OnValueChanged);

        serializedObject.ApplyModifiedProperties();
    }
}