using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;

namespace DesktopUITests
{
    public class DesktopTest
    {
        private WindowsDriver<WindowsElement> driver;
        private const string appiumServer = "http://127.0.0.1:4723/wd/hub";
        private const string ShornerUrl = "https://shorturl.nakov.repl.co/api";
        private const string appLocation = @"C:\Exam\Shorturlapp\ShortURL-DesktopClient.exe";
        private AppiumOptions options;

        [SetUp]
        public void OpenApplication()
        {

            this.options = new AppiumOptions() { PlatformName = "Windows" };
            options.AddAdditionalCapability("app", appLocation);
            this.driver = new WindowsDriver<WindowsElement>(new Uri(appiumServer), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void ShutDownApp()
        {
            this.driver.Quit();
        }

        [Test]
        public void Test_ListAllUrls_AddUrl_Invalid()
        {
           var urlField = driver.FindElementByAccessibilityId("textBoxApiUrl");
            urlField.Clear();
            urlField.SendKeys(ShornerUrl);

            var buttonConnect = driver.FindElementByAccessibilityId("buttonConnect");
            buttonConnect.Click();

            var buttonAdd = driver.FindElementByAccessibilityId("buttonAdd");
            buttonAdd.Click();

            var newUrl = "https://selenium.dev" + DateTime.Now.Ticks.ToString();
            var newshort = "selenium.dev" + DateTime.Now.Ticks.ToString();

            var textBoxUrl = driver.FindElementByAccessibilityId("textBoxURL");
            textBoxUrl.Clear();
            textBoxUrl.SendKeys(newUrl);

            var shortText = driver.FindElementByAccessibilityId("textBoxCode");
            shortText.Clear();
            shortText.SendKeys(newshort);

            var createButton = driver.FindElementByAccessibilityId("buttonCreate");

            //var statusBox = driver.FindElementByAccessibilityId("status box"); //ListViewSubItem

            Thread.Sleep(15000);

            //var closeButton driver.FindElementsByXPath()

            var allUrls = driver.FindElementsByAccessibilityId("ListViewSubItem");

            foreach (var addedUrl in allUrls)
            {
                if (addedUrl.Text.Contains("https://selenium.dev"))
                {
                    Assert.That(addedUrl.Text, Is.EqualTo(newshort));
                    break;
                }
            }
        }
    }
}