using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace MRTKExtensions.ServiceExtensions.Editor
{
    [CustomEditor(typeof(ServiceDisplayHook))]
    [ExecuteAlways]
    public class ServiceDisplayEditor : UnityEditor.Editor
    {
        private readonly Color proHeaderColor = new Color32(56, 56, 56, 255);
        private readonly Color defaultHeaderColor = new Color32(194, 194, 194, 255);

#if UNITY_2019_1_OR_NEWER
        private const int headerYOffet = -6;
        private const int headerXOffset = 44;
#else
        private const int headerYOffet = 0;
        private const int headerXOffset = 48;
#endif
        /// <summary>
        /// Used as postfix for keys
        /// </summary>
        private int keyCounter;

        /// <summary>
        /// Key prefix (set to class name)
        /// </summary>
        private readonly string preferenceKeyPrefix;

        /// <summary>
        /// The service found
        /// </summary>
        private object service;

        /// <summary>
        /// The name of the service in the hook
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Draw the GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (service == null)
            {
                var hook = target as ServiceDisplayHook;
                if (hook != null)
                {
                    serviceName = hook.gameObject.name;
                    if (MixedRealityToolkit.IsInitialized)
                    {
                        service = MixedRealityServiceRegistry.GetAllServices()
                            .FirstOrDefault(p => p.Name == serviceName);
                    }
                }
            }

            if (service != null)
            {
                DrawInspectorGUI(service, service.GetType().FullName);
            }
            else
            {
                DrawHeader($"No service with name {serviceName} found");
            }
        }

        /// <summary>
        /// Draw the inspector UI for the current service
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="header"></param>
        protected void DrawInspectorGUI(object targetObject, string header)
        {
            keyCounter = 0;
            DrawHeader(header);
            RenderObjectFields(targetObject);
        }

        /// <summary>
        /// Draw the header
        /// </summary>
        /// <param name="header"></param>
        private void DrawHeader(string header)
        {
            // Draw a rect over the top of the existing header label
            var labelRect = EditorGUILayout.GetControlRect(false, 0f);
            labelRect.height = EditorGUIUtility.singleLineHeight;
            labelRect.y -= labelRect.height - headerYOffet;
            labelRect.x = headerXOffset;
            labelRect.xMax -= labelRect.x * 2f;

            EditorGUI.DrawRect(labelRect, EditorGUIUtility.isProSkin ? proHeaderColor : defaultHeaderColor);
            EditorGUI.LabelField(labelRect, header, EditorStyles.boldLabel);
        }

        /// <summary>
        /// Render all properties as editor fields, create foldout if it's a complex object
        /// </summary>
        /// <param name="targetObject"></param>
        protected void RenderObjectFields(object targetObject)
        {
            if (targetObject == null)
            {
                return;
            }
            foreach (var prop in targetObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Render everything except the profile that's been taken care of already
                if (prop.Name == nameof(BaseExtensionService.ConfigurationProfile))
                {
                    continue;
                }
                var propVal = prop.GetValue(targetObject);

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
        protected void DrawField(string name, object propVal)
        {
            // Check if there's a custom field drawer first
            if (DrawCustomField(name, propVal))
            {
                return;
            }

            if (DrawValueType(name, propVal))
            {
                return;
            }

            if (DrawCollection(name, propVal))
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
                    EditorGUILayout.TextField(name, propVal?.ToString(),
                        Array.Empty<GUILayoutOption>());
                    break;
            }
        }

        private bool DrawCollection(string name, object propVal)
        {
            if (propVal is ICollection collection)
            {
                RenderFoldout(name, () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        var objCount = 0;
                        foreach (var obj in collection)
                        {
                            keyCounter++;
                            RenderFoldout($"{obj.GetType().Name}[{objCount++}]", () =>
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    RenderObjectFields(obj);
                                }
                            }, keyCounter.ToString());
                        }
                    }
                }, (++keyCounter).ToString());


                return true;
            }
            return false;
        }


        /// <summary>
        /// If this object has a Value property (*cough*React*cough*) draw it's value property)
        /// </summary>
        /// <param name="name">property name</param>
        /// <param name="propVal">property value</param>
        private bool DrawValueType(string name, object propVal)
        {
            if (propVal != null)
            {
                var propertyToFind = propVal.GetType().GetProperties().
                    FirstOrDefault(p => p.Name == "Value");
                if (propertyToFind != null)
                {
                    DrawField(name, propertyToFind.GetValue(propVal));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Allow for custom framework-specific fields
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propVal"></param>
        /// <returns></returns>
        protected virtual bool DrawCustomField(string name, object propVal)
        {
            return false;
        }

        /// <summary>
        /// Render Bold/HelpBox style Foldout. Graciously borrowed adapted from BaseMixedRealityProfileInspector - and very much
        /// simplified
        /// </summary>
        /// <param name="title">Title in foldout</param>
        /// <param name="renderContent">code to execute to render inside of foldout</param>
        /// <param name="preferenceKeyPostFix">current show/hide state will be tracked associated with provided preference key</param>
        private void RenderFoldout(string title, Action renderContent, string preferenceKeyPostFix)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var preferenceKey = $"{preferenceKeyPrefix}_{preferenceKeyPostFix}";
            var storedState = SessionState.GetBool(preferenceKey, false);
            var currentState = EditorGUILayout.Foldout(storedState, title, true, MixedRealityStylesUtility.BoldFoldoutStyle);

            if (currentState != storedState)
            {
                SessionState.SetBool(preferenceKey, currentState);
            }

            if (currentState)
            {
                renderContent();
            }

            EditorGUILayout.EndVertical();
        }

        private bool doRepaint;
        void OnEnable()
        {
            doRepaint = true;
            RepaintLoop();
        }

        /// <summary>
        /// Forced repaint every 100ms to give instant effect
        /// </summary>
        /// <returns></returns>
        private async Task RepaintLoop()
        {
            while (doRepaint)
            {
                await Task.Delay(100);
                Repaint();
            }
        }

        private void OnDisable()
        {
            doRepaint = false;
        }
    }
}
