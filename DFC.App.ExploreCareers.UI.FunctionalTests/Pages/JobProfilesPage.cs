using DFC.TestAutomation.UI.Extension;
using DFC.App.ExploreCareers.UI.FunctionalTests.Support;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.Pages
{
    class JobProfilesPage
    {
        private ScenarioContext _scenarioContext;
        public JobProfilesPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement searchField => _scenarioContext.GetWebDriver().FindElement(By.Id("search-main"));
        IWebElement jobProlileHeading => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".govuk-grid-column-two-thirds h1"));

        public void EnterSearchTerm(string searchTerm)
        {
            Utilities.ScrollIntoView(this._scenarioContext.GetWebDriver(), _scenarioContext.GetWebDriver().FindElement(By.Id("search-main")));
            searchField.SendKeys(searchTerm);
        }

        public void SelectFromAutosuggest(string autosuggested)
        {
            searchField.Click();
            _scenarioContext.GetWebDriver().FindElement(By.XPath("//div[@class='job-profile-search-content']//following-sibling::ul/li/div[contains(text(), '" + autosuggested + "')]")).Click();
        }

        public string GetSelectedSearchTerm()
        {
            return searchField.GetAttribute("value");
        }

        public void ClickEnterInSearchField()
        {
            searchField.SendKeys(Keys.Enter);
        }

        public string GetJobProfileHeading()
        {
            var jobProfileHeadingText = jobProlileHeading.Text.Trim();
            
             return jobProfileHeadingText;
        }
    }
}