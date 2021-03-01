namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    public class DocumentViewModel
    {
        public HeadViewModel Head { get; set; } = new HeadViewModel();

        public BreadcrumbViewModel Breadcrumb { get; set; } = new BreadcrumbViewModel();

        public HeroViewModel HeroBanner { get; set; } = new HeroViewModel();

        public BodyViewModel Body { get; set; } = new BodyViewModel();
    }
}