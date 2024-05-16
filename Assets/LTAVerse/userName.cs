using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LTA.Holoapp
{
    
    public class userName : MonoBehaviour
    {
        public TextMeshPro uName = default;
        public TextMeshPro inputText = default;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

       

        public void onClick()
        {
            Debug.Log("preview text = " + inputText.text);
            uName.SetText(inputText.text);

        }
        // Update is called once per frame
        void Update()
        {
            uName.SetText(inputText.text);
        }
    }
}




