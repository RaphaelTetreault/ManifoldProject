using Manifold;
using Manifold.IO;
using System;
using UnityEngine;
using UnityEditor;

namespace Manifold.EditorTools.GC.GFZ
{
    public class GfzProjectWindow : EditorWindow
    {
        private static GfzProjectWindow self;

        // Handy window serialization
        // https://answers.unity.com/questions/119978/how-do-i-have-an-editorwindow-save-its-data-inbetw.html
        public const string saveName = "gfz-project-window";

        // Serialize values so they persist
        [SerializeField] int tabIndex;
        [SerializeField] GfzProjectSettings settings = new GfzProjectSettings();


        private void OnEnable()
        {
            // Load in data
            settings = GfzProjectSettings.Load(saveName);
            //
            self = this;
        }

        private void OnDisable()
        {
            // Save out data
            GfzProjectSettings.Save(saveName, settings);
            //
            self = null;
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem(GfzMenuItems.ProjectWindow.OpenNewWindow, priority = GfzMenuItems.ProjectWindow.Priority.OpenNewWindow)]
        static void OpenNewWindow()
        {
            // Get existing open window or if none, make a new one:
            GfzProjectWindow window = (GfzProjectWindow)GetWindow(typeof(GfzProjectWindow));
            window.titleContent = new GUIContent("GFZ Project Settings");
            window.Show();
        }

        void OnGUI()
        {
            // The current window instance in serializable format
            var window = new SerializedObject(this);
            // The window tab we want to look at
            tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "Settings", "Graphics", "Testing" });
            EditorGUILayout.Space();

            switch (tabIndex)
            {
                case 0:
                    {
                        settings.DrawSettingsTab();
                    }
                    break;

                case 1:
                    {
                        settings.DrawColors();
                    }
                    break;

                case 2:
                    {
                        settings.DrawTestTab();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            // Save changes
            window.ApplyModifiedProperties();
        }


        /// <summary>
        /// Returns setting data stored in the GFZ Project Window panel.
        /// </summary>
        /// <returns></returns>
        public static GfzProjectSettings GetSettings()
        {
            if (self != null)
            {
                //Debug.Log("Got instance data");
                return self.settings;
            }
            else
            {
                //Debug.Log("Got data from disk");
                return GfzProjectSettings.Load(saveName);
            }
        }

    }
}