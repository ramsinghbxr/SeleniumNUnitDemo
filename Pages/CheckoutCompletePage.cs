using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Checkout Complete Page Object - Contains all elements and methods for Order Completion Page
/// Uses Page Object Model pattern with By locators
/// </summary>
public class CheckoutCompletePage : BasePage
{
    public CheckoutCompletePage(IWebDriver driver) : base(driver)
    {
        // Page elements initialized via locators
    }

    #region Web Element Locators

    private By CompleteHeaderLocator => By.ClassName("complete-header");
    private By CompleteTextLocator => By.ClassName("complete-text");
    private By BackToProductsButtonLocator => By.Id("back-to-products");
    private By MenuButtonLocator => By.Id("react-burger-menu-btn");
    private By LogoutButtonLocator => By.Id("logout_sidebar_link");

    #endregion

    #region Page Actions/Methods

    /// <summary>
    /// Verify checkout completion page is loaded
    /// </summary>
    public bool IsCheckoutCompletePageLoaded()
    {
        return IsElementDisplayed(By.ClassName("complete-header"));
    }

    /// <summary>
    /// Get completion message
    /// </summary>
    public string GetCompletionMessage()
    {
        return GetElementText(By.ClassName("complete-header"));
    }

    /// <summary>
    /// Get completion details text
    /// </summary>
    public string GetCompletionDetails()
    {
        return GetElementText(By.ClassName("complete-text"));
    }

    /// <summary>
    /// Click back to products button
    /// </summary>
    public InventoryPage ClickBackToProducts()
    {
        WaitAndClick(BackToProductsButtonLocator);
        return new InventoryPage(_driver);
    }

    /// <summary>
    /// Logout from complete page
    /// </summary>
    public LoginPage Logout()
    {
        WaitAndClick(MenuButtonLocator);
        WaitAndClick(LogoutButtonLocator);
        return new LoginPage(_driver);
    }

    #endregion
}
