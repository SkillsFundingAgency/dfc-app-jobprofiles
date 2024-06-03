namespace DFC.App.JobProfile.Data.Configuration
{
    public class AVAPIServiceSettings
    {
        public string FAAEndPoint { get; set; }

        public string FAASubscriptionKey { get; set; }

        public int FAAPageSize { get; set; }

        public int FAAMaxPagesToTryPerMapping { get; set; }

        public string FAASortBy { get; set; }

        public int RequestTimeOutSeconds { get; set; }

        public string StandardsForHealthCheck { get; set; }
    }
}
