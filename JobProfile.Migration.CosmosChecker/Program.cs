using System.Threading.Tasks;

namespace JobProfile.Migration.CosmosChecker
{
    public static class Program
    {
        private static readonly CosmosDbComparator cosmosComparator = new CosmosDbComparator();
        private static readonly AzureSearchComparator azureIndexComparator = new AzureSearchComparator();

        static async Task Main(string[] args)
        {
            await cosmosComparator.CompareCosmosDb();
            //await azureIndexComparator.CompareAzureCache();
        }
    }
}
