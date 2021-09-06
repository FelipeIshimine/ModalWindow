using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.ModalWindows.Core
{
    [CreateAssetMenu(menuName = "ModalWindow/Manager")]
    public class ModalWindowManager : RuntimeScriptableSingleton<ModalWindowManager>
    {
        public string prefabsFolder;

        public int defaultLayer = 10;
        public static int DefaultLayer => Instance.defaultLayer;
        public List<MessageToModalPair> pairs = new List<MessageToModalPair>();

        private Dictionary<string, GameObject> _typeToPrefab = new Dictionary<string, GameObject>();

        
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

        public void CloseCurrent()
        {
            ModalWindowsController.CloseCurrent();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
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

            foreach (var o in prefabs)
            {
                var prefab = (GameObject)o;
                var windowComponent = prefab.GetComponent<BaseModalWindow>();
                prefabComponents.Add(windowComponent.GetMessageType().Name, windowComponent);
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
            string messageTypeName = baseModalWindow.GetMessageType().Name;
            return obj.typeName == messageTypeName;
        }

        private void TryToAdd(Type type)
        {
            if (!pairs.Exists(x => x.typeName == type.Name))
                pairs.Add(new MessageToModalPair(){typeName = type.Name});

            var pair = pairs.Find(x => x.typeName == type.Name);
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
            Debug.Log(messageTypeName);
            Debug.Log(Instance == null);
            if (Instance._typeToPrefab.TryGetValue(messageTypeName, out GameObject go))
                return go;
            Debug.Log( Instance.pairs == null);
            go = Instance.pairs.Find(x => x.typeName == messageTypeName).prefab;
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