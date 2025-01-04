using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Narazaka.MakeUPMPackage
{
    public class MakeUPMPackage : EditorWindow
    {
        const string DefaultIdKey = "Narazaka_MakeUPMPackage_DefaultId";
        const string DefaultRepoUserKey = "Narazaka_MakeUPMPackage_DefaultRepoUser";
        const string DefaultTemplateRepoKey = "Narazaka_MakeUPMPackage_DefaultTemplateRepo";
        [MenuItem("Tools/[UPM] Make UPM Package")]
        public static void ShowWindow()
        {
            GetWindow<MakeUPMPackage>("Make UPM Package");
        }

        string Id;
        string Name = "";
        string RepoName = "";
        string RepoUser;
        string TemplateRepo;
        bool IsPublic = true;
        bool Foldout;
        string DefaultId;
        string DefaultRepoUser;
        string DefaultTemplateRepo;

        private void OnEnable()
        {
            Id = DefaultId = EditorPrefs.GetString(DefaultIdKey, "net.narazaka.vrchat.");
            RepoUser = DefaultRepoUser = EditorPrefs.GetString(DefaultRepoUserKey, "Narazaka");
            TemplateRepo = DefaultTemplateRepo = EditorPrefs.GetString(DefaultTemplateRepoKey, "Narazaka/vpm-package-template");
        }

        private void OnGUI()
        {
            Id = EditorGUILayout.TextField("Id", Id);
            Name = EditorGUILayout.TextField("Name", Name);
            RepoName = EditorGUILayout.TextField("RepoName", RepoName);
            RepoUser = EditorGUILayout.TextField("RepoUser", RepoUser);
            TemplateRepo = EditorGUILayout.TextField("TemplateRepo", TemplateRepo);
            IsPublic = EditorGUILayout.Toggle("IsPublic", IsPublic);
            if (GUILayout.Button("Make UPM Package"))
            {
                if (!Validate()) return;
                try
                {
                    MakeUPM(Id, Name, RepoUser, RepoName, TemplateRepo, IsPublic);
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog("Error", e.ToString(), "OK");
                }
            }
            EditorGUILayout.LabelField($"will make git@github.com:{RepoUser}/{RepoName}");
            Foldout = EditorGUILayout.Foldout(Foldout, "Default Settings");
            if (Foldout)
            {
                EditorGUI.BeginChangeCheck();
                DefaultId = EditorGUILayout.TextField("Id", DefaultId);
                DefaultRepoUser = EditorGUILayout.TextField("RepoUser", DefaultRepoUser);
                DefaultTemplateRepo = EditorGUILayout.TextField("TemplateRepo", DefaultTemplateRepo);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(DefaultIdKey, DefaultId);
                    EditorPrefs.SetString(DefaultRepoUserKey, DefaultRepoUser);
                    EditorPrefs.SetString(DefaultTemplateRepoKey, DefaultTemplateRepo);
                }
            }
        }

        bool Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(Id))
            {
                errors.Add("Id must not be empty");
            }
            if (Id.EndsWith("."))
            {
                errors.Add("Id must not end with '.'");
            }
            if (string.IsNullOrEmpty(Name))
            {
                errors.Add("Name must not be empty");
            }
            if (string.IsNullOrEmpty(RepoName))
            {
                errors.Add("RepoName must not be empty");
            }
            if (string.IsNullOrEmpty(RepoUser))
            {
                errors.Add("RepoUser must not be empty");
            }
            if (string.IsNullOrEmpty(TemplateRepo))
            {
                errors.Add("TemplateRepo must not be empty");
            }
            if (errors.Count > 0)
            {
                EditorUtility.DisplayDialog("Error", string.Join("\n", errors), "OK");
                return false;
            }
            return true;
        }

        private static void MakeUPM(string id, string name, string repoUser, string repoName, string templateRepo, bool isPublic)
        {
            ProcessUtil.PrintResult(ProcessUtil.Call(new[] { "gh", "repo", "create", $"{repoUser}/{repoName}", "--template", templateRepo, (isPublic ? "--public" : "--private") }));
            ProcessUtil.PrintResult(ProcessUtil.Call(new[] { "git", "clone", $"git@github.com:{repoUser}/{repoName}.git", $"Packages/{id}" }));
            ReplaceFile($"Packages/{id}/package.json", id, name, repoName);
            ReplaceFile($"Packages/{id}/README.md", id, name, repoName);
            AssetDatabase.ImportAsset($"Packages/{id}/package.json");
            AssetDatabase.ImportAsset($"Packages/{id}/README.md");
            AssetDatabase.ImportAsset($"Packages/{id}/LICENSE.txt");
            ProcessUtil.PrintResult(ProcessUtil.Call(cwd: $"Packages/{id}", args: new[] { "git", "add", "." }));
            ProcessUtil.PrintResult(ProcessUtil.Call(cwd: $"Packages/{id}", args: new[] { "git", "commit", "--amend", "-m", "init" }));
            ProcessUtil.PrintResult(ProcessUtil.Call(cwd: $"Packages/{id}", args: new[] { "git", "push", "-f" }));
            AssetDatabase.Refresh();
        }

        static void ReplaceFile(string path, string id, string name, string repoName)
        {
            var text = File.ReadAllText(path);
            text = text.Replace("{{id}}", id).Replace("{{name}}", name).Replace("{{repoName}}", repoName);
            File.WriteAllText(path, text);
        }
    }
}
