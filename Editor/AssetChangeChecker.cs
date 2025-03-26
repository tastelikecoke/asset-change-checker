using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace AssetChangeChecker
{
    public class AssetChangeChecker : EditorWindow
    {
        public bool changed = false;
        public FileSystemWatcher watcher;

        private DateTime lastRefresh;

        [MenuItem("Tools/Asset Change Checker")]
        public static void ShowChecker()
        {
            AssetChangeChecker assetChangeChecker = GetWindow<AssetChangeChecker>();
            assetChangeChecker.titleContent = new GUIContent("Asset Change Checker");
        }
        public void CreateGUI()
        {
            changed = false;

            VisualElement root = rootVisualElement;
            Button button = new Button();
            button.name = "button";
            button.text = $"Refresh now";

            button.clicked += () =>
            {
                PlayerPrefs.SetString("lastsave", DateTime.Now.ToLongTimeString());
                changed = false;
                AssetDatabase.Refresh();
                UpdateButton();
            };
            root.Add(button);
        }

        public void OnEnable()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            FileSystemWatcher currentWatcher = new FileSystemWatcher(path);
            watcher = currentWatcher;
            watcher.Filter = @"*.*";
            watcher.Changed += OnChanged;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        public void OnDisable()
        {
            watcher.Changed -= OnChanged;
            watcher.Dispose();
        }


        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            changed = true;
        }

        public void Update()
        {
            if (!changed) return;

            UpdateButton();
        }

        public void UpdateButton()
        {
            string lastRefreshString = PlayerPrefs.GetString("lastsave");
            VisualElement root = rootVisualElement;
            Button button = root.hierarchy.Children().First() as Button;

            button.text = $"Refresh now";
            button.style.backgroundColor = new StyleColor(Color.clear);
            if (changed)
            {
                string dateText = "";
                if (DateTime.TryParse(lastRefreshString, out lastRefresh))
                {
                    dateText = $"({(int)(DateTime.Now - lastRefresh).TotalSeconds} seconds ago)";
                }

                button.text = $"Refresh now (File Changed){dateText}";
                button.style.backgroundColor = new StyleColor(new Color(0.4f, 0.0f, 0.0f));
            }
        }
    }
}