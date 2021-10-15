// <copyright file="JobProfilePage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI.Extension;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile.UI.FunctionalTests.Pages
{
    internal class JobProfilePage
    {
        public JobProfilePage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The job profile cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public JobProfilePage NavigagteToJobProfilePage(string profile)
        {
            var baseUrl = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            this.Context.GetWebDriver().Url = baseUrl + profile;
            return this;
        }
    }
}
