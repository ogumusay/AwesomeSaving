using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System;

namespace AwesomeSaving
{
    public class SavingHelper : MonoBehaviour
    {
        private static SavingHelper _instance;

        #region Singleton

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            DontDestroyOnLoad(_instance.gameObject);
        }

        public static SavingHelper GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            return CreateNewInstance();
        }

        private static SavingHelper CreateNewInstance()
        {
            GameObject instance = new GameObject("Saving Helper", typeof(SavingHelper));
            _instance = instance.GetComponent<SavingHelper>();
            return _instance;
        }

        #endregion

        #region Save

        private Queue<SaveRequest> _saveRequestQueue = new Queue<SaveRequest>();
        private bool _isSaving = false;

        public void Save(string path, object obj, Action<object> onComplete = null)
        {            
            SaveRequest request = new SaveRequest(path, obj, onComplete);
            _saveRequestQueue.Enqueue(request);
            Debug.Log("Created Request and Enqueued for: " + path);
            if (!_isSaving)
            {
                Debug.Log("Starting Saving Process: " + path);
                StartCoroutine(WriteAsyncCoroutine());
            }
            else
            {
                Debug.Log("Saving System is busy! Waiting...: " + path);
            }
        }

        private IEnumerator WriteAsyncCoroutine()
        {
            _isSaving = true;
            SaveRequest request = _saveRequestQueue.Dequeue();
            Debug.Log("Saving...: " + request.Path);
            Task async = WriteAsync(request);
            while (!async.IsCompleted)
            {
                yield return new WaitForEndOfFrame();
            }

            if (_saveRequestQueue.Count > 0)
            {
                Debug.Log("Done! " + request.Path + " Proceeding to next request.");
                request.OnComplete?.Invoke(request.Content);
                yield return WriteAsyncCoroutine();
            }
            else
            {
                Debug.Log("Done!" + request.Path);
                _isSaving = false;
                request.OnComplete?.Invoke(request.Content);
            }
        }

        private async Task WriteAsync(SaveRequest request)
        {
            string serializedObj = JsonConvert.SerializeObject(request.Content, Formatting.Indented);
            StreamWriter streamWriter = new StreamWriter(request.Path, append: false);
            await streamWriter.WriteAsync(serializedObj);
            streamWriter.Close();
        }

        #endregion

        #region Load

        public T Load<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                Debug.LogError(path + " doesnt exist");
                return null;
            }

            StreamReader streamReader = new StreamReader(path);
            string serializedObj = streamReader.ReadToEnd();
            streamReader.Close();

            T obj = JsonConvert.DeserializeObject<T>(serializedObj);
            return obj;
        }

        #endregion
    }

    public class SaveRequest
    {
        public string Path { get; private set; }
        public object Content { get; private set; }
        public Action<object> OnComplete { get; private set; }
        public SaveRequest(string path, object content, Action<object> onComplete)
        {
            Path = path;
            Content = content;
            OnComplete = onComplete;
        }
    }   
}