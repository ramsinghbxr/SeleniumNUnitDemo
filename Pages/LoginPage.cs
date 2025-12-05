using OpenQA.Selenium;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Login Page Object - Contains all elements and methods for Login Page
/// </summary>
public class LoginPage : BasePage
{
    // Web Element Locators
    private By UsernameFieldLocator => By.Id("user-name");
    private By PasswordFieldLocator => By.Id("password");
    private By LoginButtonLocator => By.Id("login-button");
    private By LoginLogoLocator => By.ClassName("login_logo");
    private By ErrorMessageLocator => By.ClassName("error-message-container");

    public LoginPage(IWebDriver driver) : base(driver)
    {
    }

    #region Page Actions/Methods

    /// <summary>
    /// Navigate to login page
    /// </summary>
    public void NavigateToLoginPage()
    {
        NavigateTo("https://www.saucedemo.com");
    }

    /// <summary>
    /// Verify login page is loaded
    /// </summary>
    public bool IsLoginPageLoaded()
    {
        return IsElementDisplayed(LoginLogoLocator);
    }

    /// <summary>
    /// Enter username
    /// </summary>
    public void EnterUsername(string username)
    {
        WaitAndSendKeys(UsernameFieldLocator, username);
    }

    /// <summary>
    /// Enter password
    /// </summary>
    public void EnterPassword(string password)
    {
        WaitAndSendKeys(PasswordFieldLocator, password);
    }

    /// <summary>
    /// Click login button
    /// </summary>
    public void ClickLoginButton()
    {
        WaitAndClick(LoginButtonLocator);
    }

    /// <summary>
    /// Perform login action
    /// </summary>
    public InventoryPage Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLoginButton();
        System.Threading.Thread.Sleep(1000);
        return new InventoryPage(_driver);
    }

    /// <summary>
    /// Get error message text
    /// </summary>
    public string GetErrorMessage()
    {
        if (IsElementDisplayed(ErrorMessageLocator))
        {
            return GetElementText(ErrorMessageLocator);
        }
        return string.Empty;
    }

    /// <summary>
    /// Check if error message is displayed
    /// </summary>
    public bool IsErrorMessageDisplayed()
    {
        return IsElementDisplayed(ErrorMessageLocator);
    }

    #endregion
}
