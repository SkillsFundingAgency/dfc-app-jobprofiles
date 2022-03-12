using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace DFC.App.ExploreCareers.UI.FunctionalTests.Support
{
    public class Utilities
    {
        public static void ScrollIntoView(IWebDriver driver, IWebElement elementLocator)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", elementLocator);
        }

        public static void javascriptClick(IWebDriver driver, By locator)
        {
            IJavaScriptExecutor JS = (IJavaScriptExecutor)driver;
            JS.ExecuteScript("arguments[0].click();", locator);
        }
    }
}
