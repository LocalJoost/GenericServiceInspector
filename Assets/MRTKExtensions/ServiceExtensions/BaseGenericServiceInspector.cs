using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;

namespace MRTKExtensions.ServiceExtensions.Editor
{
    /// <summary>
    /// A class that draws a generic inspector for an extension service
    /// </summary>
    public class BaseGenericServiceInspector : BaseMixedRealityServiceInspector
    {
        /// <summary>
        /// Preference keys showing open/close of foldout
        /// </summary>
        private readonly Dictionary<string, bool> preferenceKeys = new Dictionary<string, bool>();

        /// <summary>
        /// Used as postfix for keys
        /// </summary>
        private int keyCounter = 0;

        /// <summary>
        /// Key prefix (set to class name)
        /// </summary>
        private readonly string preferenceKeyPrefix;

        public BaseGenericServiceInspector()
        {
            preferenceKeyPrefix = GetType().FullName;
        }

        /// <summary>
        /// Draw the inspector UI for the current serivce
        /// </summary>
        /// <param name="target"></param>
        public override void DrawInspectorGUI(object target)
        {
            keyCounter = 0;
            GUI.enabled = false;
            RenderObjectFields(target);
        }

        /// <summary>
        /// Get a preference key value - and create if not present
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool GetPreferenceKeyValue(string id)
        {
            if (!preferenceKeys.ContainsKey(id))
            {
                preferenceKeys.Add(id, false);
            }

            return preferenceKeys[id];
        }

        /// <summary>
        /// Render all properties as editor fields, create foldout is complex object
        /// </summary>
        /// <param name="target"></param>
        private void RenderObjectFields(object target)
        {
            foreach (var prop in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Render everything except the profile that's been taken care of already
                if (prop.Name == nameof(BaseExtensionService.ConfigurationProfile))
                {
                    continue;
                }
                var propVal = prop.GetValue(target);

                // When property value equals the full name, we have a complex type so render its properties recursively
                // This does not work for types with ToString overridden, so the InspectorExpandAttribute can be used to force that
                if (propVal?.ToString() == propVal?.GetType().FullName ||
                    prop.PropertyType.GetCustomAttribute(typeof(InspectorExpandAttribute)) != null)
                {
                    keyCounter++;
                    RenderFoldout(prop.Name, () =>
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            RenderObjectFields(propVal);
                        }
                    }, keyCounter.ToString());
                }
                else
                {
                    DrawField(prop.Name, propVal);
                }
            }
        }

        /// <summary>
        /// Draw various field types
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="propVal">property value</param>
        private void DrawField(string name, object propVal)
        {
            // Check if there's a custom field drawer first
            if (DrawCustomField(name, propVal))
            {
                return;
            }

            switch (propVal)
            {
                case bool boolVal:
                    EditorGUILayout.Toggle(name, boolVal,
                        Array.Empty<GUILayoutOption>());
                    break;
                case Vector2 v2Val:
                    EditorGUILayout.Vector2Field(name, v2Val,
                        Array.Empty<GUILayoutOption>());
                    break;
                case Vector3 v3Val:
                    EditorGUILayout.Vector3Field(name, v3Val,
                        Array.Empty<GUILayoutOption>());
                    break;
                case Color colorVal:
                    EditorGUILayout.ColorField(name, colorVal,
                        Array.Empty<GUILayoutOption>());
                    break;
                default:
                    EditorGUILayout.TextField(name, propVal.ToString(),
                        Array.Empty<GUILayoutOption>());
                    break;
            }
        }

        /// <summary>
        /// Allow for custom framework-specific fields
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propVal"></param>
        /// <returns></returns>
        public virtual bool DrawCustomField(string name, object propVal)
        {
            return false;
        }


        /// <summary>
        /// Render Bold/HelpBox style Foldout. Graciously borrowed and adapted from BaseMixedRealityProfileInspector 
        /// </summary>
        /// <param name="title">Title in foldout</param>
        /// <param name="renderContent">code to execute to render inside of foldout</param>
        /// <param name="preferenceKeyPostFix">current show/hide state will be tracked associated with provided preference key</param>
        protected void RenderFoldout(string title, Action renderContent, string preferenceKeyPostFix)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var preferenceKey = $"{preferenceKeyPrefix}_{preferenceKeyPostFix}";
            var isValidPreferenceKey = !string.IsNullOrEmpty(preferenceKey);
            var state = GetPreferenceKeyValue(preferenceKey);
            if (isValidPreferenceKey)
            {
                state = SessionState.GetBool(preferenceKey, false);
                preferenceKeys[preferenceKey] = EditorGUILayout.Foldout(state, title, true, MixedRealityStylesUtility.BoldFoldoutStyle);
            }

            var currentState = GetPreferenceKeyValue(preferenceKey);

            if (isValidPreferenceKey && currentState != state)
            {
                SessionState.SetBool(preferenceKey, currentState);
            }

            if (currentState)
            {
                renderContent();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
