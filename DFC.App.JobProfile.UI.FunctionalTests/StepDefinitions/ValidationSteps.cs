// <copyright file="ValidationSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class ValidationSteps
    {
        public ValidationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Then(@"I am on the (.*) page")]
        public void ThenIAmOnThePage(string pageName)
        {
            By locator = null;
            switch (pageName.ToLower(CultureInfo.CurrentCulture))
            {
                //case "job group: Elected officers and representatives":
                //    locator = By.CssSelector("h1");
                //    break;

                case "feedback survey":
                    locator = By.Name("cmdGo");
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToBePresent(locator);
                    break;

                case "thanks":
                    locator = By.CssSelector("h2");
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(locator, pageName);
                    break;

                default:
                    locator = By.CssSelector("h1");
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(locator, pageName);
                    break;
            }

           // this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(locator, pageName);
        }

        [Then(@"The appropriate message is displayed")]
        public void ThenTheAppropriateMessageIsDisplayed()
        {
            if (!this.Context.GetHelperLibrary<AppSettings>().CommonActionHelper.ElementContainsText(By.ClassName("dfc-code-jp-novacancyText"), "We can't find any apprenticeship vacancies in England for a"))
            {
                throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The correct message is not displayed");
            }
        }

        [Then(@"the additional survey message is displayed for (.*) response")]
        public void SurveyMonkeyMessageIsDisplayed(string response)
        {

            switch (response.ToLower(CultureInfo.CurrentCulture))
            {
                case "no":
                    if (!this.Context.GetHelperLibrary<AppSettings>().CommonActionHelper.ElementContainsText(By.ClassName("job-profile-feedback-end-no"), "Click here if you'd like to let us know how we can improve the service."))
                    {
                        throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The correct message is not displayed");
                    }

                    break;
                case "yes":
                    if (!this.Context.GetHelperLibrary<AppSettings>().CommonActionHelper.ElementContainsText(By.ClassName("job-profile-feedback-end-yes"), "Thank you for your feedback."))
                    {
                        throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The correct message is not displayed");
                    }

                    break;
            }

        }
    }
}