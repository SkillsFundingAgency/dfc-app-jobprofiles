using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewSupport.Models
{
    [ExcludeFromCodeCoverage]
    public class Breadcrumb
    {
        public Breadcrumb(IReadOnlyCollection<BreadcrumbItem> items) => Items = items;

        public IReadOnlyCollection<BreadcrumbItem> Items { get; }
    }
}
