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
    }
}
