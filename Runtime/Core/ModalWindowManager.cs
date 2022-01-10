using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GE.ModalWindows
{
    [CreateAssetMenu(menuName = "ModalWindow/Manager")]
    public class ModalWindowManager : RuntimeScriptableSingleton<ModalWindowManager>
    {
        public string prefabsFolder;

        public int defaultLayer = 10;
        public static int DefaultLayer => Instance.defaultLayer;
        public List<MessageToModalPair> pairs = new List<MessageToModalPair>();

        private readonly Dictionary<string, GameObject> _typeToPrefab = new Dictionary<string, GameObject>();

        public void Enqueue(BaseModalMessage baseModalMessage)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Only in playmode");
                return;
            }
            ModalWindowsController.Enqueue(baseModalMessage);
        }

        public void Show(BaseModalMessage baseModalMessage)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Only in playmode");
                return;
            }
            ModalWindowsController.Show(baseModalMessage);
        }

        
#if UNITY_EDITOR
        private void OnValidate()
        {
            ScanPrefabs();
        }

        private void ScanPrefabs()
        {
            List<MessageToModalPair> validPairs = pairs.FindAll(ValidPair);
            pairs.Clear();
            pairs.AddRange(validPairs);
            foreach (Type subclassType in GetAllSubclassTypes<BaseModalMessage>())
                TryToAdd(subclassType);

            FillPrefabs();
        }

        private void FillPrefabs()
        {
            string path = $"{Application.dataPath}/{prefabsFolder}";
            path = path.Replace("Assets/Assets/", "Assets/");
            
            var prefabs = LoadFilesInFolder<GameObject>(path, "*.prefab", SearchOption.TopDirectoryOnly);

            Dictionary<string, BaseModalWindow> prefabComponents = new Dictionary<string, BaseModalWindow>();

            foreach (var go in prefabs)
            {
                var windowComponent = go.GetComponent<BaseModalWindow>();
                prefabComponents.Add(windowComponent.GetMessageType().FullName, windowComponent);
            }

            for (var index = pairs.Count - 1; index >= 0; index--)
            {
                var messageToModalPair = pairs[index];
                if (messageToModalPair.prefab == null)
                {
                    var toModalPair = messageToModalPair;
                    Debug.Log(messageToModalPair.typeName);
                    toModalPair.prefab = prefabComponents[messageToModalPair.typeName].gameObject;
                    pairs.Remove(messageToModalPair);
                    pairs.Add(toModalPair);
                }
            }
        }

        public static T[] LoadFilesInFolder<T>(string folderPath, string pattern, SearchOption searchOption) where T : UnityEngine.Object
        {
            string[] files = Directory.GetFiles(folderPath, pattern, searchOption);
            T[] results = new T[files.Length];
            for (var index = 0; index < files.Length; index++)
            {
                string file = files[index];
                string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                results[index] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            return results;
        }
        private bool ValidPair(MessageToModalPair obj)
        {
            if (obj.prefab == null) return false;
            var baseModalWindow = obj.prefab.GetComponent<BaseModalWindow>();
            if (baseModalWindow == null)
                return false;
            string messageTypeName = baseModalWindow.GetMessageType().FullName;
            return obj.typeName == messageTypeName;
        }

        private void TryToAdd(Type type)
        {
            string fullName = type.FullName;
            if (!pairs.Exists(x => x.typeName == fullName))
                pairs.Add(new MessageToModalPair(){typeName = fullName });

            var pair = pairs.Find(x => x.typeName == fullName);
        }

        private ModalWindowsController ModalWindowsController => Application.isPlaying && ModalWindowsController.Instance != null?ModalWindowsController.Instance:null;

#endif
        private static IEnumerable<Type> GetAllSubclassTypes<T>() 
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                select type;
        }

        public static GameObject GetWindow(string messageTypeName)
        {
            if (Instance._typeToPrefab.TryGetValue(messageTypeName, out GameObject go))
                return go;
            go = Instance.pairs.Find(x => x.typeName == messageTypeName).prefab;
            Instance._typeToPrefab.Add(messageTypeName, go);
            return go;
        }
    }

    [System.Serializable]
    public struct MessageToModalPair
    {
        public string typeName;
        public GameObject prefab;
    }
}