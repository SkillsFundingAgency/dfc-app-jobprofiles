using DFC.App.ExploreCareers.UI.FunctionalTests.Support;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.Pages
{
    class JobCategoriesPage
    {
        private ScenarioContext _scenarioContext;
        public JobCategoriesPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        //IWebElement heading => _scenarioContext.GetWebDriver().FindElement(By.ClassName("heading-xlarge")); //PreProd
        IWebElement heading => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-heading-xl")); //SIT 
        IWebElement exploreCareersBreadcrumb => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-breadcrumbs li:nth-of-type(2) a"));

        public string jobProfileHeading { get; set; }

        public string GetHeadingText()
        {
            return heading.Text;
        }

        public bool GetJobCategorySideLinks(string jobCategory)
        {
            bool linkNotPresent = true;
            
            IList<IWebElement> allLinks = _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".govuk-list.font-xsmall > li > a"));

            for (int i = 0; i < allLinks.Count; i++)
            {
                if(allLinks[i].Text == jobCategory)
                {
                    linkNotPresent = false;
                    break;
                }
            }

            return linkNotPresent;
        }

        public IList<IWebElement> GetJobProfiles()
        {
            IList<IWebElement> jobProfiles = _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".dfc-code-search-jpTitle.govuk-link"));

            return jobProfiles;
        }

        public int VerifyJobProfileCount(IList<IWebElement> jobProfileElements)
        {
            IEnumerable<string> jobProfileElementsString = jobProfileElements.Select(i => i.Text);

            int multipleOccurences = jobProfileElementsString.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList().Count;

            return multipleOccurences;
        }

        public void ClickLinkInPosition(string linkPosition)
        {
            switch(linkPosition)
            {
                case "first":
                    jobProfileHeading = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(1) h2 a")).Text;
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(1) h2 a")).Click();
                    break;
                case "second":
                    jobProfileHeading = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(2) h2 a")).Text;
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(2) h2 a")).Click();
                    break;
                case "third":
                    jobProfileHeading = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(3) h2 a")).Text;
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(3) h2 a")).Click();
                    break;
                case "fourth":
                    jobProfileHeading = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(4) h2 a")).Text;
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(4) h2 a")).Click();
                    break;
                case "fifth":
                    jobProfileHeading = _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(5) h2 a")).Text;
                    _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-list.job-categories_items li:nth-of-type(5) h2 a")).Click();
                    break;
            }
        }

        public bool IsPagePaginated()
        {
            try
            {
                _scenarioContext.GetWebDriver().FindElement(By.ClassName("pagination-label"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ClickExploreCareersBreadcrumb()
        {
            exploreCareersBreadcrumb.Click();
        }
    }
}