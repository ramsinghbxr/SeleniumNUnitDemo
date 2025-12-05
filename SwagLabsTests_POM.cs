using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumNUnitDemo.Pages;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumNUnitDemo;

/// <summary>
/// Test class using Page Object Model (POM) pattern
/// All UI interactions are encapsulated in Page Objects
/// Tests remain clean and maintainable
/// </summary>
[TestFixture]
public class SwagLabsTests_POM
{
    private IWebDriver _driver;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // This runs once before all tests in the class
        new DriverManager().SetUpDriver(new ChromeConfig());
    }

    [SetUp]
    public void Setup()
    {
        // This runs before each test
        var service = ChromeDriverService.CreateDefaultService();
        var options = new ChromeOptions();
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--start-maximized");

        _driver = new ChromeDriver(service, options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TearDown]
    public void TearDown()
    {
        // This runs after each test
        _driver?.Quit();
        _driver?.Dispose();
    }

    #region Test Cases

    /// <summary>
    /// Test Case 1: Verify successful login with valid credentials
    /// </summary>
    [Test]
    [Order(1)]
    public void Test_01_SuccessfulLogin()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();

        // Act
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

        // Assert
        Assert.IsTrue(inventoryPage.IsInventoryPageLoaded(), "Inventory page should be loaded");
        Assert.That(inventoryPage.GetInventoryPageTitle(), Is.EqualTo("Products"), "Page title should be 'Products'");
    }

    /// <summary>
    /// Test Case 2: Verify login with invalid credentials shows error
    /// </summary>
    [Test]
    [Order(2)]
    public void Test_02_LoginWithInvalidCredentials()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();

        // Act
        loginPage.Login("invalid_user", "wrong_password");

        // Assert
        Assert.IsTrue(loginPage.IsErrorMessageDisplayed(), "Error message should be displayed");
        Assert.That(loginPage.GetErrorMessage(), Contains.Substring("Epic sadface"), "Error message should contain appropriate text");
    }

    /// <summary>
    /// Test Case 3: Verify products are displayed on inventory page
    /// </summary>
    [Test]
    [Order(3)]
    public void Test_03_VerifyProductsOnInventoryPage()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

        // Act
        int productCount = inventoryPage.GetInventoryItemCount();

        // Assert
        Assert.Greater(productCount, 0, "Products should be displayed");
        Assert.That(productCount, Is.EqualTo(6), "Should display 6 products");
    }

    /// <summary>
    /// Test Case 4: Verify product can be added to cart
    /// </summary>
    [Test]
    [Order(4)]
    public void Test_04_AddProductToCart()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

        // Act
        inventoryPage.AddProductToCart(0);
        int cartCount = inventoryPage.GetCartItemCount();

        // Assert
        Assert.That(cartCount, Is.EqualTo(1), "Cart should have 1 item");
    }

    /// <summary>
    /// Test Case 5: Verify multiple products can be added to cart
    /// </summary>
    [Test]
    [Order(5)]
    public void Test_05_AddMultipleProductsToCart()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

        // Act
        inventoryPage.AddProductToCart(0);
        inventoryPage.AddProductToCart(1);
        inventoryPage.AddProductToCart(2);
        int cartCount = inventoryPage.GetCartItemCount();

        // Assert
        Assert.That(cartCount, Is.EqualTo(3), "Cart should have 3 items");
    }

    /// <summary>
    /// Test Case 6: Verify cart page displays added products
    /// </summary>
    [Test]
    [Order(6)]
    public void Test_06_ViewCartWithProducts()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");
        inventoryPage.AddProductToCart(0);
        inventoryPage.AddProductToCart(1);

        // Act
        var cartPage = inventoryPage.ClickCart();
        int cartItemCount = cartPage.GetCartItemCount();

        // Assert
        Assert.IsTrue(cartPage.IsCartPageLoaded(), "Cart page should be loaded");
        Assert.That(cartItemCount, Is.EqualTo(2), "Cart should contain 2 products");
    }

    /// <summary>
    /// Test Case 7: Verify product can be removed from cart
    /// </summary>
    [Test]
    [Order(7)]
    public void Test_07_RemoveProductFromCart()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");
        inventoryPage.AddProductToCart(0);
        inventoryPage.AddProductToCart(1);
        var cartPage = inventoryPage.ClickCart();

        // Act
        cartPage.RemoveItemFromCart(0);
        int cartItemCount = cartPage.GetCartItemCount();

        // Assert
        Assert.That(cartItemCount, Is.EqualTo(1), "Cart should have 1 item after removal");
    }

    /// <summary>
    /// Test Case 8: Verify complete checkout flow
    /// </summary>
    [Test]
    [Order(8)]
    public void Test_08_CompleteCheckoutFlow()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");
        inventoryPage.AddProductToCart(0);
        inventoryPage.AddProductToCart(1);
        var cartPage = inventoryPage.ClickCart();

        // Act - Go to checkout
        var checkoutPage = cartPage.ClickCheckout();

        // Assert
        Assert.IsTrue(checkoutPage.IsCheckoutPageLoaded(), "Checkout page should be loaded");
    }

    /// <summary>
    /// Test Case 9: Verify checkout form validation
    /// </summary>
    [Test]
    [Order(9)]
    public void Test_09_CheckoutFormFilling()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");
        inventoryPage.AddProductToCart(0);
        var cartPage = inventoryPage.ClickCart();
        var checkoutPage = cartPage.ClickCheckout();

        // Act
        checkoutPage.FillCheckoutForm("John", "Doe", "12345");
        var overviewPage = checkoutPage.ClickContinue();

        // Assert
        Assert.IsTrue(overviewPage.IsCheckoutOverviewPageLoaded(), "Checkout overview page should be loaded");
    }

    /// <summary>
    /// Test Case 10: Verify complete purchase
    /// </summary>
    [Test]
    [Order(10)]
    public void Test_10_CompletePurchase()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");
        inventoryPage.AddProductToCart(0);
        var cartPage = inventoryPage.ClickCart();
        var checkoutPage = cartPage.ClickCheckout();
        checkoutPage.FillCheckoutForm("Jane", "Smith", "54321");
        var overviewPage = checkoutPage.ClickContinue();

        // Act
        var completePage = overviewPage.ClickFinish();

        // Assert
        Assert.IsTrue(completePage.IsCheckoutCompletePageLoaded(), "Checkout complete page should be loaded");
        Assert.That(completePage.GetCompletionMessage(), Contains.Substring("Thank you"), "Completion message should be displayed");
    }

    /// <summary>
    /// Test Case 11: Verify logout functionality
    /// </summary>
    [Test]
    [Order(11)]
    public void Test_11_LogoutFunctionality()
    {
        // Arrange
        var loginPage = new LoginPage(_driver);
        loginPage.NavigateToLoginPage();
        var inventoryPage = loginPage.Login("standard_user", "secret_sauce");

        // Act
        var logoutLoginPage = inventoryPage.Logout();

        // Assert
        Assert.IsTrue(logoutLoginPage.IsLoginPageLoaded(), "Should be back at login page after logout");
    }

    #endregion
}
