// <copyright file="BasicSteps.cs" company="National Careers Service">
// Copyright (c) National Careers Service. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using DFC.App.JobProfile.Model;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace DFC.App.JobProfile.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class BasicSteps
    {
        public BasicSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }


        [When(@"I click the (.*) link")]
        public void WhenIClickTheLink(string linkText)
        {
            var link = this.Context.GetWebDriver().FindElement(By.LinkText(linkText));
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.Context.GetWebDriver();
            js.ExecuteScript("arguments[0].scrollIntoView();", link);

            if (!link.Displayed)
            {
                throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The {linkText} link is not displayed");
            }

            link.Click();
        }

        [When(@"I expand all accordion sections")]
        public void WhenIClickTheButton()
        {
            var elementAtDown = this.Context.GetWebDriver().FindElement(By.ClassName("govuk-accordion__open-all"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.Context.GetWebDriver();
            js.ExecuteScript("arguments[0].scrollIntoView();", elementAtDown);
            System.Threading.Thread.Sleep(1000);
            this.Context.GetWebDriver().FindElement(By.ClassName("govuk-accordion__open-all")).Click();
        }

        [When(@"I select course title (.*)")]
        public void WhenISelectCourseTitle(int courseToSelect)
        {
            int courseIndex = courseToSelect - 1;
            var listOfCourses = this.Context.GetWebDriver().FindElements(By.CssSelector(".dfc-code-jp-trainingCourse .opportunity-item .govuk-heading-s a"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.Context.GetWebDriver();
            js.ExecuteScript("arguments[0].scrollIntoView();", listOfCourses[courseIndex]);
            this.Context.Get<IObjectContext>().SetObject("courseTitle", listOfCourses[courseIndex].Text);
            listOfCourses[courseIndex].Click();
        }

        [When(@"I select apprenticeship title (.*)")]
        public void WhenISelectApprenticeshipTitle(int apprenticeshipToSelect)
        {
            int apprenticeshipIndex = apprenticeshipToSelect - 1;
            var listOfApprenticeships = this.Context.GetWebDriver().FindElements(By.CssSelector("#appGeneric .opportunity-item .govuk-heading-s a"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.Context.GetWebDriver();
            js.ExecuteScript("arguments[0].scrollIntoView();", listOfApprenticeships[apprenticeshipIndex]);
            this.Context.Get<IObjectContext>().SetObject("apprenticeshipTitle", listOfApprenticeships[apprenticeshipIndex].Text);
            listOfApprenticeships[apprenticeshipIndex].Click();
        }

        [When(@"I click on career title (.*)")]
        public void WhenIClickOnCareerTitle(int relatedCareer)
        {
            int careerIndex = relatedCareer - 1;
            var listOfCareers = this.Context.GetWebDriver().FindElements(By.CssSelector(".list-big li"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)this.Context.GetWebDriver();
            js.ExecuteScript("arguments[0].scrollIntoView();", listOfCareers[careerIndex]);
            this.Context.Get<IObjectContext>().SetObject("careerTitle", listOfCareers[careerIndex].Text);
            listOfCareers[careerIndex].Click();
        }
    }
}