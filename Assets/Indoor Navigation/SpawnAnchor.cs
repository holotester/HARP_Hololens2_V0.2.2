using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using MRTK.Tutorials.AzureCloudServices.Scripts.UX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnAnchor : MonoBehaviour
{
    public Database DataBase => database;

    [SerializeField]
    private Database database;
    [SerializeField]
    private AnchorManager sceneController1;

    public GameObject prefab,prefab2;

    public Vector3 mainPos, refPos;
    private double offsetX,offsetY,offsetZ;

    private GameObject EmptyObj;

    public Dictionary<string,GameObject> connectedActiveAnchors = new Dictionary<string,GameObject>();

    public Dictionary<string, double> edgesDict = new Dictionary<string, double>();
    public Dictionary<string, Dictionary<string, double>> VerticeDetail = new Dictionary<string, Dictionary<string, double>>();

    public Dictionary<string, Tuple<double, double>> angleDetail = new Dictionary<string, Tuple<double, double>>();
    public Dictionary<string, int> urCounter = new Dictionary<string, int>();

    public Camera cam;
    //public CreateAnchors createAnchors;
    public int offsetCounter;
    public bool corEnd = false;
    public bool rotateDir;
    Vector3 mainAnchorPos;
    public string shiftedName;
    public List<string> mainachName;
    public Material mat;
    public GameObject loaderObj;

    public Dictionary<int, int> offsets = new Dictionary<int, int>();
    public Dictionary<int, bool> corChecker = new Dictionary<int, bool>();
    public Dictionary<int, int> offsetLR = new Dictionary<int, int>();
    public int maxx=0;
    /// <summary>
    /// Get the counter to determine when is created
    /// </summary>
    /// 

    public async Task GetMaxInt()
    {
        var allConnect = await database.GetAllTrackedNBObjects();
        foreach (var t in allConnect)
        {
            if (maxx < t.counter)
            {
                maxx = t.counter;
            }
            if (!offsets.ContainsKey(t.counter))
            {
                offsets.Add(t.counter, default(int));
                corChecker.Add(t.counter, false);
                offsetLR.Add(t.counter, 0); 
            }
        }
    }
    /// <summary>
    /// Find all the anchor
    /// </summary>
    public async void SubmitQuery()
    {
        var project = await FindObjectAll();
        //foreach(var a in project)
        //{
        //    Debug.Log(a.SpatialAnchorId);
        //}
        //Debug.Log(project);
        StartFindLocation(project);
    }
    /// <summary>
    /// Find all the main anchor name to be put to the dropdown
    /// </summary>
    public async Task addMainSA()
    {
        var s = FindObjectOfType<AnchorManager>();
        
        foreach(KeyValuePair<string,AnchorPosition> aA in s.activeAnchors)
        {
            var z = await database.FindTrackedObjectBySpId(aA.Key);
            EmptyObj = new GameObject("holder");
            EmptyObj.transform.position = aA.Value.transform.position;
            
            if(z.Name == "Ref")
            {
                refPos = aA.Value.transform.position;
            }
            else
            {
                mainPos = aA.Value.transform.position;
                connectedActiveAnchors[z.Name] = EmptyObj;
                mainachName.Add(z.Name);
            }
        }
    }
    /// <summary>
    /// Generate the connected anchors
    /// </summary>
    public async void findNextAnchor(string key)
    {
        //loaderObj.SetActive(true);
        var queryData = await database.FindNextConnected(key);
        
        foreach (var i in queryData)
        {
            Debug.Log(i);
            
            Vector3 recoverC;
            if (key == "main")
            {
                var z = Resources.FindObjectsOfTypeAll<CreateAnchors>();
                var s = z[0];
                await s.GetMainPos(i.ConnectSpatialAnchor);
                recoverC = s.GetMainCoord.position;
                mainAnchorPos = s.GetMainCoord.position;
            }
            else
            {
                recoverC = connectedActiveAnchors[i.ConnectSpatialAnchor].transform.position;
            }
            var x = Instantiate(prefab);
            float x_value = recoverC.x - (float)i.Xvalue;
            float y_value = recoverC.y - (float)i.Yvalue;
            float z_value = recoverC.z - (float)i.Zvalue;

            Debug.Log("1:  "+x_value + " " + y_value + " " + z_value);
            Vector3 loc = new Vector3(x_value, y_value, z_value);
            Debug.Log("loc " + loc);
            x.transform.position = loc;
            var dddd = refPos - mainPos;
            Debug.Log(i.Name + " : " + i.Angle + " : " + Vector3.Angle(x.transform.position - mainAnchorPos, dddd));
            Debug.Log(i.counter+":"+offsets[i.counter] + ":" + default(int));
            if (offsets[i.counter] == default(int))
            {
                shiftedName = i.Name;
                Debug.Log("SHIFTEEDNAME: " + i.Name);
                StartCoroutine(WrapperCor((float)i.Angle, x, mainAnchorPos, dddd, i.counter, i.Dir));

            }

            var nameTag = x.gameObject.transform.GetChild(1).GetComponent<ToolTip>();
            x.transform.GetChild(0).GetComponent<Renderer>().material = mat;
            nameTag.ToolTipText = i.Name;
            connectedActiveAnchors[i.Name] = x;
            urCounter[i.Name] = i.counter;
            angleDetail[i.Name] = new System.Tuple<double, double>(i.Angle, (double)Vector3.Angle(x.transform.position - mainAnchorPos, dddd));
            //loaderObj.SetActive(false);

            findNextAnchor(i.Name);
        }
        Debug.Log("TOTAL ACTIVE ANCHOR LOADED: "+connectedActiveAnchors.Count);
        return;
    }
    /// <summary>
    /// Corountine to shift anchor back to original location
    /// </summary>
    IEnumerator WrapperCor(float targetAngle, GameObject x, Vector3 ori, Vector3 dir, int offsetNo, string LR)
    {
        yield return StartCoroutine(UpdateLoc(targetAngle,x,ori,dir,offsetNo,LR));
        var aa = !corChecker.ContainsValue(false);
        Debug.Log("completed : "+ aa);

        if (!corChecker.ContainsValue(false))
        {
            checkcor();
        }
        //findNextAnchor(nextLF);
        //checkcor();
    }

    /// <summary>
    /// Rotate achor back to it original angle
    /// </summary>
    IEnumerator UpdateLoc(float targetAngle, GameObject x, Vector3 ori, Vector3 dir, int offsetNo, string leftRight)
    {
        offsets[offsetNo] = 0;
        var directionSA = refPos - ori;
        var temp = targetAngle - Vector3.Angle(x.transform.position - ori, dir);
        Debug.Log(Vector3.Angle(x.transform.position - ori, dir));
        x.transform.RotateAround(ori, Vector3.up, 5 * Time.deltaTime);
        var temp2 = targetAngle - Vector3.Angle(x.transform.position - ori, dir);
        Debug.Log(Vector3.Angle(x.transform.position - ori, dir));
        Debug.Log("Temp: " + temp + " Temp2: " + temp2); 
        if (temp < temp2)
        {
            offsetLR[offsetNo] = 1;
            offsets[offsetNo] = 1;
        }
        else
        {
            offsetLR[offsetNo] = 2;
        }
        while (true)
        {
            Debug.Log(targetAngle + " : " + Vector3.Angle(x.transform.position-ori, dir) + " : " + rotateDir + " : " + (5*Time.deltaTime));
            if (offsetLR[offsetNo] == 1)
            {
                //rotateDir = true;
                
                x.transform.RotateAround(ori, Vector3.up, 5 * Time.deltaTime);
                offsets[offsetNo]++;
                //if (offsetLR[offsetNo] == 0)
                //{
                //    offsetLR[offsetNo] = 1;
                //}
            }
            //else if (Vector3.Angle(x.transform.position - ori, dir) >= Mathf.RoundToInt(targetAngle) || offsetLR[offsetNo] == 2)
            else if (offsetLR[offsetNo] == 2)
            {
                //rotateDir = false;
                x.transform.RotateAround(ori, Vector3.down, 5 * Time.deltaTime);
                offsets[offsetNo]++;
                //if (offsetLR[offsetNo] == 0)
                //{
                //    offsetLR[offsetNo] = 2;
                //}
            }
            if (Mathf.RoundToInt(targetAngle) - 1 <= Vector3.Angle(x.transform.position - ori, dir) && Vector3.Angle(x.transform.position - ori, dir) <= Mathf.RoundToInt(targetAngle) + 1 )
            {
                Debug.Log((x.transform.position-ori) + " : " + directionSA);
                //if(leftRight == "left")
                //{
                //if ((x.transform.position - ori).x > directionSA.x)
                //{
                Debug.Log("reached :" + offsets[offsetNo]);
                x.transform.GetChild(0).GetComponent<Renderer>().material = mat;
                corChecker[offsetNo] = true;
                yield break;
                //}
                //}
                //else
                //{
                //    if ((x.transform.position - ori).x > directionSA.x)
                //    {
                //        Debug.Log("reached2 :" + offsets[offsetNo]);
                //        x.transform.GetChild(0).GetComponent<Renderer>().material = mat;
                //        corChecker[offsetNo] = true;
                //        yield break;
                //    }
                //}
                
            }
            yield return new WaitForSeconds(2/3);
        }
    }
    /// <summary>
    /// Shift all the reamining connected anchor
    /// </summary>
    public void checkcor()
    {
        var tempDict = new Dictionary<string, GameObject>(connectedActiveAnchors);
        tempDict.Remove(shiftedName);
        foreach(var m in mainachName)
        {
            tempDict.Remove(m);
        }
        foreach (KeyValuePair<string, GameObject> spawned in tempDict)
        {
            Debug.Log("LLULLULLL: " + spawned.Key);
            int whatUrCounter = urCounter[spawned.Key];

            if (offsetLR[whatUrCounter] == 1)
            {
                spawned.Value.transform.RotateAround(mainAnchorPos, Vector3.up, 5 * Time.deltaTime * offsets[whatUrCounter]);
                
            }
            else
            {
                spawned.Value.transform.RotateAround(mainAnchorPos, Vector3.down, 5 * Time.deltaTime * offsets[whatUrCounter]);
            }
            spawned.Value.transform.GetChild(0).GetComponent<Renderer>().material = mat;
        }
        loaderObj.SetActive(false);
    }
    

    public async void GetConnect()
    {

        await addMainSA();
        await GetMaxInt();
        Debug.Log("MAXXXXXX " + maxx);
        string lookFor = "main";
        findNextAnchor(lookFor);
        

    }
    private async Task<List<TrackedObject>> FindObjectAll()
    {
        var projectFromDb1 = await database.GetAllTrackedObjects();
        //Debug.Log(projectFromDb);
        Debug.Log(projectFromDb1.Count);
        return projectFromDb1;
    }
    private async Task<List<NearbyObject>> FindConnectObjAll()
    {
        var projectFromDb1 = await database.GetAllTrackedNBObjects();
        return projectFromDb1;
    }
    public void StartFindLocation(List<TrackedObject> trackedObject)
    { 
        foreach (var a in trackedObject)
        {
            Debug.Log(a.SpatialAnchorId);
        }
        //Debug.Log(project);
        //List<string> anchorsToFind = new List<string>();
        //foreach (var item in trackedObject)
        //{
        //    anchorsToFind.Add(item.SpatialAnchorId);
        //}
        sceneController1.FindAnchorNew(trackedObject);
    }
    private void HandleOnAnchorFound(object sender, EventArgs e)
    {
        Debug.Log("ObjectCardViewController.HandleOnAnchorFound");
        sceneController1.OnFindAnchorSucceeded -= HandleOnAnchorFound;
        
    }
}
