﻿using DFC.App.ExploreCareers.UI.FunctionalTests.Support;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TechTalk.SpecFlow;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.Pages
{
    class SearchResultsPage
    {
        private ScenarioContext _scenarioContext;
        public SearchResultsPage(ScenarioContext context)
        {
            _scenarioContext = context;
        }

        IWebElement searchField => _scenarioContext.GetWebDriver().FindElement(By.Id("search-main"));
        IWebElement searchButton => _scenarioContext.GetWebDriver().FindElement(By.ClassName("button"));
        IWebElement resultsCount => _scenarioContext.GetWebDriver().FindElement(By.Id("result-count"));
        IWebElement textDidYouMean => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".search-dym > span"));
        IWebElement nextPaginator => _scenarioContext.GetWebDriver().FindElement(By.ClassName("dfc-code-search-nextlink"));
        IWebElement searchResultsForText => _scenarioContext.GetWebDriver().FindElement(By.CssSelector(".search-input.ui-front > h1"));
        IWebElement footer => _scenarioContext.GetWebDriver().FindElement(By.ClassName("govuk-footer"));

        public void SelectFromAutosuggest(string autosuggested)
        {
            searchField.Click();
            _scenarioContext.GetWebDriver().FindElement(By.XPath("//h1//following-sibling::ul/li/div[contains(text(), '" + autosuggested + "')]")).Click();
        }

        public string GetSelectedSearchTerm()
        {
            return searchField.GetAttribute("value");
        }

        public void ClickSearchButton()
        {
            searchButton.Click();
        }

        public string GetZeroResultsMsg()
        {
            return resultsCount.Text.Trim();
        }

        public string GetDidYouMean()
        {
            return textDidYouMean.Text.Trim();
        }

        public void ClearSearchField()
        {
            searchField.Clear();
        }

        public string GetUrl()
        {
            return _scenarioContext.GetWebDriver().Url;
        }

        public int GetNumberOfSearchResults()
        {
            int searchCount = int.Parse(resultsCount.Text.Replace("results found", "").Trim(), new CultureInfo("en-au"));
            return searchCount;
        }

        public decimal NumberOfSeachResultPages()
        {
            decimal ofSearchCount = GetNumberOfSearchResults();
            decimal searchCountGroups = (ofSearchCount / 10);
            decimal ofSearchCountGroups = Math.Ceiling(searchCountGroups) - 1;

            return ofSearchCountGroups;
        }

        public bool Paginator()
        {
            decimal numberOfSeachResultPages = NumberOfSeachResultPages();

            while (numberOfSeachResultPages > 0)
            {
                nextPaginator.Click();
                numberOfSeachResultPages--;
            }

            Utilities.ScrollIntoView(_scenarioContext.GetWebDriver(), footer);

            bool isPresent = true;

            try
            {
                _scenarioContext.GetWebDriver().FindElement(By.ClassName("dfc-code-search-nextlink"));
            }
            catch (NoSuchElementException)
            {
                isPresent = false;
            }

            return isPresent;
        }

        public string GetSearchResultsForText()
        {
            return searchResultsForText.Text.Trim();
        }
    }
}
