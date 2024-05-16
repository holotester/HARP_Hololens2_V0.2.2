using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;
using System.IO;
using System.Xml;

#if ENABLE_WINMD_SUPPORT
using Windows.Media.Capture;
using System;
#endif

namespace Microsoft.MixedReality.OpenXR.BasicSample
{
    public class ObjectTrack : MonoBehaviour
    {
        /// <summary>
        /// Sets the probability threshold for the facial detection
        /// </summary>
        private static float probabilityThreshold = 0.4f;

        /// <summary>
        /// Sets an Instance for the FaceTracking so that other methods are able to access the methods
        /// </summary>
        public static ObjectTrack Instance;

        /// <summary>
        /// Locks the bool to the camera available
        /// </summary>
        private bool camAvailable;

        /// <summary>
        /// Runs the Hololens camera with UnityEngine's camera
        /// </summary>
        private WebCamTexture cam;

        ///// <summary>
        ///// Spawns cursor for the Main Camera
        ///// </summary>
        //private Texture defaultBackground;

        /// <summary>
        /// Shows the live view of Hololens camera
        /// </summary>
        public RawImage background;

        /// <summary>
        /// Scales the picture shown to the Hololens aspect ratio
        /// </summary>
        public AspectRatioFitter fit;

        /// <summary>
        /// Gets the Face Label
        /// </summary>
        [SerializeField]
        private GameObject objectPrefab;

        /// <summary>
        /// Shows the static image when the face is detected
        /// </summary>
        public RawImage viewTakenPhoto;

        /// <summary>
        /// Sets the aspect ratio for the static image when the face is detected
        /// </summary>
        public AspectRatioFitter viewTakenPhotoFit;

        /// <summary>
        /// To unhide static image when face detected
        /// </summary>
        public GameObject images;

        /// <summary>
        /// To get Main Camera
        /// </summary>
        private GameObject camera;

        /// <summary>
        /// To get Main Camera
        /// </summary>
        private string modelUrl;
        private XmlNodeList HARP;
        private XmlElement root;
        private readonly XmlDocument xmldocument = new XmlDocument();

        IEnumerator ReadUrl(string url)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                xmldocument.LoadXml(@webRequest.downloadHandler.text);
                root = xmldocument.DocumentElement;
                HARP = root.SelectNodes("/HARP/ObjectDetection");
                modelUrl = HARP[0]["text"].InnerText;
            }

        }
        private void Awake()
        {
            StartCoroutine(ReadUrl("https://onedrive.live.com/download?resid=64C81AD110C9659A%21492&authkey=!ALWEtQefgENEetY"));
            Instance = this;

        }

        private static Vector3 cameraPos = new Vector3(0, 0, 0);

        private Vector2 GetCenter(Prediction p)
        {
            return new Vector2((float)(p.boundingBox.left + (0.5 * p.boundingBox.width)),
                (float)(p.boundingBox.top + (0.5 * p.boundingBox.height)));
        }

        /// <summary>
        /// Starts the live view of the camera when the scene is loaded and active
        /// </summary>
        private void OnEnable()
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            //defaultBackground = background.texture;
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                Debug.Log("No camera detected");
                camAvailable = false;
                return;
            }

            cam = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            if (cam == null)
            {
                Debug.Log("Unable to find back camera");
                return;
            }
            cam.Play();
            background.texture = cam;
            camAvailable = true;

            if (!camAvailable) return;

            float ratio = (float)cam.width / (float)cam.height;
            fit.aspectRatio = ratio;
            float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }

        /// <summary>
        /// Stops the camera when the Scene is unloaded
        /// </summary>
        private void OnDisable()
        {
            cam.Stop();
        }

        /// <summary>
        /// Takes photo from the Hololens camera and starts to detect faces from the image
        /// </summary>
        public async void TakePhoto()
        {
/*            cameraPos = CameraCache.Main.transform.position;
            Debug.Log("Taking photo!!");
            Texture2D _texture2D = new Texture2D(cam.width, cam.height);

            _texture2D.SetPixels32(cam.GetPixels32());
            CustomVision customVision = gameObject.AddComponent<CustomVision>();
            byte[] bArray = _texture2D.EncodeToPNG();
            Debug.Log("Taken photo!");
            var jsonResponse = await CustomVision.Instance.MakePredictionRequest(bArray);
            Debug.Log("Finished CustomVision");
            viewTakenPhoto.texture = _texture2D;
            images.GetComponent<RawImage>().texture = _texture2D;

            CreateBoundingBox(jsonResponse);*/

            Texture2D texture2D = new Texture2D(cam.width, cam.height);
            texture2D.SetPixels32(cam.GetPixels32());
            viewTakenPhoto.texture = texture2D;
            float ratio = (float)cam.width / (float)cam.height;
            viewTakenPhotoFit.aspectRatio = ratio;
            float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
            viewTakenPhoto.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;

            viewTakenPhoto.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            Debug.Log("Taking photo!!");
            Texture2D _texture2D = new Texture2D(cam.width, cam.height);

            _texture2D.SetPixels32(cam.GetPixels32());
            byte[] bArray = _texture2D.EncodeToPNG();
            _texture2D.LoadImage(bArray);
            images.GetComponent<RawImage>().texture = _texture2D;
            _texture2D.Apply();
            viewTakenPhoto.texture = _texture2D;

            CustomVision customVision = gameObject.AddComponent<CustomVision>();
            Debug.Log("Taken photo!");
            var jsonResponse = await customVision.MakePredictionRequest(bArray, modelUrl);

            CreateBoundingBox(jsonResponse);

        }

        public async void CreateBoundingBox(CustomVisionAnalysisObject jsonContent)
        {
            Debug.Log("starting bounding box");
            Transform cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            var heightFactor = Screen.height / Screen.width;
            var topCorner = cameraTransform.position + cameraTransform.forward -
                            cameraTransform.right / 2f +
                            cameraTransform.up * heightFactor / 2f;



            var sortedPredictions = jsonContent.predictions.OrderBy(p => p.probability).ToList().FindAll(e => e.probability > probabilityThreshold);

            foreach (Prediction prediction in sortedPredictions)
            {
                Debug.Log(prediction.tagName + ", " + prediction.probability);
                changeText(prediction);

            }
            if (!images.activeSelf) images.SetActive(true);
            Debug.Log("Starting prediction");
        }

        private void CreatePoint(Prediction p, int heightFactor, Vector3 topCorner, Transform cameraTransform)
        {
            var center = GetCenter(p);
            Debug.Log("Center of point: " + center);
            var recognizedPos = topCorner + cameraTransform.right * center.x -
                                cameraTransform.up * center.y * heightFactor;
            Debug.Log("Recognised Pos:  " + recognizedPos);

            //Ray ray = MRTKCam.ScreenPointToRay(recognizedPos);
            //Debug.Log(Physics.Raycast(ray.origin, ray.direction, out rayCastHit, Mathf.Infinity, GetSpatialMeshMask()));

            //Debug.Log(rayCastHit.point);

            images.SetActive(true);
            var label = objectPrefab;
            label.GetComponentInChildren<TextMeshPro>().text = p.tagName;
            //var label = Instantiate(objectPrefab);
            //label.GetComponentInChildren<ToolTip>().ToolTipText = p.tagName;
            //label.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            //label.transform.localPosition = new Vector3(recognizedPos.x, rayCastHit.point.y, rayCastHit.point.z);
            //label.transform.parent = objectParent.transform;
        }

        public float highestP;

        private void changeText(Prediction p)
        {

            if (p.probability > highestP)
            {
                var label = objectPrefab;
                label.GetComponentInChildren<TextMeshPro>().text = p.tagName;
                highestP = p.probability;
            }

        }
    }
}

