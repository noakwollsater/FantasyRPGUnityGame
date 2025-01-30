﻿#if UNITY_EDITOR
using System;
#if UNITY_2021_2_OR_NEWER
#elif UNITY_2020_3_OR_NEWER
using System.Reflection;
#endif
using UnityEditor;
using UnityEngine;

namespace Kamgam.MeshExtractor
{
    public static class SaveAssetHelper
    {
#if UNITY_2021_2_OR_NEWER
#elif UNITY_2020_3_OR_NEWER
        static bool _saveIfDirtyMethodRetrieved;
        static MethodInfo _saveIfDirtyObjMethod;
        static MethodInfo _saveIfDirtyGuidMethod;

        static bool cacheReflectionsAndReturnResult(bool useGUID)
        {
            if (!_saveIfDirtyMethodRetrieved)
            {
                _saveIfDirtyMethodRetrieved = true;
                // fail silently
                try
                {
                    var assembiles = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assembiles)
                    {
                        if (assembly.GetName().Name == "UnityEditor")
                        {
                            var assetDatabaseType = assembly.GetType("UnityEditor.AssetDatabase", throwOnError: true);
                            _saveIfDirtyObjMethod = assetDatabaseType.GetMethod("SaveAssetIfDirty", new Type[] { typeof(UnityEngine.Object) });
                            _saveIfDirtyGuidMethod = assetDatabaseType.GetMethod("SaveAssetIfDirty", new Type[] { typeof(GUID) });
                            break;
                        }
                    }
                    if (useGUID)
                        return _saveIfDirtyGuidMethod != null;
                    else
                        return _saveIfDirtyObjMethod != null;

                }
                catch (Exception)
                {
                    return false;
                }
            }

            if (useGUID)
                return _saveIfDirtyGuidMethod != null;
            else
                return _saveIfDirtyObjMethod != null;
        }

        static object[] _parameters = new object[1];
#endif

        public static void SaveAssetIfDirty(UnityEngine.Object obj)
        {
#if UNITY_2021_2_OR_NEWER
            // Available in all versions.
            AssetDatabase.SaveAssetIfDirty(obj);
#elif UNITY_2020_3_OR_NEWER
            try
            {
                // Available in some:
                // Unity 2020.3.16+ has it
                // Unity 2021.1.17+ has it
                // Unity 2021.2.0+ has it
                if (cacheReflectionsAndReturnResult(useGUID: false))
                {
                    _parameters[0] = obj;
                    _saveIfDirtyObjMethod.Invoke(null, _parameters);
                }
                else
                {
                    AssetDatabase.SaveAssets();
                }
            }
            catch(Exception)
            {
                AssetDatabase.SaveAssets();
            }
#else
            // SaveAssetIfDirty is never available.
            AssetDatabase.SaveAssets();
#endif
        }

    }
}
#endif