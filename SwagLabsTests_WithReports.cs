using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumNUnitDemo.Pages;
using SeleniumNUnitDemo.Utilities;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumNUnitDemo;

/// <summary>
/// Test class with Extent Reports integration
/// Captures screenshots on test failure and logs test steps
/// Generates beautiful HTML reports with visual evidence
/// </summary>
[TestFixture]
public class SwagLabsTests_WithReports
{
    private IWebDriver _driver;
    private LoginPage _loginPage;
    private InventoryPage _inventoryPage;
    private CartPage _cartPage;
    private CheckoutPage _checkoutPage;
    private CheckoutOverviewPage _checkoutOverviewPage;
    private CheckoutCompletePage _checkoutCompletePage;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Initialize WebDriver Manager
        new DriverManager().SetUpDriver(new ChromeConfig());
        
        // Initialize Extent Reports
        ReportManager.InitializeReport();
        ReportManager.LogInfo("Test Suite Started - Extent Reports initialized");
    }

    [SetUp]
    public void Setup()
    {
        // Initialize Chrome driver with popup suppression
        _driver = ChromeDriverConfig.CreateChromeDriver();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        // Initialize page objects
        _loginPage = new LoginPage(_driver);
        _inventoryPage = new InventoryPage(_driver);
        _cartPage = new CartPage(_driver);
        _checkoutPage = new CheckoutPage(_driver);
        _checkoutOverviewPage = new CheckoutOverviewPage(_driver);
        _checkoutCompletePage = new CheckoutCompletePage(_driver);
    }

    [TearDown]
    public void TearDown()
    {
        // Take screenshot if test failed
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            ReportManager.LogInfo("Test Failed - Taking screenshot");
            string screenshotPath = _loginPage.TakeScreenshot($"Failure_{TestContext.CurrentContext.Test.Name}");
            
            if (!string.IsNullOrEmpty(screenshotPath))
            {
                ReportManager.LogFail($"Test Failed: {TestContext.CurrentContext.Result.Message}", screenshotPath);
            }
        }

        // Close driver
        _driver?.Quit();
        _driver?.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Flush and generate final report
        ReportManager.LogInfo("Test Suite Completed - Flushing reports");
        ReportManager.FlushReport();
        
        string reportPath = ReportManager.GetReportPath();
        Console.WriteLine($"\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  EXTENT REPORT GENERATED SUCCESSFULLY                          ║");
        Console.WriteLine($"║  Location: {reportPath.Substring(0, Math.Min(reportPath.Length, 55)).PadRight(55)} ║");
        Console.WriteLine($"╚════════════════════════════════════════════════════════════════╝\n");
    }

    #region Test Cases

    [Test]
    [Category("Login")]
    [Description("Verify successful login with valid credentials")]
    public void Test_LoginWithValidCredentials()
    {
        string testName = "Login with Valid Credentials";
        ReportManager.CreateTest(testName, "Verify user can login with valid credentials");

        try
        {
            ReportManager.LogInfo("Test Started: " + testName);
            ReportManager.LogInfo("Step 1: Navigate to login page");
            _loginPage.NavigateToLoginPage();

            ReportManager.LogInfo("Step 2: Verify login page is displayed");
            Assert.That(_loginPage.IsLoginPageLoaded(), Is.True, "Login page should be displayed");
            ReportManager.LogPass("Login page displayed successfully");

            ReportManager.LogInfo("Step 3: Enter username");
            _loginPage.EnterUsername("standard_user");
            ReportManager.LogPass("Username entered");

            ReportManager.LogInfo("Step 4: Enter password");
            _loginPage.EnterPassword("secret_sauce");
            ReportManager.LogPass("Password entered");

            ReportManager.LogInfo("Step 5: Click login button");
            _loginPage.ClickLoginButton();
            ReportManager.LogPass("Login button clicked");

            ReportManager.LogInfo("Step 6: Wait for inventory page to load");
            Thread.Sleep(2000);

            ReportManager.LogInfo("Step 7: Verify successful login - inventory page should display");
            Assert.That(_inventoryPage.IsInventoryPageLoaded(), Is.True, "Inventory page should be displayed after login");
            ReportManager.LogPass("Login successful - Inventory page displayed");

            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test Failed with Exception: {ex.Message}");
            throw;
        }
    }

    [Test]
    [Category("Login")]
    [Description("Verify login fails with invalid credentials")]
    public void Test_LoginWithInvalidCredentials()
    {
        string testName = "Login with Invalid Credentials";
        ReportManager.CreateTest(testName, "Verify user cannot login with invalid credentials");

        try
        {
            ReportManager.LogInfo("Test Started: " + testName);
            ReportManager.LogInfo("Step 1: Navigate to login page");
            _loginPage.NavigateToLoginPage();

            ReportManager.LogInfo("Step 2: Enter invalid username");
            _loginPage.EnterUsername("invalid_user");
            ReportManager.LogPass("Invalid username entered");

            ReportManager.LogInfo("Step 3: Enter invalid password");
            _loginPage.EnterPassword("invalid_password");
            ReportManager.LogPass("Invalid password entered");

            ReportManager.LogInfo("Step 4: Click login button");
            _loginPage.ClickLoginButton();

            ReportManager.LogInfo("Step 5: Verify error message is displayed");
            Thread.Sleep(1000);
            bool errorDisplayed = _loginPage.IsErrorMessageDisplayed();
            Assert.That(errorDisplayed, Is.True, "Error message should be displayed for invalid credentials");
            ReportManager.LogPass("Error message displayed for invalid credentials");

            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test Failed with Exception: {ex.Message}");
            throw;
        }
    }

    [Test]
    [Category("Shopping")]
    [Description("Verify user can add items to cart")]
    public void Test_AddItemsToCart()
    {
        string testName = "Add Items to Cart";
        ReportManager.CreateTest(testName, "Verify user can add multiple items to cart");

        try
        {
            ReportManager.LogInfo("Test Started: " + testName);
            
            ReportManager.LogInfo("Step 1: Login with valid credentials");
            _loginPage.NavigateToLoginPage();
            _loginPage.Login("standard_user", "secret_sauce");
            ReportManager.LogPass("Login successful");

            // Handle any unexpected alerts after login
            ReportManager.LogInfo("Checking for unexpected alerts");
            HandleUnexpectedAlerts();
            Thread.Sleep(500);

            ReportManager.LogInfo("Step 2: Verify inventory page displayed");
            Assert.That(_inventoryPage.IsInventoryPageLoaded(), Is.True);
            ReportManager.LogPass("Inventory page displayed");

            ReportManager.LogInfo("Step 3: Add first item to cart");
            _inventoryPage.AddProductToCart(0);
            ReportManager.LogPass("First item added to cart");

            ReportManager.LogInfo("Step 4: Add second item to cart");
            _inventoryPage.AddProductToCart(1);
            ReportManager.LogPass("Second item added to cart");

            ReportManager.LogInfo("Step 5: Verify cart count");
            int cartCount = _inventoryPage.GetCartItemCount();
            Assert.That(cartCount, Is.EqualTo(2), "Cart should contain 2 items");
            ReportManager.LogPass($"Cart count verified: {cartCount} items");

            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test Failed with Exception: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Handle any unexpected JavaScript alerts or dialogs
    /// </summary>
    private void HandleUnexpectedAlerts()
    {
        try
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
            IAlert? alert = wait.Until(driver =>
            {
                try
                {
                    return driver.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    return null;
                }
            });

            if (alert != null)
            {
                string alertText = alert.Text;
                ReportManager.LogWarning($"Alert detected and dismissed: {alertText}");
                alert.Accept();
            }
        }
        catch (Exception ex)
        {
            ReportManager.LogInfo($"No alerts detected: {ex.Message}");
        }
    }

    [Test]
    [Category("Shopping")]
    [Description("Verify complete checkout flow")]
    public void Test_CompleteCheckoutFlow()
    {
        string testName = "Complete Checkout Flow";
        ReportManager.CreateTest(testName, "Verify complete end-to-end checkout process");

        try
        {
            ReportManager.LogInfo("Test Started: " + testName);

            ReportManager.LogInfo("Step 1: Login");
            _loginPage.NavigateToLoginPage();
            _loginPage.Login("standard_user", "secret_sauce");
            ReportManager.LogPass("Login successful");

            ReportManager.LogInfo("Step 2: Add items to cart");
            _inventoryPage.AddProductToCart(0);
            _inventoryPage.AddProductToCart(1);
            ReportManager.LogPass("Items added to cart");

            ReportManager.LogInfo("Step 3: Navigate to cart");
            _inventoryPage.ClickCart();
            Thread.Sleep(1000);
            ReportManager.LogPass("Cart page displayed");

            ReportManager.LogInfo("Step 4: Verify cart items");
            Assert.That(_cartPage.GetCartItemCount(), Is.GreaterThan(0), "Cart should contain items");
            ReportManager.LogPass("Cart items verified");

            ReportManager.LogInfo("Step 5: Proceed to checkout");
            _cartPage.ClickCheckout();
            Thread.Sleep(1000);
            ReportManager.LogPass("Checkout page displayed");

            ReportManager.LogInfo("Step 6: Fill in checkout details");
            _checkoutPage.EnterFirstName("John");
            _checkoutPage.EnterLastName("Doe");
            _checkoutPage.EnterPostalCode("12345");
            ReportManager.LogPass("Checkout details entered");

            ReportManager.LogInfo("Step 7: Click continue");
            _checkoutPage.ClickContinue();
            Thread.Sleep(1000);
            ReportManager.LogPass("Checkout overview page displayed");

            ReportManager.LogInfo("Step 8: Verify order summary");
            Assert.That(_checkoutOverviewPage.IsCheckoutOverviewPageLoaded(), Is.True, "Checkout overview should be displayed");
            ReportManager.LogPass("Order summary verified");

            ReportManager.LogInfo("Step 9: Click finish to complete order");
            _checkoutOverviewPage.ClickFinish();
            Thread.Sleep(1000);
            ReportManager.LogPass("Finish button clicked");

            ReportManager.LogInfo("Step 10: Verify order completion");
            Assert.That(_checkoutCompletePage.IsCheckoutCompletePageLoaded(), Is.True, "Checkout complete page should be displayed");
            ReportManager.LogPass("Order completed successfully");

            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test Failed with Exception: {ex.Message}");
            throw;
        }
    }

    [Test]
    [Category("Shopping")]
    [Description("Verify logout functionality")]
    public void Test_Logout()
    {
        string testName = "Logout Functionality";
        ReportManager.CreateTest(testName, "Verify user can logout successfully");

        try
        {
            ReportManager.LogInfo("Test Started: " + testName);

            ReportManager.LogInfo("Step 1: Login");
            _loginPage.NavigateToLoginPage();
            _loginPage.Login("standard_user", "secret_sauce");
            ReportManager.LogPass("Login successful");

            ReportManager.LogInfo("Step 2: Verify user is logged in");
            Assert.That(_inventoryPage.IsInventoryPageLoaded(), Is.True);
            ReportManager.LogPass("Inventory page displayed");

            ReportManager.LogInfo("Step 3: Click menu button");
            _inventoryPage.OpenMenu();
            ReportManager.LogPass("Menu opened");

            ReportManager.LogInfo("Step 4: Click logout");
            _inventoryPage.Logout();
            Thread.Sleep(1000);
            ReportManager.LogPass("Logout clicked");

            ReportManager.LogInfo("Step 5: Verify login page is displayed");
            Assert.That(_loginPage.IsLoginPageLoaded(), Is.True, "Login page should be displayed after logout");
            ReportManager.LogPass("Logout successful - Login page displayed");

            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test Failed with Exception: {ex.Message}");
            throw;
        }
    }

    #endregion
}
