// <copyright file="NavigationSteps.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.App.JobProfile.UI.FunctionalTests.Pages;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class NavigationSteps
    {
        public NavigationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Given(@"I navigate to the (.*) profile")]
        public void GivenINavigateToTheMPProfile(string profile)
        {
            var jobProfilePage = new JobProfilePage(this.Context);
            jobProfilePage.NavigagteToJobProfilePage(profile);
        }
    }
}
