using DFC.App.ExploreCareers.UI.FunctionalTests.Pages;
using DFC.TestAutomation.UI.Extension;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    public sealed class ExploreCareersCUI
    {
        private ScenarioContext _scenarioContext;
        private readonly ExploreCareersPage _exploreCareersPage;
        private readonly JobCategoriesPage _jobCategoriesPage;
        private readonly JobProfilesPage _jobProfilesPage;
        private readonly SearchResultsPage _searchResultsPage;
        string _page;
        IEnumerable<JobCategories> jobCategories;

        public ExploreCareersCUI(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _exploreCareersPage = new ExploreCareersPage(scenarioContext);
            _jobCategoriesPage = new JobCategoriesPage(scenarioContext);
            _jobProfilesPage = new JobProfilesPage(scenarioContext);
            _searchResultsPage = new SearchResultsPage(scenarioContext);
        }

        [Given(@"I am at the ""(.*)"" page")]
        public void GivenIAmAtThePage(string resource)
        {
            _exploreCareersPage.NavigateToPage(resource, null);
        }

        [When(@"I click on the (.*) link")]
        public void WhenIClickOnTheAdministrationLink(string link)
        {
            _exploreCareersPage.ClickLinkJobCategory(link);
        }

        [Then(@"I am taken to the (.*) page")]
        public void ThenIAmTakenToThePage(string page)
        {
            Assert.AreEqual(page, _jobCategoriesPage.GetHeadingText(), "This is not the " + page + "page.");
        }

        [Given(@"I navigate to the (.*) page")]
        public void GivenINavigateToThePage(string page)
        {
            _page = page;
            _exploreCareersPage.NavigateToPage(page, null);
        }

        [When(@"I enter the search term (.*) in the search field")]
        public void GivenIEnterTheSearchTermInTheSearchField(string searchTerm)
        {
            switch(_page)
            {
                case "Explore careers":
                    _exploreCareersPage.EnterSearchTerm(searchTerm);
                    break;
                case "Job profiles":
                    _jobProfilesPage.EnterSearchTerm(searchTerm);
                    break;
                case "Search results":
                    _jobProfilesPage.EnterSearchTerm(searchTerm);
                    break;
            }
        }

        [When(@"I enter the non existent search term (.*) in the search field")]
        public void WhenIEnterTheNonExistentSearchTermInTheSearchField(string nonExistentSearchTerm)
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.EnterSearchTerm(nonExistentSearchTerm);
                    break;
                case "Job profiles":
                    _jobProfilesPage.EnterSearchTerm(nonExistentSearchTerm);
                    break;
                case "Search results":
                    _jobProfilesPage.EnterSearchTerm(nonExistentSearchTerm);
                    break;
            }
        }

        [Then(@"I am able to select (.*) from the resultant auto suggest")]
        public void ThenIAmAbleToSelectFromTheResultantAutoSuggest(string option)
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.SelectFromAutosuggest(option);
                    break;
                case "Job profiles":
                    _jobProfilesPage.SelectFromAutosuggest(option);
                    break;
                case "Search results":
                    _searchResultsPage.SelectFromAutosuggest(option);
                    break;
            }
        }

        [Then(@"(.*) is populated in the search field")]
        public void IsPopulatedInTheSearchField(string autosuggestedOption)
        {
            switch (_page)
            {
                case "Explore careers":
                    Assert.AreEqual(autosuggestedOption, _exploreCareersPage.GetSelectedSearchTerm(), "Selected option is not " + autosuggestedOption + ".");
                    break;
                case "Job profiles":
                    Assert.AreEqual(autosuggestedOption, _jobProfilesPage.GetSelectedSearchTerm(), "Selected option is not " + autosuggestedOption + ".");
                    break;
                case "Search results":
                    Assert.AreEqual(autosuggestedOption, _searchResultsPage.GetSelectedSearchTerm(), "Selected option is not " + autosuggestedOption + ".");
                    break;
            }
        }

        [Given(@"I enter the search term (.*) in the search field")]
        public void GivenIEnterTheSearchTermNurInTheSearchField(string searchTerm)
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.EnterSearchTerm(searchTerm);
                    break;
                case "Job profiles":
                    _jobProfilesPage.EnterSearchTerm(searchTerm);
                    break;
                case "Search results":
                    _jobProfilesPage.EnterSearchTerm(searchTerm);
                    break;
            }
        }

        [Given(@"the search field is empty")]
        public void GivenTheSearchFieldIsEmpty()
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.ClearSearchField();
                    break;
                case "Job profiles":
                    _exploreCareersPage.ClearSearchField();
                    break;
                case "Search results":
                    _searchResultsPage.ClearSearchField();
                    break;
            }
        }

        [When(@"I click the search button")]
        public void WhenIClickTheSearchButton()
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.ClickSearchButton();
                    break;
                case "Job profiles":
                    _exploreCareersPage.ClickSearchButton();
                    break;
                case "Search results":
                    _searchResultsPage.ClickSearchButton();
                    break;
            }
        }

        [Then(@"the page does not advance")]
        public void ThenThePageDoesNotAdvance()
        {
            switch (_page)
            {
                case "Explore careers":
                    Assert.IsTrue(_exploreCareersPage.GetUrl().Contains("/explore-careers"), "The page advanced unexpectedly");
                    break;
                case "Search results":
                    Assert.IsTrue(_searchResultsPage.GetUrl().Contains("/search-results/Search/"), "The page advanced unexpectedly");
                    break;
            }
        }

        [Then(@"I get the message ""(.*)"" in the search results page")]
        public void ThenIGetTheMessageInTheSearchResultsPage(string zeroResultsMsg)
        {
            Assert.AreEqual(zeroResultsMsg, _searchResultsPage.GetZeroResultsMsg(), "The search unexpectedly returned results");
        }

        [Then(@"I am taken to the search results page with the message Did you mean (.*) displayed")]
        public void ThenIAmTakenToTheSearchResultsPageWithTheMessageDidYouMeanDisplayed(string autoSuggestedSearchTerm)
        {
            Assert.AreEqual("Did you mean " + autoSuggestedSearchTerm, _searchResultsPage.GetDidYouMean(), "Unexpected auto suggesteed search term displayed");
        }

        [Then(@"the message ""(.*)"" displayed below it")]
        public void ThenTheMessageDisplayedBelowIt(string zeroResultsMsg)
        {
            Assert.AreEqual(zeroResultsMsg, _searchResultsPage.GetZeroResultsMsg(), "The search unexpectedly returned results");
        }

        [Then(@"the number of search results returned is commensurate with the number of search result pages")]
        public void ThenTheNumberOfSearchResultsReturnedIsCommensurateWithTheNumberOfSearchResultPages()
        {
            Assert.False(_searchResultsPage.Paginator(), "Next paginator is still unexpectedly displayed.");
        }

        [When(@"I press the Enter button instead of clicking search")]
        public void WhenIPressTheEnterButtonInsteadOfClickingSearch()
        {
            switch (_page)
            {
                case "Explore careers":
                    _exploreCareersPage.ClickEnterInSearchField();
                    break;
                case "Job profiles":
                    _jobProfilesPage.ClickEnterInSearchField();
                    break;
                case "Search results":
                    _searchResultsPage.ClickSearchButton();
                    break;
            }
        }

        [Then(@"the search results screen is displayed.")]
        public void ThenTheSearchResultsScreenIsDisplayed()
        {
            switch (_page)
            {
                case "Search results":
                    Assert.IsTrue(_searchResultsPage.GetZeroResultsMsg().Contains("results found"), "Pressing the enter key did not produce search results");
                    break;
            }

            Assert.AreEqual("Search results for", _searchResultsPage.GetSearchResultsForText(), "Pressing Enter did not advance to the search results page");
        }

        [Given(@"I am at the ""(.*)"" page for (.*)")]
        public void GivenIAmAtThePageFor(string resourceOne, string resourceTwo)
        {
            _exploreCareersPage.NavigateToPage(resourceOne, resourceTwo);
        }

        [Given(@"I am at the ""(.*)"" Administration web page")]
        public void GivenIAmAtTheAdministrationWebPage(string resource)
        {
            _exploreCareersPage.NavigateToPage(resource, "Administration");
        }

        [Given(@"I am at the ""(.*)"" web page for (.*)")]
        public void GivenIAmAtTheWebPageFor(string resource, string resourceTwo)
        {
            _exploreCareersPage.NavigateToPage(resource, resourceTwo);
        }

        [Then(@"the (.*) link is not present in amongst the links beneath the Other job categories side section")]
        public void ThenTheLinkIsNotPresentInAmongstTheLinksBeneathTheOtherJobCategoriesSideSection(string jobCategory)
        {
            Assert.True(_jobCategoriesPage.GetJobCategorySideLinks(jobCategory), "The " + jobCategory + " link is present, unexpectedly.");
        }

        [Given(@"I check the list displayed below against the list of Job categories displayed on the page")]
        public void GivenICheckTheListDisplayedBelowAgainstTheListOfJobCategoriesDisplayedOnThePage(Table table)
        {
            jobCategories = table.CreateSet<JobCategories>().ToList();
        }

        [Then(@"both lists are the same")]
        public void ThenBothListsAreTheSame()
        {
            Assert.IsTrue(_exploreCareersPage.VerifyJobCategoryList(jobCategories), "Expected and actual Job categories are not the same");
        }
    }

    public class JobCategories
    {
        public string jobCategory { get; set; }
    }
}
