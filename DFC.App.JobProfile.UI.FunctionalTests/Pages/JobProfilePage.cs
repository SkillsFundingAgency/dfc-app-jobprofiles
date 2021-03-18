// <copyright file="JobProfilePage.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
            this.Context.GetWebDriver().Url = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            return this;
        }
    }
}
