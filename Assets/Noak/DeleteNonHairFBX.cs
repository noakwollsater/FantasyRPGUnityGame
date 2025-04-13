//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Linq;

//public class DeleteNonHairFBX : EditorWindow
//{
//    string targetFolder = "Assets/Resources/Meshes/Outfits/ApocalypseOutlaws";
//    string[] keepKeywords = new string[] { "SK_VIKG_WARR_05_10TORS", "SK_PIRT_CAPT_09_10TORS_HU01", "SK_PIRT_CAPT_10_10TORS_HU01"
//    , "SK_APOC_OUTL_04_10TORS_HU01", "SK_GOBL_FIGT_06_10TORS_GO01", "SK_PIRT_CAPT_10_11AUPL_HU01",
//    "SK_PIRT_CAPT_10_12AUPR_HU01", "SK_PIRT_CAPT_10_13ALWL_HU01", "SK_PIRT_CAPT_10_14ALWR_HU01",
//    "SK_PIRT_CAPT_08_15HNDL_HU01", "SK_PIRT_CAPT_08_16HNDR_HU01", "SK_APOC_OUTL_10_13ALWL_HU01",
//    "SK_APOC_OUTL_10_14ALWR_HU01", "SK_PIRT_CAPT_10_15HNDL_HU01", "SK_PIRT_CAPT_10_16HNDR_HU01",
//    "SK_PIRT_CAPT_09_17HIPS_HU01", "SK_APOC_OUTL_07_17HIPS_HU01", "SK_PIRT_CAPT_06_18LEGL_HU01",
//    "SK_PIRT_CAPT_06_19LEGR_HU01", "SK_APOC_OUTL_08_18LEGL_HU01", "SK_APOC_OUTL_08_19LEGR_HU01",
//    "SK_APOC_OUTL_06_20FOTL_HU01", "SK_APOC_OUTL_06_21FOTR_HU01", "NOSE", "TETH", "TONG",
//    "HAIR", "EBRL", "EBRR", "FCHR", "EARL", "EARR"};

//    [MenuItem("Tools/Delete Non-Hair FBX Files")]
//    public static void ShowWindow()
//    {
//        GetWindow<DeleteNonHairFBX>("Delete Non-Hair FBX");
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("Delete Non-Hair FBX Files", EditorStyles.boldLabel);

//        targetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);

//        if (GUILayout.Button("Delete Now"))
//        {
//            if (EditorUtility.DisplayDialog("Confirm Deletion",
//                "Are you sure you want to delete all FBX files that are NOT hair-related in:\n" + targetFolder,
//                "Yes, Delete", "Cancel"))
//            {
//                DeleteFBXFiles();
//            }
//        }
//    }

//    void DeleteFBXFiles()
//    {
//        string[] allFBX = Directory.GetFiles(targetFolder, "*.fbx", SearchOption.TopDirectoryOnly);
//        int deletedCount = 0;

//        foreach (string path in allFBX)
//        {
//            string fileName = Path.GetFileName(path).ToLower();
//            bool keep = keepKeywords.Any(k => fileName.Contains(k.ToLower())); // <== FIXED LINE

//            if (!keep)
//            {
//                if (AssetDatabase.DeleteAsset(path))
//                {
//                    Debug.Log("Deleted: " + fileName);
//                    deletedCount++;
//                }
//                else
//                {
//                    Debug.LogWarning("Failed to delete: " + fileName);
//                }
//            }
//            else
//            {
//                Debug.Log("Kept: " + fileName);
//            }
//        }

//        AssetDatabase.Refresh();
//        Debug.Log($"Finished. Deleted {deletedCount} files.");
//    }

//}
