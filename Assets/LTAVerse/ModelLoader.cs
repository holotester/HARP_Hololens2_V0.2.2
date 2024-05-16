using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace LTA.Holoapp
{
    public class ModelLoader : MonoBehaviour
    {
        GameObject wrapper;
        string filePath;
        public GameObject openButtonPrefab = default;
        /// <summary>
        /// Spacing between the Todo List buttons
        /// </summary>
        private int yCoord = 0;
        public Transform menu;
        /// <summary>
        /// Creates a menu filled with the preconfigured checklists
        /// </summary>
        public Transform allTodoListManager;
        /// <summary>
        /// Prefab for to create new CheckList Menu
        /// </summary>
        public GameObject todoListPrefab;

        public GameObject checklist;



        string path;
        List<string> allFiles = new List<string>();
        private void Start()
        {
            filePath = $"{Application.persistentDataPath}/Files/";
           
            wrapper = new GameObject
            {
                name = "Model"
            };

           
        }

        /// <summary>
        /// Creates a menu filled with the preconfigured checklists
        /// </summary>
        public void CreateMenu()
        {
            Debug.Log("doc path = " + path);
            Debug.Log("inside files = " + allFiles.Count);
            for (int i = 0; i < allFiles.Count; i++)
            {
                string filename = allFiles[i];
                GameObject item = Instantiate(openButtonPrefab);
                Debug.Log("file name = " +allFiles[i]);
                item.transform.SetParent(menu);
                item.transform.localScale = new Vector3(0.9560533f, 0.9295349f, 1);
                item.transform.localPosition = new Vector3(0.067f, -0.015f - yCoord * 0.04f, 0);
                yCoord++;
                item.transform.localRotation = Quaternion.Euler(0, 0, 0);
                string showFile = filename.Remove(filename.Length - 4);
                item.GetComponentInChildren<TextMeshPro>().text = showFile;
                item.GetComponentInChildren<Interactable>().OnClick.AddListener(delegate
                {
                    LoadModel($"{Application.persistentDataPath}/Files/Files/"+filename);
                });

            }
        }

        /// <summary>
        /// Opens the speific checklist clicked by utilising the ChecklistManager
        /// </summary>
        void OpenTodo(string filename)
        {
            if (GameObject.Find(filename) == null)
            {
                //StartCoroutine(EnableButtonAfterDelay(TheButton, 2f));
                GameObject todoList = Instantiate(todoListPrefab);
                todoList.name = filename;
                todoList.transform.SetParent(allTodoListManager);
                

            }
        }

        public async void DownloadFile(string url)
        {
            Debug.Log("application path = " + Application.persistentDataPath);
            string filePath = Application.persistentDataPath + "/Files.zip";
            var client = new System.Net.WebClient();
            
            //delete all existing zip files first
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            client.DownloadFile("https://firebasestorage.googleapis.com/v0/b/lta-model.appspot.com/o/Files.zip?alt=media", filePath);
            Debug.Log(@Application.persistentDataPath + "/Files/Files");
            Debug.Log(Directory.Exists(Application.persistentDataPath + "/Files/Files"));
            
            //delete all existing models in the directory
            if (Directory.Exists(Application.persistentDataPath + "/Files/Files"))
            {
                Debug.Log("deleting files");
                /*File.Delete(Application.persistentDataPath + "/Files/Files");*/
                DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/Files/Files");
                foreach(FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }
            }
            Debug.Log("files deleted");
            System.IO.Compression.ZipFile.ExtractToDirectory(filePath, Application.persistentDataPath + "/Files");
            Debug.Log("File path extracted");

            path = $"{Application.persistentDataPath}/Files/Files/";
            //string path = GetFilePath(url);
            DirectoryInfo fileee = new DirectoryInfo(path);
            //get all the models with .glb
            foreach (var file in fileee.GetFiles("*.glb"))
            {
                allFiles.Add(System.IO.Path.GetFileName(file.ToString()));
            }

            CreateMenu();
            /*

            for (int i = 0; i < allFiles.Count; i++)
            {
                string filename = allFiles[i];
                GameObject item = Instantiate(openButtonPrefab);
                Debug.Log(allFiles[i]);
                item.transform.SetParent(menu);
                item.transform.localScale = new Vector3(0.9560533f, 0.9295349f, 1);
                item.transform.localPosition = new Vector3(0.067f, -0.015f - yCoord * 0.04f, 0);
                yCoord++;
                item.transform.localRotation = Quaternion.Euler(0, 0, 0);
                string showFile = filename.Remove(filename.Length - 4);
                item.GetComponentInChildren<TextMeshPro>().text = showFile;
                item.GetComponentInChildren<Interactable>().OnClick.AddListener(delegate
                {
                    LoadModel(filename);
                });

            }*/

            /*Debug.Log("hellloooo");
            var filepicker = new Windows.Storage.Pickers.FileOpenPicker();
            filepicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            filepicker.FileTypeFilter.Add(".glb");

            string file = await filepicker.PickSingleFileAsync();

            string path = $"{Application.persistentDataPath}/Files/" + file;*/
            /* string path = $"{Application.persistentDataPath}/Files/base_new_reduced.glb";
             Environment.GetFolderPath(Application.persistentDataPath);
             Debug.Log("filepath = " + path);
              if (File.Exists(path))
             {
                 Debug.Log("Found file locally, loading...");
                 LoadModel(path);
                 return;
             }*/


            /* Debug.Log("end lo");
             StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
             {
                 if (req.isNetworkError || req.isHttpError)
                 {
                     // Log any errors that may happen
                     Debug.Log($"{req.error} : {req.downloadHandler.text}");
                 }
                 else
                 {
                     // Save the model into a new wrapper
                     LoadModel(path);
                 }
             }));*/
        }

        string GetFilePath(string url)
        {
            string[] pieces = url.Split('/');
            string filename = pieces[pieces.Length - 1];

            return $"{filePath}{filename}";
        }

        void LoadModel(string path)
        {
            ResetWrapper();
            GameObject model = Importer.LoadFromFile(path);
            model.transform.SetParent(wrapper.transform);
            //model.transform.localScale = new Vector3(1, 1, 1);
            
        }

        IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        void ResetWrapper()
        {
            if (wrapper != null)
            {
                foreach (Transform trans in wrapper.transform)
                {
                    Destroy(trans.gameObject);
                }
            }
        }
    }
}
