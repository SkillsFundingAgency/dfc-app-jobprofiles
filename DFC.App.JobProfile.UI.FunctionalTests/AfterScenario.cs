// <copyright file="AfterScenario.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium.Remote;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile
{
    [Binding]
    public class AfterScenario
    {
        public AfterScenario(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException($"The scenario context is null. The {this.GetType().Name} class cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        [AfterScenario(Order = 0)]
        public async Task UpdateBrowserStack()
        {
            var browserHelper = this.Context.GetHelperLibrary<AppSettings>().BrowserHelper;
            if (browserHelper.IsExecutingInBrowserStack())
            {
                var sessionId = (this.Context.GetWebDriver() as RemoteWebDriver).SessionId.ToString();
                var browserStackHelper = this.Context.GetHelperLibrary<AppSettings>().BrowserStackHelper;

                if (this.Context.TestError != null)
                {
                    var errorMessage = this.Context.TestError.InnerException.Message;
                    await browserStackHelper.SetTestToFailedWithReason(sessionId, errorMessage).ConfigureAwait(false);
                }
                else
                {
                    await browserStackHelper.SetTestToPassed(sessionId).ConfigureAwait(false);
                }
            }
        }

        [AfterScenario(Order = 1)]
        public void DisposeWebDriver()
        {
            var webDriver = this.Context.GetWebDriver();
            webDriver?.Quit();
            webDriver?.Dispose();
        }
    }
}