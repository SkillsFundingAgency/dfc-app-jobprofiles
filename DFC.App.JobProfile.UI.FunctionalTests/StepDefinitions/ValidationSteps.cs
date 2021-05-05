// <copyright file="ValidationSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
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
                case "job group: Elected officers and representatives":
                    locator = By.CssSelector("h1");
                    break;

                default:
                    locator = By.CssSelector("h1");
                    break;
            }

            this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(locator, pageName);
        }
    }
}