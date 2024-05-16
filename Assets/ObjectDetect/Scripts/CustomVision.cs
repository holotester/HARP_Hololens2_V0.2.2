using Microsoft.MixedReality.Toolkit.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.OpenXR.BasicSample
{
    public class CustomVision : MonoBehaviour
    {
        /// <summary>
        /// Creates an Instance for the CustomVision so that the methods are avaliable in other classes
        /// </summary>
        public static CustomVision Instance;

        /// <summary>
        /// Object for the json string to be deserialised into 
        /// </summary>
        private CustomVisionAnalysisObject res;

        /// <summary>
        /// Runs before the scene is loaded
        /// </summary>
        string key = "e4ef65ff51c24a5091df617edb90dbfb";

        private string modelUrl;

        IEnumerator ReadUrl(string url)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                modelUrl = @webRequest.downloadHandler.text;
            }

        }

        private void Awake()
        {
            StartCoroutine(ReadUrl("https://raw.githubusercontent.com/20145050-Vernon-Ong/LTA-Active-Mobility-Web-App/master/ModelURL.txt"));
            Instance = this;
        }

        /// <summary>
        /// Makes an API call to detect objects in an image
        /// </summary>
        public async Task<CustomVisionAnalysisObject> MakePredictionRequest(byte[] byteArray, string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", key);

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(byteArray))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var resContent = await response.Content.ReadAsStringAsync();
                Debug.Log(resContent);
                res = JsonUtility.FromJson<CustomVisionAnalysisObject>(resContent);
            }
            return res;
        }
    }
}
