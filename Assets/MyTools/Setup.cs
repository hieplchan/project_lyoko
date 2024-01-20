using System.IO;
using UnityEditor;
using Application = UnityEngine.Application;

namespace StartledSeal
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("Project", "Animations", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");  
            UnityEditor.AssetDatabase.Refresh();
        }
        
        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullPath = Path.Combine(Application.dataPath, root);
                foreach (var folder  in folders)
                {
                    var path = Path.Combine(fullPath, folder);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
        }    
    }
}