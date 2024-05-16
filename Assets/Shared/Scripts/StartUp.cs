using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.OpenXR.BasicSample
{
    public class StartUp : MonoBehaviour
    {
        /// <summary>
        /// Ensure that the code in OnEnable only runs once
        /// </summary>
        private static bool IsFirstLoad = true;

        /// <summary>
        /// Runs the Splash screen when Scene is loaded
        /// </summary>
        private async void OnEnable()
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;

                Debug.Log("hello!");
                await Task.Delay(4000);
                HideIntro();
            }
        }

        /// <summary>
        /// Loads Main Menu Scene
        /// </summary>
        void HideIntro()
        {
            CoreServices.SceneSystem.LoadContent("MainMenu", LoadSceneMode.Single);
            Debug.Log("Working!");
            
        }
    }
}
