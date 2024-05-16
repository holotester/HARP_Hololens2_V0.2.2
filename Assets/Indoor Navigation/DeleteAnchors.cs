using Microsoft.MixedReality.Toolkit.UI;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using MRTK.Tutorials.AzureCloudServices.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DeleteAnchors : MonoBehaviour
{
    public Database DataBase => database;
    [SerializeField]
    private Database database;
    [SerializeField]
    private AnchorManager sceneController1;
    [Header("UI Elements")]
    [SerializeField]
    private TMP_Dropdown allAnchorDropdown;
    [SerializeField]
    private GameObject dd, but1, but2, comp;
    [SerializeField]
    private SpawnAnchor spawnAnchor;

    //public Camera mycam;

    private TrackedObject trackedObject;
    private NearbyObject trackedNBObject;
    private int selectedType;
    private List<string> main_Anchor = new List<string>();
    public List<string> m_DropOptions = new List<string>();
    public List<string> deletedAn = new List<string>();

    public async void Awake()
    {
        Debug.Log("Awak");
        database.Awake();
        allAnchorDropdown.AddOptions(m_DropOptions);
    }
    /// <summary>
    /// Reset dropdown after deleting
    /// </summary>
    public async void ResetDD()
    {
        database.Awake();
        allAnchorDropdown.options.Clear();
        m_DropOptions.Clear();
        m_DropOptions.Add("Select Anchor");
        //List<string> m_DropOptions = new List<string> { "Home", "Office" };
        await GetMainAchInfo();
        Debug.Log("res:" + m_DropOptions.Count);
        allAnchorDropdown.AddOptions(m_DropOptions);

        allAnchorDropdown.value = 0;
        dd.SetActive(true);
        but1.SetActive(true);
        but2.SetActive(true);
        comp.SetActive(false);

    }
    /// <summary>
    /// Get the spatial anchor details
    /// </summary>
    public async Task GetMainAchInfo()
    {
        //Debug.Log(deletedAn[0]);
        var z = await database.GetAllTrackedObjects();
        foreach (var t in z)
        {
            if (!m_DropOptions.Contains(t.Name) && !deletedAn.Contains(t.Name))
            {
                m_DropOptions.Add(t.Name);
                main_Anchor.Add(t.Name);
            }

        }
        var w = await database.GetAllTrackedNBObjects();
        foreach (var t in w)
        {
            if (!m_DropOptions.Contains(t.Name) && !deletedAn.Contains(t.Name))
            {
                m_DropOptions.Add(t.Name);
            }

        }

        //Debug.Log(m_DropOptions.Count);
    }
    /// <summary>
    /// Remove anchor that is already spawned
    /// </summary>
    public void completedPage()
    {
        dd.SetActive(false);
        but1.SetActive(false);
        but2.SetActive(false);
        comp.SetActive(true);
    }
    /// <summary>
    /// Delete the selected anchor
    /// </summary>
    public async void deleteSelected()
    {
        if (allAnchorDropdown.value != 0)
        {
            if (main_Anchor.Contains(allAnchorDropdown.options[allAnchorDropdown.value].text))
            {
                var ddd = await database.FindTrackedObjectByName2(allAnchorDropdown.options[allAnchorDropdown.value].text);
 
                await database.DeleteMainAnchor(ddd[0]);
                Debug.Log("deleted: " + allAnchorDropdown.options[allAnchorDropdown.value].text);

                try
                {
                    var s = FindObjectOfType<AnchorManager>();
                    var zz = s.activeAnchors;
                    Debug.Log(zz.Count);
                    Debug.Log("in:" + zz[ddd[0].SpatialAnchorId].gameObject);
                    Destroy(zz[ddd[0].SpatialAnchorId].gameObject);
                    zz.Remove(ddd[0].SpatialAnchorId);
                }
                catch { }
                
                //allAnchorDropdown.options.RemoveAt(allAnchorDropdown.value);
            }
            else
            {
                
                var ddd = await database.FindConnectedByName(allAnchorDropdown.options[allAnchorDropdown.value].text);
                //Debug.Log(ddd[0]);
                await database.DeleteConnectAnchor(ddd[0]);
                Debug.Log("deleted: " + allAnchorDropdown.options[allAnchorDropdown.value].text);
                try
                {
                    var s = FindObjectOfType<SpawnAnchor>();
                    var zz = s.connectedActiveAnchors;
                    Destroy(zz[allAnchorDropdown.options[allAnchorDropdown.value].text]);
                    zz.Remove(allAnchorDropdown.options[allAnchorDropdown.value].text);
                }
                catch { }
                

                //allAnchorDropdown.options.RemoveAt(allAnchorDropdown.value);
            }
            //Destroy(zz[allAnchorDropdown.options[allAnchorDropdown.value].text]);
            //zz.Remove(allAnchorDropdown.options[allAnchorDropdown.value].text);
            deletedAn.Add(allAnchorDropdown.options[allAnchorDropdown.value].text);
            allAnchorDropdown.options.RemoveAt(allAnchorDropdown.value);
            completedPage();
        }
    }

}
