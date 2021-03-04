namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    public class DocumentViewModel
    {
        public HeadViewModel Head { get; set; } = new HeadViewModel();

        public HeroViewModel HeroBanner { get; set; } = new HeroViewModel();

        public BodyViewModel Body { get; set; } = new BodyViewModel();
    }
}