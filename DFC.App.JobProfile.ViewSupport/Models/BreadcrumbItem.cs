namespace DFC.App.JobProfile.ViewSupport.Models
{
    public class BreadcrumbItem
    {
        public BreadcrumbItem(string route, string title)
        {
            Route = route;
            Title = title;
        }

        public string Route { get; }

        public string Title { get; }
    }
}
