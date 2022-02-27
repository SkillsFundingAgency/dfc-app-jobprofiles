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

        /* Data Table statement "var data = table.CreateSet<Poco>()", did not extract data from data table , hence all the code below */
        [Then(@"the Administration Job category is present on the page")]
        public void ThenTheAdministrationJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Administration", _exploreCareersPage.GetJobCategoryList()[0].Text, "Job category list is out of sync");
        }

        [Then(@"the Animal care Job category is present on the page")]
        public void ThenTheAnimalCareJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Animal care", _exploreCareersPage.GetJobCategoryList()[1].Text, "Job category list is out of sync");
        }

        [Then(@"the Beauty and wellbeing Job category is present on the page")]
        public void ThenTheBeautyAndWellbeingJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Beauty and wellbeing", _exploreCareersPage.GetJobCategoryList()[2].Text, "Job category list is out of sync");
        }

        [Then(@"the Business and finance Job category is present on the page")]
        public void ThenTheBusinessAndFinanceJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Business and finance", _exploreCareersPage.GetJobCategoryList()[3].Text, "Job category list is out of sync");
        }

        [Then(@"the Computing, technology and digital Job category is present on the page")]
        public void ThenTheComputingTechnologyAndDigitalJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Computing, technology and digital", _exploreCareersPage.GetJobCategoryList()[4].Text, "Job category list is out of sync");
        }

        [Then(@"the Construction and trades Job category is present on the page")]
        public void ThenTheConstructionAndTradesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Construction and trades", _exploreCareersPage.GetJobCategoryList()[5].Text, "Job category list is out of sync");
        }

        [Then(@"the Creative and media Job category is present on the page")]
        public void ThenTheCreativeAndMediaJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Creative and media", _exploreCareersPage.GetJobCategoryList()[6].Text, "Job category list is out of sync");
        }

        [Then(@"the Delivery and storage Job category is present on the page")]
        public void ThenTheDeliveryAndStorageJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Delivery and storage", _exploreCareersPage.GetJobCategoryList()[7].Text, "Job category list is out of sync");
        }

        [Then(@"the Emergency and uniform services Job category is present on the page")]
        public void ThenTheEmergencyAndUniformServicesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Emergency and uniform services", _exploreCareersPage.GetJobCategoryList()[8].Text, "Job category list is out of sync");
        }

        [Then(@"the Engineering and maintenance Job category is present on the page")]
        public void ThenTheEngineeringAndMaintenanceJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Engineering and maintenance", _exploreCareersPage.GetJobCategoryList()[9].Text, "Job category list is out of sync");
        }

        [Then(@"the Environment and land Government services Job category is present on the page")]
        public void ThenTheEnvironmentAndLandGovernmentServicesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Environment and land", _exploreCareersPage.GetJobCategoryList()[10].Text, "Job category list is out of sync");
        }

        [Then(@"the Government services Job category is present on the page")]
        public void ThenTheGovernmentServicesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Government services", _exploreCareersPage.GetJobCategoryList()[11].Text, "Job category list is out of sync");
        }

        [Then(@"the Healthcare Job category is present on the page")]
        public void ThenTheHealthcareJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Healthcare", _exploreCareersPage.GetJobCategoryList()[12].Text, "Job category list is out of sync");
        }

        [Then(@"the Home services Job category is present on the page")]
        public void ThenTheHomeServicesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Home services", _exploreCareersPage.GetJobCategoryList()[13].Text, "Job category list is out of sync");
        }

        [Then(@"the Hospitality and food Job category is present on the page")]
        public void ThenTheHospitalityAndFoodJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Hospitality and food", _exploreCareersPage.GetJobCategoryList()[14].Text, "Job category list is out of sync");
        }

        [Then(@"the Law and legal Job category is present on the page")]
        public void ThenTheLawAndLegalJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Law and legal", _exploreCareersPage.GetJobCategoryList()[15].Text, "Job category list is out of sync");
        }

        [Then(@"the Managerial Job category is present on the page")]
        public void ThenTheManagerialJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Managerial", _exploreCareersPage.GetJobCategoryList()[16].Text, "Job category list is out of sync");
        }

        [Then(@"the Manufacturing Job category is present on the page")]
        public void ThenTheManufacturingJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Manufacturing", _exploreCareersPage.GetJobCategoryList()[17].Text, "Job category list is out of sync");
        }

        [Then(@"the Retail and sales Job category is present on the page")]
        public void ThenTheRetailAndSalesJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Retail and sales", _exploreCareersPage.GetJobCategoryList()[18].Text, "Job category list is out of sync");
        }

        [Then(@"the Science and research Job category is present on the page")]
        public void ThenTheScienceAndResearchJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Science and research", _exploreCareersPage.GetJobCategoryList()[19].Text, "Job category list is out of sync");
        }

        [Then(@"the Social care Job category is present on the page")]
        public void ThenTheSocialCareJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Social care", _exploreCareersPage.GetJobCategoryList()[20].Text, "Job category list is out of sync");
        }

        [Then(@"the Sports and leisure Job category is present on the page")]
        public void ThenTheSportsAndLeisureJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Sports and leisure", _exploreCareersPage.GetJobCategoryList()[21].Text, "Job category list is out of sync");
        }

        [Then(@"the Teaching and education Job category is present on the page")]
        public void ThenTheTeachingAndEducationJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Teaching and education", _exploreCareersPage.GetJobCategoryList()[22].Text, "Job category list is out of sync");
        }

        [Then(@"the Transport Job category is present on the page")]
        public void ThenTheTransportJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Transport", _exploreCareersPage.GetJobCategoryList()[23].Text, "Job category list is out of sync");
        }

        [Then(@"the Travel and tourism Job category is present on the page")]
        public void ThenTheTravelAndTourismJobCategoryIsPresentOnThePage()
        {
            Assert.AreEqual("Travel and tourism", _exploreCareersPage.GetJobCategoryList()[24].Text, "Job category list is out of sync");
        }

        [Given(@"I am at the ""(.*)"" page for (.*)")]
        public void GivenIAmAtThePageFor(string resourceOne, string resourceTwo)
        {
            _exploreCareersPage.NavigateToPage(resourceOne, resourceTwo);
        }

        [Given(@"I am at the ""(.*)"" web page for Administration")]
        public void GivenIAmAtTheWebPageForAdministration(string resource)
        {
            _exploreCareersPage.NavigateToPage(resource, "Administration");
        }

        [Then(@"the (.*) link is not present amongst the links under the Other job categories side section")]
        public void ThenTheLinkIsNotPresentAmongstTheLinksUnderTheOtherJobCategoriesSideSection(string jobCategoriesLink)
        {
            Assert.False(_jobCategoriesPage.CheckLinkPresentInOtherJobCategories(jobCategoriesLink), "The link is present but should not be.");
        }

    }
}
