using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(VisualSettings))]

public class VisualSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var visualSettingInstance = target as VisualSettings;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("QualitySetting"));
    
        EditorGUILayout.HelpBox("Feel free to add more settings in the VisualSettings.cs file! Look there for more info.", MessageType.Info);


        if (visualSettingInstance.toggle != null)
        {
            EditorGUILayout.LabelField("Choose the value when checked:");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enabledToggleValue"));
        }
        serializedObject.ApplyModifiedProperties(); //Applies the modifications to the variables themselves
    }
}
#endif
public class VisualSettings : MonoBehaviour
{

    public Slider slider;
    public Toggle toggle;
    public enum VisualSetting { AntiAliasing, pixelLightCount, VSync }; //Add here to allow for greater visual settings in the editor.  Reference Unity's QualitySettings for possible choices. Handles properties of type int and bool.
    public VisualSetting QualitySetting;


    public int enabledToggleValue = 1; //Can be overridden in Inspector

    private void Start()
    {
        slider = transform.GetComponentInParent<Slider>();
        toggle = transform.GetComponentInParent<Toggle>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(delegate { SliderChange(QualitySetting, slider.value); });
        } else if (toggle != null)
        {
            toggle.onValueChanged.AddListener(delegate { ToggleChange(QualitySetting); });
        }
    }
    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.Label(this.transform.position, QualitySetting.ToString(), GUI.skin.button);
#endif
    }
    public void SliderChange(VisualSetting setting, float floatValue) //Used when using a UI slider
    {
        int value = (int)floatValue;
        if (value < 0)
        {
            throw new System.ArgumentOutOfRangeException();
        }
        foreach (var property in typeof(QualitySettings).GetTypeInfo().GetProperties())
        {
            if (property.ToString().ToLower().Contains(setting.ToString().ToLower()))
            {
                property.SetValue(null, value);
                return;
            }
        }
    }
    public void ToggleChange(VisualSetting setting) //Used when using a UI toggle.
    {
        foreach (var property in typeof(QualitySettings).GetTypeInfo().GetProperties())
        {
            if (property.ToString().ToLower().Contains(setting.ToString().ToLower()))
            {
                if (property.PropertyType.Name == "Boolean")
                {
                    property.SetValue(null, toggle.isOn);
                } else if (toggle.isOn) //Handles numerical values
                {
                    property.SetValue(null, enabledToggleValue);
                } else
                {
                    property.SetValue(null, 0);
                }
                return;
            }
        }
    }
}