using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumNUnitDemo.Utilities;

/// <summary>
/// Chrome Driver Configuration Utility
/// Handles Chrome browser setup with popup and notification suppression
/// </summary>
public static class ChromeDriverConfig
{
    /// <summary>
    /// Configure Chrome options to suppress popups and notifications
    /// </summary>
    public static ChromeOptions GetChromeOptions()
    {
        var options = new ChromeOptions();
        
        // Basic arguments for stability and performance
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--start-maximized");
        
        // Disable password save/autofill prompts
        options.AddArgument("--disable-password-manager-reauthentication");
        options.AddLocalStatePreference("credentials_enable_service", false);
        options.AddLocalStatePreference("profile.password_manager_enabled", false);
        
        // Disable notifications and popups
        options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
        options.AddUserProfilePreference("profile.managed_default_content_settings.notifications", 2);
        
        // Disable various browser features that might interfere
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-default-apps");
        options.AddArgument("--disable-sync");
        options.AddArgument("--disable-extensions");
        
        return options;
    }

    /// <summary>
    /// Configure Chrome options with headless mode (for CI/CD)
    /// </summary>
    public static ChromeOptions GetChromeOptionsHeadless()
    {
        var options = GetChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        return options;
    }

    /// <summary>
    /// Create ChromeDriver with configured options
    /// </summary>
    public static IWebDriver CreateChromeDriver()
    {
        var service = ChromeDriverService.CreateDefaultService();
        var options = GetChromeOptions();
        return new ChromeDriver(service, options);
    }

    /// <summary>
    /// Create ChromeDriver with headless options
    /// </summary>
    public static IWebDriver CreateChromeDriverHeadless()
    {
        var service = ChromeDriverService.CreateDefaultService();
        var options = GetChromeOptionsHeadless();
        return new ChromeDriver(service, options);
    }
}
