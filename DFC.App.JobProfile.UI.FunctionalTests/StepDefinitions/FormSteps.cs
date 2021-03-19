// <copyright file="FormSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    }
}
