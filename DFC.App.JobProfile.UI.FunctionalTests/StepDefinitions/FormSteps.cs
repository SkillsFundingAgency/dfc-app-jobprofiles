// <copyright file="FormSteps.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class FormSteps
    {
        public FormSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [When(@"I search for (.*) under the JP search feature")]
        public void WhenISearchForUnderTheJPSearchFeature(string searchTerm)
        {
            var searchField = this.Context.GetWebDriver().FindElement(By.ClassName("search-input"));
            this.Context.GetHelperLibrary<AppSettings>().FormHelper.EnterText(searchField, searchTerm);
            this.Context.GetWebDriver().FindElement(By.ClassName("submit")).Click();
        }

        [When(@"I enter the feedback (.*)")]
        public void WhenIEnterFeedback(string feedback)
        {
            var feedbackField = this.Context.GetWebDriver().FindElement(By.Id("t71675538"));
            this.Context.GetHelperLibrary<AppSettings>().FormHelper.EnterText(feedbackField, feedback);
            this.Context.GetWebDriver().FindElement(By.Id("cmdGo")).Click();
        }
    }
}
