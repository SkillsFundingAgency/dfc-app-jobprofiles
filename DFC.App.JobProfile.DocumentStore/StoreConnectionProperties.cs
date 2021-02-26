namespace DFC.App.JobProfile.DocumentStore
{
    public abstract class StoreConnectionProperties
    {
        public string AccessKey { get; set; }

        public string EndpointLocation { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public string PartitionKey { get; set; }
    }
}