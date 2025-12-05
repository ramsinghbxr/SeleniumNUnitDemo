using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumNUnitDemo.Pages;

/// <summary>
/// Base Page class - contains common methods and properties for all pages
/// Implements Page Object Model pattern
/// </summary>
public abstract class BasePage
{
    protected IWebDriver _driver;
    protected WebDriverWait _wait;
    protected const int DefaultTimeout = 10;

    public BasePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeout));
    }

    /// <summary>
    /// Navigate to a specific URL
    /// </summary>
    public virtual void NavigateTo(string url)
    {
        _driver.Navigate().GoToUrl(url);
    }

    /// <summary>
    /// Wait for element to be visible and return it
    /// </summary>
    protected IWebElement WaitForElement(By locator)
    {
        return _wait.Until(driver => driver.FindElement(locator));
    }

    /// <summary>
    /// Wait for element to be clickable and click it
    /// </summary>
    protected void WaitAndClick(By locator)
    {
        var element = _wait.Until(driver =>
        {
            var el = driver.FindElement(locator);
            if (el.Displayed && el.Enabled)
                return el;
            return null;
        });
        element.Click();
    }

    /// <summary>
    /// Wait for element and send keys
    /// </summary>
    protected void WaitAndSendKeys(By locator, string text)
    {
        var element = WaitForElement(locator);
        element.Clear();
        element.SendKeys(text);
    }

    /// <summary>
    /// Get text from element with wait
    /// </summary>
    protected string GetElementText(By locator)
    {
        return WaitForElement(locator).Text;
    }

    /// <summary>
    /// Check if element is displayed
    /// </summary>
    protected bool IsElementDisplayed(By locator)
    {
        try
        {
            return _driver.FindElement(locator).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    /// <summary>
    /// Take screenshot
    /// </summary>
    public void TakeScreenshot(string fileName)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            string screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
            Directory.CreateDirectory(screenshotDir);
            string filePath = Path.Combine(screenshotDir, fileName + ".png");
            screenshot.SaveAsFile(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to take screenshot: {ex.Message}");
        }
    }

    /// <summary>
    /// Get page title
    /// </summary>
    public string GetPageTitle()
    {
        return _driver.Title;
    }

    /// <summary>
    /// Get current URL
    /// </summary>
    public string GetCurrentUrl()
    {
        return _driver.Url;
    }
}
