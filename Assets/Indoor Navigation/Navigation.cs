using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using MRTK.Tutorials.AzureCloudServices.Scripts.UX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public Database DataBase => database;

    [SerializeField]
    private Database database;
    [SerializeField]
    private CreateAnchors createAnchors;

    [SerializeField]
    private TMP_Dropdown startDropdown;
    [SerializeField]
    private TMP_Dropdown endDropdown;
    public GameObject linePrefab;

    public Dictionary<string, GameObject> connectedActiveAnchors = new Dictionary<string, GameObject>();
    public Dictionary<string, double> edgesDict = new Dictionary<string, double>();
    public Dictionary<string, Dictionary<string, double>> VerticeDetail = new Dictionary<string, Dictionary<string, double>>();
    private List<GameObject> lineList = new List<GameObject>();

    private Vector3 desti;

    public Camera cam;
    public Camera[] allCameras;

    private GameObject EmptyObj;

    /// <summary>
    /// Get the camera obj
    /// </summary>
    private void Start()
    {
        allCameras = FindObjectsOfType<Camera>();
    }
    /// <summary>
    /// After line generated, remove line when user is 1m away from the destination
    /// </summary>
    public void UpdateLoc()
    {
        if(desti != new Vector3(0,0,0))
        {

            var closeToDest = 1;
            Debug.Log(Vector3.Distance(desti, allCameras[0].transform.position));
            if (Vector3.Distance(desti, allCameras[0].transform.position) < closeToDest)
            {
                Debug.Log("TotalNoOfLine: "+lineList.Count);
                foreach(var u in lineList)
                {
                    Destroy(u);
                }
                //Destroy(abc);
                CancelInvoke();
            }

        }
    }
    /// <summary>
    /// Set start and end point
    /// </summary>
    public async void setStartEnd()
    {
        startDropdown.options.Clear();
        endDropdown.options.Clear();
        await createAnchors.GetMainAchInfo();
        var a = Resources.FindObjectsOfTypeAll<CreateAnchors>();
        var s = a[0];
        startDropdown.AddOptions(s.m_DropOptions);
        endDropdown.AddOptions(s.m_DropOptions);

    }
    /// <summary>
    /// Get all the active anchor
    /// </summary>
    public async Task testActive()
    {
        var s = FindObjectOfType<SpawnAnchor>();
        var zz = s.connectedActiveAnchors;
        //zz.ToList().ForEach(m => connectedActiveAnchors.Add(m.Key, m.Value));
        foreach (var c in zz.ToList())
        {
            if (!connectedActiveAnchors.ContainsKey(c.Key))
            {
                connectedActiveAnchors.Add(c.Key, c.Value);
            }
        }
        //connectedActiveAnchors = zz;
        Debug.Log(connectedActiveAnchors.Count);
        foreach (KeyValuePair<string, GameObject> i in connectedActiveAnchors)
        {
            Debug.Log("Active: " + i.Key);
            var z = await database.FindConnectedByName(i.Key);
            foreach (var CA in z)
            {
                var formatString = CA.Name + "," + CA.ConnectSpatialAnchor;
                if (!edgesDict.ContainsKey(formatString))
                {
                    edgesDict.Add(formatString, CA.Distance);
                }
                var reversedFormatString = CA.ConnectSpatialAnchor + "," + CA.Name;
                if (!edgesDict.ContainsKey(reversedFormatString))
                {
                    edgesDict.Add(reversedFormatString, CA.Distance);
                }

            }
        }
    }
    /// <summary>
    /// Create edges for all the anchors use for fsp
    /// </summary>
    public async Task CreateEdge()
    {
        await testActive();
        if (VerticeDetail == null)
        {
            VerticeDetail = new Dictionary<string, Dictionary<string, double>>();
        }
        foreach (KeyValuePair<string, double> detail in edgesDict)
        {
            string[] splitNodeName = detail.Key.Split(',');
            string node1 = splitNodeName[0];
            string node2 = splitNodeName[1];
            if (!VerticeDetail.ContainsKey(node1))
            {
                Dictionary<string, double> edgeList = new Dictionary<string, double>();
                edgeList.Add(node2, detail.Value);
                VerticeDetail.Add(node1, edgeList);
            }
            else if (!VerticeDetail[node1].ContainsKey(node2))
            {
                VerticeDetail[node1].Add(node2, detail.Value);
            }
            if (!VerticeDetail.ContainsKey(node2))
            {
                Dictionary<string, double> edgeList = new Dictionary<string, double>();
                edgeList.Add(node1, detail.Value);
                VerticeDetail.Add(node2, edgeList);
            }
            else if (!VerticeDetail[node2].ContainsKey(node1))
            {
                VerticeDetail[node2].Add(node1, detail.Value);
            }
        }
        //foreach (KeyValuePair<string, Dictionary<string, double>> a in VerticeDetail)
        //{
        //    Debug.Log("check key");
        //    Debug.Log(a.Key);
        //    foreach (KeyValuePair<string, double> b in a.Value)
        //    {
        //        Debug.Log("check value");
        //        Debug.Log(b.Key + ": " + b.Value);

        //    }
        //}
    }
    /// <summary>
    /// DSP calculation
    /// </summary>
    public async void FSP()
    {
        await CreateEdge();
        string origin = startDropdown.options[startDropdown.value].text;
        string dest = endDropdown.options[endDropdown.value].text;
        desti = connectedActiveAnchors[dest].transform.position;
        InvokeRepeating("UpdateLoc", (float)1, (float)1);
        Debug.Log("HELLLLOOO+ = " + desti);
        LinkedList<string> anchorList = new LinkedList<string>();
        List<string> idList = new List<string>();
        List<string> vertexSet = new List<string>();
        List<double> dist = new List<double>();
        List<string> prev = new List<string>();
        Debug.Log("sTarting" + VerticeDetail.Count);
        foreach (KeyValuePair<string, Dictionary<string, double>> node in VerticeDetail)
        {
            if (node.Key == origin)
            {
                dist.Add(0);
            }
            else
            {
                dist.Add(double.PositiveInfinity);
            }
            prev.Add("");
            idList.Add(node.Key);
            vertexSet.Add(node.Key);
        }
        Debug.Log(vertexSet.Count);
        while (vertexSet.Count > 0)
        {
            double minDis = double.PositiveInfinity;
            var u = "";
            Debug.Log("zzz");

            foreach (string vertex in vertexSet)
            {
                int vertexID = idList.IndexOf(vertex);
                //Debug.Log(vertex + " :" + vertexID);
                //Debug.Log(1 < double.PositiveInfinity);
                if (dist[vertexID] < minDis)
                {
                    minDis = dist[vertexID];
                    u = idList[vertexID];
                }
            }
            Debug.Log("u " + u);
            vertexSet.Remove(u);

            if (u == dest)
            {
                break;
            }
            foreach (KeyValuePair<string, double> d in VerticeDetail[u])
            {
                double alt = minDis + d.Value;
                int id = idList.IndexOf(d.Key);
                if (alt < dist[id])
                {
                    dist[id] = alt;
                    prev[id] = u;
                }
            }
        }
        int destId = idList.IndexOf(dest);
        var cand = dest;
        //Debug.Log("cand: " + cand + destId);

        if (prev[destId] != "" || cand == origin)
        {
            Debug.Log(prev[destId] + "-" + cand);
            while (cand != "")
            {
                Debug.Log("newcand: " + cand);
                anchorList.AddFirst(cand);
                //anchorNameList.Insert(0, cand);
                destId = idList.IndexOf(cand);
                cand = prev[destId];
            }
        }

        Debug.Log("Found shortest path.");
        string[] shortestPath = new string[anchorList.Count];
        anchorList.CopyTo(shortestPath, 0);
        foreach (var q in connectedActiveAnchors)
        {
            Debug.Log("CCCCCCCCCCCC" + q);
        }
        for (int i =0; i<shortestPath.Length-1;i++)
        {
            var x = Instantiate(linePrefab);
            //x.name = "line";
            var lineRen = x.transform.GetChild(0).GetComponent<LineRenderer>();
            lineRen.positionCount = 2;
            lineRen.SetPosition(0, connectedActiveAnchors[shortestPath[i]].transform.position);
            
            lineRen.SetPosition(1, connectedActiveAnchors[shortestPath[i+1]].transform.position);
            Debug.Log("Nmae: " + shortestPath[i] + connectedActiveAnchors[shortestPath[i]].gameObject.transform.position);
            lineList.Add(x);
        }
        Debug.Log("End");
    }

    public async void addinMain()
    {
        var s = FindObjectOfType<AnchorManager>();

        foreach (KeyValuePair<string, AnchorPosition> aA in s.activeAnchors)
        {
            var z = await database.FindTrackedObjectBySpId(aA.Key);
            EmptyObj = new GameObject("holder");
            EmptyObj.transform.position = aA.Value.transform.position;

            if (z.Name!= "Ref")
            { 
                if(!connectedActiveAnchors.ContainsKey(z.Name))
                {
                    Debug.Log(connectedActiveAnchors[z.Name]);
                    connectedActiveAnchors[z.Name] = EmptyObj;
                }
            }

        }
    }
}
