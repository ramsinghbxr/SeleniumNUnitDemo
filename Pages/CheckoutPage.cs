using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Checkout Page Object - Contains all elements and methods for Checkout Page
/// Uses Page Object Model pattern with By locators
/// </summary>
public class CheckoutPage : BasePage
{
    public CheckoutPage(IWebDriver driver) : base(driver)
    {
        // Page elements initialized via locators
    }

    #region Web Element Locators

    private By FirstNameFieldLocator => By.Id("first-name");
    private By LastNameFieldLocator => By.Id("last-name");
    private By PostalCodeFieldLocator => By.Id("postal-code");
    private By ContinueButtonLocator => By.Id("continue");
    private By FinishButtonLocator => By.Id("finish");
    private By CancelButtonLocator => By.Id("cancel");
    private By PageTitleLocator => By.ClassName("title");

    #endregion

    #region Page Actions/Methods

    /// <summary>
    /// Verify checkout page is loaded
    /// </summary>
    public bool IsCheckoutPageLoaded()
    {
        return IsElementDisplayed(By.Id("first-name"));
    }

    /// <summary>
    /// Enter first name
    /// </summary>
    public void EnterFirstName(string firstName)
    {
        WaitAndSendKeys(FirstNameFieldLocator, firstName);
    }

    /// <summary>
    /// Enter last name
    /// </summary>
    public void EnterLastName(string lastName)
    {
        WaitAndSendKeys(LastNameFieldLocator, lastName);
    }

    /// <summary>
    /// Enter postal code
    /// </summary>
    public void EnterPostalCode(string postalCode)
    {
        WaitAndSendKeys(PostalCodeFieldLocator, postalCode);
    }

    /// <summary>
    /// Fill checkout form
    /// </summary>
    public void FillCheckoutForm(string firstName, string lastName, string postalCode)
    {
        EnterFirstName(firstName);
        EnterLastName(lastName);
        EnterPostalCode(postalCode);
    }

    /// <summary>
    /// Click continue button
    /// </summary>
    public CheckoutOverviewPage ClickContinue()
    {
        WaitAndClick(ContinueButtonLocator);
        return new CheckoutOverviewPage(_driver);
    }

    /// <summary>
    /// Click cancel button
    /// </summary>
    public CartPage ClickCancel()
    {
        WaitAndClick(CancelButtonLocator);
        return new CartPage(_driver);
    }

    #endregion
}
