using System.Collections;
using MRTK.Tutorials.AzureCloudServices.Scripts.Controller;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace MRTK.Tutorials.AzureCloudServices.Scripts.UX
{
    /// <summary>
    /// Handles the anchor position visual.
    /// </summary>
    public class AnchorPosition : MonoBehaviour
    {
        public TrackedObject TrackedObject => trackedObject;

        [SerializeField]
        private GameObject toolTipPanel = default;
        //[SerializeField]
        //private ObjectCardViewController objectCard = default;
        [SerializeField]
        private TextMeshPro labelText = default;

        [SerializeField]
        private SpawnAnchor testsceneController;




        private TrackedObject trackedObject;

        public void Init(TrackedObject source)
        {
            toolTipPanel.SetActive(true);
            trackedObject = source;
            //Workaround because TextMeshPro label is not ready until next frame
            StartCoroutine(DelayedInitCoroutine());
        }
        
        private IEnumerator DelayedInitCoroutine()
        {
            yield return null;
            if (trackedObject != null)
            {
                labelText.text = trackedObject.Name;
                //objectCard.Init(trackedObject);
            }
        }

        public async void NewInit(string spid)
        {
            toolTipPanel.SetActive(true);
            Debug.Log("AP NEWINIT: "+ spid);
            if (testsceneController == null)
            {
                testsceneController = FindObjectOfType<SpawnAnchor>();
            }
            //newTestingScript s = GetComponent<newTestingScript>();
            trackedObject = await testsceneController.DataBase.FindTrackedObjectBySpId(spid);
            
            if (trackedObject == null)
            {
                Debug.Log("printliao");
            }
            else
            {
                StartCoroutine(DelayedInitCoroutine());
            }
                //Debug.Log(trackedObject);
                

        }
    }
}
