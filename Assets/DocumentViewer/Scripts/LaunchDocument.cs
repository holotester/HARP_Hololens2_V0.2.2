using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


#if WINDOWS_UWP
using System;
using Windows.Storage;
using Windows.System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using System.Diagnostics;
#endif

namespace Microsoft.MixedReality.OpenXR.BasicSample
{
    public class LaunchDocument : MonoBehaviour
    {

        /// <summary>
        /// Opens OpenFilePicker and opens the pdf document in Microsoft Edge
        /// </summary>
        public async void PressedButton()
        {

           
#if !UNITY_EDITOR && UNITY_WSA_10_0

                    UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
                    {
                        var filepicker = new FileOpenPicker();
                        filepicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                        
                        filepicker.FileTypeFilter.Add(".pdf");

                        var file = await filepicker.PickSingleFileAsync();
                        await Launcher.LaunchFileAsync(file);

                        //Debug.Log("works");
                    }, false);



#else
            //show all pdf files in "My Documents"
            Debug.Log("else");
            UnityEngine.WSA.Launcher.LaunchUri(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), false);

#endif
            
        }
    }
}
