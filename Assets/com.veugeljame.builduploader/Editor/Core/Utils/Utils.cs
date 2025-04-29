using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wireframe
{
    internal static class Utils
    {
        public static readonly string CacheFolder = Application.persistentDataPath + "/BuildUploader/CachedBuilds";
        
        public static Texture2D WindowIcon
        {
            get
            {
                var iconPath = "Packages/com.veugeljame.builduploader/Icon.png";
                Object loadAssetAtPath = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Object));
                return loadAssetAtPath as Texture2D;
            }
        }
        
        public static Texture2D WarningIcon
        {
            get
            {
                var iconPath = "Packages/com.veugeljame.builduploader/warningicon.png";
                Object loadAssetAtPath = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Object));
                return loadAssetAtPath as Texture2D;
            }
        }
        
        public static bool IsPathADirectory(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            if (!File.Exists(sourceFile))
            {
                Debug.LogWarning($"Skipping copy. Source file does not exist: {sourceFile}");
                return;
            }

            var directory = Path.GetDirectoryName(destinationFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }


        public static async Task<bool> CopyDirectoryAsync(string sourcePath, string cacheFolderPath)
        {
            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, cacheFolderPath));
                }

                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    await CopyFileAsync(newPath, newPath.Replace(sourcePath, cacheFolderPath));
                }

                return true;
            }
            catch (IOException e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}