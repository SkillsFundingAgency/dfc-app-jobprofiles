using System.Collections.Generic;

namespace DFC.App.JobProfile.ViewSupport.Models
{
    public class Breadcrumb
    {
        public Breadcrumb(IReadOnlyCollection<BreadcrumbItem> items) => Items = items;

        public IReadOnlyCollection<BreadcrumbItem> Items { get; }
    }
}
