using System.IO;
using UnityEngine;
using UnityEditor;

namespace Narazaka.MakeUPMPackage
{
    public class CloneUPMPackage : EditorWindow
    {
        const string DefaultRepoPathKey = "Narazaka_MakeUPMPackage_DefaultRepoPath";

        [MenuItem("Tools/[UPM] Clone UPM Package")]
        public static void ShowWindow()
        {
            GetWindow<CloneUPMPackage>("Clone UPM Package");
        }

        string RepoPath;
        string DefaultRepoPath;
        bool Foldout;

        private void OnEnable()
        {
            RepoPath = DefaultRepoPath = EditorPrefs.GetString(DefaultRepoPathKey, "Narazaka/");
        }

        private void OnGUI()
        {
            RepoPath = EditorGUILayout.TextField("Repo", RepoPath);
            if (GUILayout.Button("Clone UPM Package"))
            {
                try
                {
                    CloneUPM(RepoPath);
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog("Error", e.ToString(), "OK");
                }
            }
            Foldout = EditorGUILayout.Foldout(Foldout, "Default Settings");
            if (Foldout)
            {
                EditorGUI.BeginChangeCheck();
                DefaultRepoPath = EditorGUILayout.TextField("Repo", DefaultRepoPath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(DefaultRepoPathKey, DefaultRepoPath);
                }
            }
        }

        private static void CloneUPM(string repoPath)
        {
            var path = FileUtil.GetUniqueTempPathInProject();
            ProcessUtil.Call(new[] { "git", "clone", $"git@github.com:{repoPath}.git", path }).PrintResult();
            var packageJson = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageJson>(File.ReadAllText($"{path}/package.json"));
            var id = packageJson.name;
            if (Directory.Exists($"Packages/{id}"))
            {
                EditorUtility.DisplayDialog("Error", $"Package {id} already exists", "OK");
                return;
            }
            FileUtil.MoveFileOrDirectory(path, $"Packages/{id}");
            AssetDatabase.ImportAsset($"Packages/{id}", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        public class PackageJson
        {
            public string name;
        }
    }
}
