// File: SwagLabsTests.cs

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumNUnitDemo;

[TestFixture] // NUnit's attribute for a test class
public class SwagLabsTests
{
    private IWebDriver _driver;
    private const string BaseUrl = "https://www.saucedemo.com";

    [SetUp] // NUnit's attribute for a method that runs BEFORE each test
    public void SetupTest()
    {
        // Automatically downloads and configures the correct ChromeDriver
        new DriverManager().SetUpDriver(new ChromeConfig());

        _driver = new ChromeDriver();
        _driver.Manage().Window.Maximize();

        // Use implicit waits to give elements time to appear
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [Test] // This is Test Case 1
    [Order(1)] // Ensures tests run in a specific order if needed
    public void Test01_SuccessfulLogin()
    {
        // 1. Navigate to the login page
        _driver.Navigate().GoToUrl(BaseUrl);

        // 2. Enter credentials
        _driver.FindElement(By.Id("user-name")).SendKeys("standard_user");
        _driver.FindElement(By.Id("password")).SendKeys("secret_sauce");

        // 3. Click login
        _driver.FindElement(By.Id("login-button")).Click();

        // 4. Assert: Verify we are on the inventory page
        var inventoryPageTitle = _driver.FindElement(By.ClassName("title")).Text;
        Assert.That(inventoryPageTitle, Is.EqualTo("Products"), "Login failed or landed on the wrong page.");
    }

    [Test] // This is Test Case 2
    [Order(2)]
    public void Test02_LockedOutUser()
    {
        // 1. Navigate to the login page
        _driver.Navigate().GoToUrl(BaseUrl);

        // 2. Enter locked-out user credentials
        _driver.FindElement(By.Id("user-name")).SendKeys("locked_out_user");
        _driver.FindElement(By.Id("password")).SendKeys("secret_sauce");

        // 3. Click login
        _driver.FindElement(By.Id("login-button")).Click();

        // 4. Assert: Verify the error message is correct
        var errorMessage = _driver.FindElement(By.CssSelector("h3[data-test='error']")).Text;
        Assert.That(errorMessage, Does.Contain("Sorry, this user has been locked out."), "Error message was incorrect or not found.");
    }

    [Test] // This is Test Case 3
    [Order(3)]
    public void Test03_AddItemToCart()
    {
        // 1. Log in first (this test depends on being logged in)
        // For a real project, you'd make a reusable Login method.
        _driver.Navigate().GoToUrl(BaseUrl);
        _driver.FindElement(By.Id("user-name")).SendKeys("standard_user");
        _driver.FindElement(By.Id("password")).SendKeys("secret_sauce");
        _driver.FindElement(By.Id("login-button")).Click();

        // 2. Add an item (Sauce Labs Backpack) to the cart
        _driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack")).Click();

        // 3. Assert: Verify the cart badge shows "1"
        var cartBadge = _driver.FindElement(By.ClassName("shopping_cart_badge")).Text;
        Assert.That(cartBadge, Is.EqualTo("1"), "The cart badge count is incorrect.");
    }

    [TearDown] // NUnit's attribute for a method that runs AFTER each test
    public void TeardownTest()
    {
        // Always close the browser
        if (_driver != null)
        {
            _driver.Quit();
        }
    }
}