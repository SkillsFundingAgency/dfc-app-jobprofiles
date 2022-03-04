using DFC.App.ExploreCareers.Model;
using DFC.App.ExploreCareers.UI.FunctionalTests.StepDefinitions;
using DFC.App.ExploreCareers.UI.FunctionalTests.Support;
using DFC.TestAutomation.UI.Extension;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TechTalk.SpecFlow;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.Pages
{
    class ExploreCareersPage
    {
        private ScenarioContext _scenarioContext;
        public ExploreCareersPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement searchField => _scenarioContext.GetWebDriver().FindElement(By.Id("SearchTerm"));
        IWebElement searchButton => _scenarioContext.GetWebDriver().FindElement(By.ClassName("submit"));

        public void ClickLinkJobCategory(string jobCategory)
        {
            _scenarioContext.GetWebDriver().FindElement(By.XPath("//*[@class='govuk-link'][contains(text(),'" + jobCategory + "')]")).Click();
        }

        public void NavigateToPage(string resourceOne, string resourceTwo = null)
        {
            var endpoint = this._scenarioContext.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString().Replace("job-profiles/", "");

            switch (resourceOne)
            {
                case "Explore careers":
                    this._scenarioContext.GetWebDriver().Url = endpoint + "explore-careers";
                    break;
                case "Job profiles":
                    this._scenarioContext.GetWebDriver().Url = endpoint + "job-profiles/admin-assistant";
                    Utilities.ScrollIntoView(this._scenarioContext.GetWebDriver(), _scenarioContext.GetWebDriver().FindElement(By.Id("search-main")));
                    break;
                case "Search results":
                    this._scenarioContext.GetWebDriver().Url = endpoint + "search-results";
                    break;
                case "Job categories":
                    this._scenarioContext.GetWebDriver().Url = endpoint + "job-categories/" + ProcessResourceTwo(resourceTwo);
                    break;
            }
        }

        public string ProcessResourceTwo(string resourceTwo)
        {
            string secondResource = "";

            switch (resourceTwo)
            {
                case "Administration":
                    secondResource = "Administration";
                    break;
                case "Animal care":
                    secondResource = "animal-care";
                    break;
                case "Beauty and wellbeing":
                    secondResource = "beauty-and-wellbeing";
                    break;
                case "Business and finance":
                    secondResource = "business-and-finance";
                    break;
                case "Computing, technology and digital":
                    secondResource = "computing-technology-and-digital";
                    break;
                case "Construction and trades":
                    secondResource = "construction-and-trades";
                    break;
                case "Creative and media":
                    secondResource = "creative-and-media";
                    break;
                case "Delivery and storage":
                    secondResource = "delivery-and-storage";
                    break;
                case "Emergency and uniform services":
                    secondResource = "emergency-and-uniform-services";
                    break;
                case "Engineering and maintenance":
                    secondResource = "engineering-and-maintenance";
                    break;
                case "Environment and land":
                    secondResource = "environment-and-land";
                    break;
                case "Government services":
                    secondResource = "government-services";
                    break;
                case "Healthcare":
                    secondResource = "healthcare";
                    break;
                case "Home services":
                    secondResource = "home-services";
                    break;
                case "Hospitality and food":
                    secondResource = "hospitality-and-food";
                    break;
                case "Law and legal":
                    secondResource = "law-and-legal";
                    break;
                case "Managerial":
                    secondResource = "Managerial";
                    break;
                case "Manufacturing":
                    secondResource = "Manufacturing";
                    break;
                case "Retail and sales":
                    secondResource = "retail-and-sales";
                    break;
                case "Science and research":
                    secondResource = "science-and-research";
                    break;
                case "Social care":
                    secondResource = "social-care";
                    break;
                case "Sports and leisure":
                    secondResource = "sports-and-leisure";
                    break;
                case "Teaching and education":
                    secondResource = "teaching-and-education";
                    break;
                case "Transport":
                    secondResource = "Transport";
                    break;
                case "Travel and tourism":
                    secondResource = "travel-and-tourism";
                    break;
            }

            return secondResource;
        }

        public void EnterSearchTerm(string searchTerm)
        {
            ClearSearchField();
            searchField.SendKeys(searchTerm);
        }

        public void SelectFromAutosuggest(string autosuggested)
        {
            searchField.Click();
            _scenarioContext.GetWebDriver().FindElement(By.XPath("//div[@class='header-search-content']//following-sibling::ul/li/div[contains(text(), '" + autosuggested + "')]")).Click();
        }

        public string GetSelectedSearchTerm()
        {
            return searchField.GetAttribute("value");
        }

        public void ClickSearchButton()
        {
            searchButton.Click();
        }

        public void ClearSearchField()
        {
            searchField.Clear();
        }

        public string GetUrl()
        {
            return _scenarioContext.GetWebDriver().Url;
        }

        public void ClickEnterInSearchField()
        {
            searchField.Click();
            searchField.SendKeys(Keys.Enter);
        }

        public IList<IWebElement> GetJobCategoryList()
        {
            IList<IWebElement> allList = _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".govuk-list.homepage-jobcategories > li > a"));

            return allList;
        }

        public bool GetJobCategoryListII(IEnumerable<JobCategories> expectedJobCategories)
        {
            bool a_and_b_Checks = false;

            //A - Check.
            //Convert IEnumerable expected results to string
            string[] expected = expectedJobCategories.Select(p => p.jobCategory).ToArray();

            //Get actual list from the UI
            IList<IWebElement> actualJobCategoriesList = _scenarioContext.GetWebDriver().FindElements(By.CssSelector(".govuk-list.homepage-jobcategories > li > a"));

            //translate IWebElements above into a collection of strings so they can be compared
            IEnumerable<string> actual = actualJobCategoriesList.Select(i => i.Text);

            
            //determines, as bool, if items in 1 and 2 are present in the other
            var OptionsVerified = expected.All(d => actual.Contains(d));

            //B - Check.
            int noOfActualElements = actualJobCategoriesList.Count;
            bool optionsEqual = false;

            if (expected.Length == noOfActualElements)
            {
                optionsEqual = true;
            }

            if (OptionsVerified == true && optionsEqual == true)
            {
                a_and_b_Checks = true;
            }

            return a_and_b_Checks;
        }
    }
}
