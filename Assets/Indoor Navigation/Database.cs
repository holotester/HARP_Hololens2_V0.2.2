using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using MRTK.Tutorials.AzureCloudServices.Scripts.Domain;
using UnityEngine;
using UnityEngine.Events;


public class Database : MonoBehaviour
{
    public bool IsReady { get; private set; }

    [Header("Base Settings")]
    [SerializeField]
    private string connectionString = default;
    [SerializeField]
    private string projectName = "MyAzurePowerToolsProject";
    [Header("Table Settings")]
    [SerializeField]
    private string projectsTableName = "nearbyAnchors";
    [SerializeField]
    private string trackedObjectsTableName = "spatialAnchors";
    [SerializeField]
    private string partitionKey = "main";
    [SerializeField]
    private bool tryCreateTableOnStart = true;
    [Header("Blob Settings")]
    [SerializeField]
    private string blockBlobContainerName = "tracked-objects-thumbnails";
    [SerializeField]
    private bool tryCreateBlobContainerOnStart = true;
    [Header("Events")]
    [SerializeField]
    private UnityEvent onDataManagerReady = default;
    [SerializeField]
    private UnityEvent onDataManagerInitFailed = default;

    private CloudStorageAccount storageAccount;
    private CloudTableClient cloudTableClient;
    private CloudTable projectsTable;
    private CloudTable trackedObjectsTable;
    private CloudBlobClient blobClient;
    private CloudBlobContainer blobContainer;

    public async void Awake()
    {
        storageAccount = CloudStorageAccount.Parse(connectionString);
        cloudTableClient = storageAccount.CreateCloudTableClient();
        projectsTable = cloudTableClient.GetTableReference(projectsTableName);
        trackedObjectsTable = cloudTableClient.GetTableReference(trackedObjectsTableName);
        if (tryCreateTableOnStart)
        {
            try
            {
                if (await projectsTable.CreateIfNotExistsAsync())
                {
                    Debug.Log($"Created table {projectsTableName}.");
                }
                if (await trackedObjectsTable.CreateIfNotExistsAsync())
                {
                    Debug.Log($"Created table {trackedObjectsTableName}.");
                }
            }
            catch (StorageException ex)
            {
                Debug.LogError("Failed to connect with Azure Storage.\nIf you are running with the default storage emulator configuration, please make sure you have started the storage emulator.");
                Debug.LogException(ex);
                onDataManagerInitFailed?.Invoke();
            }
        }

        IsReady = true;
        onDataManagerReady?.Invoke();
    }

    

    /// <summary>
    /// Update the project changes back to the table store;
    /// </summary>
    public async Task<bool> UpdateProject(Project project)
    {
        var insertOrMergeOperation = TableOperation.InsertOrMerge(project);
        var result = await projectsTable.ExecuteAsync(insertOrMergeOperation);

        return result.Result != null;
    }

    /// <summary>
    /// Insert a new or update an TrackedObjectProject instance on the table storage.
    /// </summary>
    /// <param name="trackedObject">Instance to write or update.</param>
    /// <returns>Success result.</returns>
    public async Task<bool> UploadOrUpdate(TrackedObject trackedObject)
    {
        if (string.IsNullOrWhiteSpace(trackedObject.PartitionKey))
        {
            trackedObject.PartitionKey = partitionKey;
        }

        var insertOrMergeOperation = TableOperation.InsertOrMerge(trackedObject);
        var result = await trackedObjectsTable.ExecuteAsync(insertOrMergeOperation);

        return result.Result != null;
    }
    public async Task<bool> UpdateAdjacentTable(NearbyObject edge)
    {
        //Debug.Log("TScript "+edge.Yvalue);

        //edge.ConnectSpatialAnchor = "abcde";
        if (string.IsNullOrWhiteSpace(edge.PartitionKey))
        {
            edge.PartitionKey = partitionKey;
        }

        var insertOrMergeOperation = TableOperation.InsertOrMerge(edge);
        var result = await projectsTable.ExecuteAsync(insertOrMergeOperation);

        return result.Result != null;
    }

    /// <summary>
    /// Get all TrackedObjectProjects from the table.
    /// </summary>
    /// <returns>List of all TrackedObjectProjects from table.</returns>
    public async Task<List<TrackedObject>> GetAllTrackedObjects()
    {
        var query = new TableQuery<TrackedObject>();
        var segment = await trackedObjectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results;
    }


    public async Task<List<NearbyObject>> GetAllTrackedNBObjects()
    {
        var query = new TableQuery<NearbyObject>();
        var segment = await projectsTable.ExecuteQuerySegmentedAsync(query, null);
        return segment.Results;
    }

    /// <summary>
    /// Find a TrackedObjectProject by a given Id (partition key).
    /// </summary>
    /// <param name="id">Id/Partition Key to search by.</param>
    /// <returns>Found TrackedObjectProject, null if nothing is found.</returns>
    public async Task<TrackedObject> FindTrackedObjectById(string id)
    {
        var retrieveOperation = TableOperation.Retrieve<TrackedObject>("SpatialAnchorId", id);
        var result = await trackedObjectsTable.ExecuteAsync(retrieveOperation);
        var trackedObject = result.Result as TrackedObject;

        return trackedObject;
    }


    public async Task<List<NearbyObject>> FindNextConnected(string id)
    {
         var query = new TableQuery<NearbyObject>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id)));
        var segment = await projectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results;
    }

    public async Task<List<NearbyObject>> FindConnectedByName(string id)
    {
        var query = new TableQuery<NearbyObject>().Where(
           TableQuery.CombineFilters(
               TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, id),
               TableOperators.And,
               TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, id)));
        var segment = await projectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results;
    }

    public async Task<List<TrackedObject>> FindTrackedObjectByName2(string id)
    {
        var query = new TableQuery<TrackedObject>().Where(
           TableQuery.CombineFilters(
               TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, id),
               TableOperators.And,
               TableQuery.GenerateFilterCondition("Name", QueryComparisons.Equal, id)));
        var segment = await trackedObjectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results;
    }

    public async Task<TrackedObject> FindTrackedObjectByName(string trackedObjectName)
    {
        var query = new TableQuery<TrackedObject>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, trackedObjectName),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, trackedObjectName)));
        var segment = await trackedObjectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results.FirstOrDefault();
    }

    public async Task<TrackedObject> FindTrackedObjectBySpId(string id)
    {
        Debug.Log("DB Access");
        var query = new TableQuery<TrackedObject>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("SpatialAnchorId", QueryComparisons.Equal, id),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("SpatialAnchorId", QueryComparisons.Equal, id)));
        var segment = await trackedObjectsTable.ExecuteQuerySegmentedAsync(query, null);

        return segment.Results.FirstOrDefault();
    }



    public async Task DeleteMainAnchor(TrackedObject item)
    {
        TableOperation delteOperation = TableOperation.Delete(item);
        await trackedObjectsTable.ExecuteAsync(delteOperation);
    }

    public async Task DeleteConnectAnchor(NearbyObject item)
    {
        TableOperation delteOperation = TableOperation.Delete(item);
        await projectsTable.ExecuteAsync(delteOperation);
    }

}
