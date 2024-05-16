using Microsoft.MixedReality.Toolkit.UI;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CreateAnchors : MonoBehaviour
{
    [SerializeField]
    private Database sceneController;
    [SerializeField]
    private AnchorManager sceneController1;
    [Header("UI Elements")]
    [SerializeField]
    private GameObject anchorSphere;
    [SerializeField]
    private GameObject testing;
    //[SerializeField]
    //private TMP_Text hintLabel = default;
    [SerializeField]
    private TMP_InputField inputField = default;
    [SerializeField]
    private InteractableToggleCollection anchorTypeRadialSet;
    [SerializeField]
    private TMP_Dropdown connectAnchorDropdown;
    [SerializeField]
    private TMP_Dropdown deleteAnchorDropdown;

    //public Camera mycam;

    private TrackedObject trackedObject;
    private NearbyObject trackedNBObject;
    private int selectedType;
    private List<string> main_Anchor = new List<string>();
    public List<string> m_DropOptions = new List<string>();
    public Transform GetMainCoord;
    //public Transform GetMainCoordR;
    //public Vector3 GetRefCoord;
    private int ddSize;

    public int max = 0;

    public GameObject prefab;

    /// <summary>
    /// Set up the dropdown for creating anchor
    /// </summary>
    public async void Awake()
    {
        Debug.Log("Awak");
        connectAnchorDropdown.options.Clear();
        //List<string> m_DropOptions = new List<string> { "Home", "Office" };
        await GetMainAchInfo();
        Debug.Log(m_DropOptions.Count);
        connectAnchorDropdown.AddOptions(m_DropOptions);
        deleteAnchorDropdown.AddOptions(m_DropOptions);
        ddSize = connectAnchorDropdown.options.Count;

        await GetMaxInt();


    }
    /// <summary>
    /// Update dropdown after one is created
    /// </summary>
    public async void updateDD()
    {
        List<string> anchorNames = new List<string>();
        //m_DropOptions.Add("HELLO");
        foreach (var anchorName in connectAnchorDropdown.options)
        {
            anchorNames.Add(anchorName.text);
        }
        await GetMainAchInfo();
        var z = m_DropOptions.Except(anchorNames);
        connectAnchorDropdown.AddOptions(z.ToList());
        
    }

    /// <summary>
    /// Get the position of the spatial anchor
    /// </summary>
    public async Task GetMainPos(string mainName)
    {
        TrackedObject SpactialID = await sceneController.FindTrackedObjectByName(mainName);
        //TrackedObject SpactialIDRef = await sceneController.FindTrackedObjectByName("Ref");
        var s = FindObjectOfType<AnchorManager>();
        Debug.Log(s.activeAnchors.Count);
        //GetMainCoord = new Vector3(0, 0, 0);
        
        GetMainCoord = s.activeAnchors[SpactialID.SpatialAnchorId].transform;
        Debug.Log(GetMainCoord);
        
    }
    /// <summary>
    /// Ge the spatial anchor info
    /// </summary>
    public async Task GetMainAchInfo()
    {
        var z = await sceneController.GetAllTrackedObjects();
        foreach(var t in z)
        {
            if (!m_DropOptions.Contains(t.Name))
            {
                if(t.Name != "Ref")
                {
                    m_DropOptions.Add(t.Name);
                    main_Anchor.Add(t.Name);
                }
                
            }
            
        }
        var w = await sceneController.GetAllTrackedNBObjects();
        foreach (var t in w)
        {
            if (!m_DropOptions.Contains(t.Name))
            {
                m_DropOptions.Add(t.Name);
            }

        }

        //Debug.Log(m_DropOptions.Count);
    }
    /// <summary>
    /// Determine creating spatial anchor or connected anchor
    /// </summary>
    public async void anchorPlacment()
    {
        selectedType = anchorTypeRadialSet.CurrentIndex;
        if (selectedType == 0)
        {
            var project = await CreateObject(inputField.text);
            if (project != null)
            {
                Debug.Log("Next Step");
                //Debug.Log(project.SpatialAnchorId);
                if (inputField.text == "Ref")
                {
                    OpenSpatialAnchorsFlow(project, "Ref");
                }
                else
                {
                    OpenSpatialAnchorsFlow(project, "main");
                }
            }
        }
        else
        {
            var project = await CreateConnectedObject(inputField.text);
            Debug.Log("IN" + project);
            if (project != null)
            {
                Debug.Log("Next Step Connected");
                Getxyz(project);
            }
        }

    }
    private async Task<TrackedObject> CreateObject(string searchName)
    {
        //hintLabel.SetText(loadingText);
        //hintLabel.gameObject.SetActive(true);
        var trackedObject = await sceneController.FindTrackedObjectByName(searchName);
        if (trackedObject == null)
        {
            trackedObject = new TrackedObject(searchName);

        }

        //hintLabel.gameObject.SetActive(false);
        return trackedObject;
    }
    private async Task<NearbyObject> CreateConnectedObject(string searchName)
    {
        //hintLabel.SetText(loadingText);
        //hintLabel.gameObject.SetActive(true);
        //var trackedNBObject = await sceneController.FindTrackedObjectByName(searchName);
        var trackedNBObject = new NearbyObject(searchName);
        return trackedNBObject;
    }
    public void OpenSpatialAnchorsFlow(TrackedObject source, string key)
    {
        Transform a = anchorSphere.transform;
        trackedObject = source;
        Debug.Log(trackedObject.SpatialAnchorId);
        //sceneController1.CreateAnchor(a);
        sceneController1.StartPlacingAnchor2(trackedObject);
        sceneController1.OnCreateAnchorSucceeded += HandleOnCreateAnchorSucceeded;
        trackedObject.PartitionKey = key;
        sceneController1.CreateAnchor(a);
    }
    /// <summary>
    /// Creating of the anchor
    /// </summary>
    public async void Getxyz(NearbyObject source)
    {
        Transform connectedCoord;
        Transform dirCoord;
        await GetMainPos("Ref");
        dirCoord = GetMainCoord;
        Debug.Log(dirCoord);
        trackedNBObject = source;
        if (main_Anchor.Contains(connectAnchorDropdown.options[connectAnchorDropdown.value].text))
        {
            await GetMainPos(connectAnchorDropdown.options[connectAnchorDropdown.value].text);
            connectedCoord = GetMainCoord;
            trackedNBObject.PartitionKey = "main";
            Debug.Log("hello got main");
        }
        else
        {
            Debug.Log("hello no main");
            var cal = FindObjectOfType<SpawnAnchor>();
            connectedCoord = cal.connectedActiveAnchors[connectAnchorDropdown.options[connectAnchorDropdown.value].text].transform;
            Debug.Log("tT "+connectedCoord.position);
            trackedNBObject.PartitionKey = connectAnchorDropdown.options[connectAnchorDropdown.value].text;
            
        }
        Debug.Log("AAAAAAAAAAAAAA");
        Debug.Log(dirCoord+":"+connectedCoord);

        Transform a = anchorSphere.transform;
        //Debug.Log(connectedCoord.position.x);
        Debug.Log(a);

        trackedNBObject.Xvalue = connectedCoord.position.x - a.transform.position.x;
        ////a.transform.localRotation.
        trackedNBObject.Yvalue = connectedCoord.position.y - a.transform.position.y;
        trackedNBObject.Zvalue = connectedCoord.position.z - a.transform.position.z;


        trackedNBObject.ConnectSpatialAnchor = connectAnchorDropdown.options[connectAnchorDropdown.value].text;
        trackedNBObject.Distance = Vector3.Distance(a.transform.position, connectedCoord.position);

        var directionSA = dirCoord.position - connectedCoord.position;
        var objDir = a.transform.position - connectedCoord.position;
        trackedNBObject.Angle = Vector3.Angle(objDir, directionSA);
        Debug.Log(trackedNBObject.Angle);

        if (a.transform.position.x < directionSA.x)
        {
            trackedNBObject.Dir = "left";
        }
        else
        {
            trackedNBObject.Dir = "right";
        }

        trackedNBObject.counter = max + 1;

        float testDD = Vector3.Distance(a.transform.position, connectedCoord.position);
        Debug.Log("C Dist: " + testDD);
        Debug.Log(trackedNBObject.Name);

        var x = Instantiate(prefab);
        x.transform.position = a.position;
        var nameTag = x.gameObject.transform.GetChild(1).GetComponent<ToolTip>();
        nameTag.ToolTipText = source.Name;
        var s = FindObjectOfType<SpawnAnchor>();
        s.connectedActiveAnchors[trackedNBObject.Name] = x;

        await sceneController.UpdateAdjacentTable(trackedNBObject);
    }

    private async void HandleOnCreateAnchorSucceeded(object sender, string id)
    {
        sceneController1.OnCreateAnchorSucceeded -= HandleOnCreateAnchorSucceeded;
        //sceneController.StartCamera();
        trackedObject.SpatialAnchorId = id;
        Debug.Log(trackedObject.SpatialAnchorId);
        await sceneController.UploadOrUpdate(trackedObject);

    }


    public async Task GetMaxInt()
    {
        var allConnect = await sceneController.GetAllTrackedNBObjects();
        foreach (var t in allConnect)
        {
            if (max < t.counter)
            {
                max = t.counter;
            }
        }

    }
    public void resetRB()
    {
        anchorTypeRadialSet.SetSelection(0);
    }
    

}
