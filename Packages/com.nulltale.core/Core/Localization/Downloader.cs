using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Core
{
    /// <summary>
    /// HTTP downloader with WWW utility (creates instance automatically)
    /// </summary>
    [ExecuteInEditMode]
    public class Downloader : MonoBehaviour
    {
        public static event Action OnNetworkReady = () => { }; 

        private static Downloader _instance;

        public static Downloader Instance
        {
            get { return _instance ?? (_instance = new GameObject("Downloader").AddComponent<Downloader>()); }
        }

        public void OnDestroy()
        {
            _instance = null;
        
        }
	   
        public static void Download(string url, Action<UnityWebRequest> callback)
        {
            Debug.LogFormat("downloading {0}", url);
            Instance.StartCoroutine(Coroutine(url, callback));
        }

        private static IEnumerator Coroutine(string url, Action<UnityWebRequest> callback)
        {
            var www = new UnityWebRequest(url);

            yield return www;
        
            if (www.error == null)
            {
                OnNetworkReady();
            }

            callback(www);
        }
    }
}