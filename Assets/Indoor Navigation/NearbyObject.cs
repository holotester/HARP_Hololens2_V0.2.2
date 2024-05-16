using Microsoft.WindowsAzure.Storage.Table;

namespace MRTK.Tutorials.AzureCloudServices.Scripts.Domain
{
    public class NearbyObject : TableEntity
    {
        public string Name { get; set; }
        public string ConnectSpatialAnchor { get; set; }
        public double Xvalue { get; set; }
        public double Yvalue { get; set; }
        public double Zvalue { get; set; }
        public double Distance { get; set; }
        public double Angle { get; set; }

        public string Dir { get; set; }
        public int counter { get; set; }

        public NearbyObject() { }

        public NearbyObject(string name)
        {
            Name = name;
            RowKey = name;
        }
    }
}
