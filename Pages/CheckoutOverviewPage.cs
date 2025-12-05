using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Checkout Overview Page Object - Contains all elements and methods for Checkout Overview
/// Uses Page Object Model pattern with By locators
/// </summary>
public class CheckoutOverviewPage : BasePage
{
    public CheckoutOverviewPage(IWebDriver driver) : base(driver)
    {
        // Page elements initialized via locators
    }

    #region Web Element Locators

    private By PageTitleLocator => By.ClassName("title");
    private By SubtotalLabelLocator => By.ClassName("summary_subtotal_label");
    private By TaxLabelLocator => By.ClassName("summary_tax_label");
    private By TotalLabelLocator => By.ClassName("summary_total_label");
    private By FinishButtonLocator => By.Id("finish");
    private By CancelButtonLocator => By.Id("cancel");
    private By CartItemsLocator => By.ClassName("cart_item");

    #endregion

    #region Page Actions/Methods

    /// <summary>
    /// Verify checkout overview page is loaded
    /// </summary>
    public bool IsCheckoutOverviewPageLoaded()
    {
        return IsElementDisplayed(By.ClassName("title"));
    }

    /// <summary>
    /// Get subtotal amount
    /// </summary>
    public string GetSubtotal()
    {
        return GetElementText(By.ClassName("summary_subtotal_label"));
    }

    /// <summary>
    /// Get tax amount
    /// </summary>
    public string GetTax()
    {
        return GetElementText(By.ClassName("summary_tax_label"));
    }

    /// <summary>
    /// Get total amount
    /// </summary>
    public string GetTotal()
    {
        return GetElementText(By.ClassName("summary_total_label"));
    }

    /// <summary>
    /// Get number of items in order
    /// </summary>
    public int GetOrderItemCount()
    {
        return _driver.FindElements(CartItemsLocator).Count;
    }

    /// <summary>
    /// Click finish button to complete order
    /// </summary>
    public CheckoutCompletePage ClickFinish()
    {
        WaitAndClick(FinishButtonLocator);
        return new CheckoutCompletePage(_driver);
    }

    /// <summary>
    /// Click cancel button
    /// </summary>
    public InventoryPage ClickCancel()
    {
        WaitAndClick(CancelButtonLocator);
        return new InventoryPage(_driver);
    }

    #endregion
}
