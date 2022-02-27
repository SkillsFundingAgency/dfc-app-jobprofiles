using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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

        IWebElement heading => _scenarioContext.GetWebDriver().FindElement(By.ClassName("heading-xlarge"));
        IWebElement searchField => _scenarioContext.GetWebDriver().FindElement(By.Id("search-main"));

        public string GetHeadingText()
        {
            return heading.Text;
        }

        public bool CheckLinkPresentInOtherJobCategories(string link)
        {
            bool isPresent = true;

            try
            {
                _scenarioContext.GetWebDriver().FindElement(By.XPath("//ul[@class='govuk-list font-xsmall']/li/a[contains(text(), '" + link + "')]"));
            }
            catch (NoSuchElementException)
            {
                isPresent = false;
            }
            
            return isPresent;
        }
    }
}
