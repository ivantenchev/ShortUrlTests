using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading;

namespace ShortUrl_WebDriverTests
{
    public class WebDriverTests
    {
        private const string url = "https://shorturl.nakov.repl.co/";
        private WebDriver driver;

        [SetUp]
        public void StartBrowser()
        {
            this.driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Quit();
        }

        [Test]
        public void Test_CheckAndAssert_HomePageTitle()
        {
            driver.Navigate().GoToUrl(url);

            driver.FindElement(By.CssSelector("html")).Click();

            Assert.That(driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("URL Shortener"));

        }

        [Test]
        public void Test_ListShorts_AssertFirstLink()
        {
            driver.Navigate().GoToUrl(url);

            var shortUrlsButton = driver.FindElement(By.CssSelector("body > header > a:nth-child(3)"));
            shortUrlsButton.Click();

            var firstSite = driver.FindElement(By.XPath("/html/body/main/table/tbody/tr[1]/td[1]/a")).Text;
            var firstShort = driver.FindElement(By.CssSelector("body > main > table > tbody > tr:nth-child(1) > td:nth-child(2) > a")).Text;

            Assert.That(firstSite, Is.EqualTo("https://nakov.com"));
            Assert.That(firstShort, Is.EqualTo("http://shorturl.nakov.repl.co/go/nak"));

        }

        [Test] //Task 2
        public void Test_CreateShort_EntryValidData()
        {
            driver.Navigate().GoToUrl(url);

            var addButton = driver.FindElement(By.LinkText("Add URL"));
            addButton.Click();

            var urlField = "https://discovery.com" + DateTime.Now.Ticks;
            var shordCodeField = "discovery" + DateTime.Now.Ticks;

            driver.FindElement(By.Id("url")).SendKeys(urlField);
            driver.FindElement(By.Id("code")).SendKeys(shordCodeField);

            var createButton = driver.FindElement(By.CssSelector("body > main > form > table > tbody > tr:nth-child(3) > td > button"));
            createButton.Click();

            var pageTitle = driver.FindElement(By.CssSelector("body > main > h1")).Text;

            Assert.That(pageTitle, Is.EqualTo("Short URLs"));

            var allUrls = driver.FindElements(By.CssSelector("body > main > table"));
            var lasturl = allUrls.Last();

            var originalUrl = lasturl.FindElement(By.LinkText(urlField)).Text;

            Assert.That(originalUrl, Is.EqualTo(urlField));

        }

        [Test]
        public void Test_CreateShort_EntryInValidData()
        {
            driver.Navigate().GoToUrl(url);

            var addButton = driver.FindElement(By.LinkText("Add URL"));
            addButton.Click();

            var urlField = "discovery.com";

            driver.FindElement(By.Id("url")).SendKeys(urlField);

            var createButton = driver.FindElement(By.CssSelector("body > main > form > table > tbody > tr:nth-child(3) > td > button"));
            createButton.Click();

            var pageTitle = driver.FindElement(By.CssSelector("body > div")).Text;

            Assert.That(pageTitle, Is.EqualTo("Invalid URL!"));

        }

        [Test]
        public void Test_Visit_NonExistingURL()
        {
            const string expectedDivErrMSg = "Cannot navigate to given short URL";
            const string expectedHeadErrMSg = "Error: Cannot navigate to given short URL";
            const string expectedPErrMSg = "Invalid short URL code: invalid536524";

            //Arrange
            driver.Navigate().GoToUrl(url + "/go/invalid536524");

            var result = driver.FindElement(By.ClassName("err")).Text;

            Assert.That(result, Is.EqualTo("Cannot navigate to given short URL"));

            var errorMsgDiv = this.driver.FindElement(By.CssSelector("body > div")).Text;
            Assert.That(errorMsgDiv, Is.EqualTo(expectedDivErrMSg));

            var errorMsgHead = this.driver.FindElement(By.CssSelector("body > main > h1")).Text;

            Assert.That(errorMsgHead, Is.EqualTo(expectedHeadErrMSg));

            var errorMsgP = this.driver.FindElement(By.CssSelector("body > main > p")).Text;

            Assert.That(errorMsgP, Is.EqualTo(expectedPErrMSg));


        }
        [Test]
        public void Visit_URL_CheckCounterIncrease()
        {
            // Arrange
            driver.Navigate().GoToUrl(url + "/go/invalid536524");

            var shortURLs_button = driver.FindElement(By.LinkText("Short URLs"));
            shortURLs_button.Click();

            var firstVisitors = driver.FindElement(By.CssSelector("body > main > table > tbody > tr:nth-child(1) > td:nth-child(4)")).Text;
            var numberFirstVisitors = firstVisitors;
            var shortUrl = driver.FindElement(By.ClassName("shorturl"));
            shortUrl.Click();

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            Thread.Sleep(3000);
            var lastVisitors = driver.FindElement(By.CssSelector("body > main > table > tbody > tr:nth-child(1) > td:nth-child(4)")).Text;
            var numberLastVisitors = lastVisitors;

            Assert.That(int.Parse(numberFirstVisitors), Is.Not.EqualTo(int.Parse(numberLastVisitors)));

        }

    }
}



