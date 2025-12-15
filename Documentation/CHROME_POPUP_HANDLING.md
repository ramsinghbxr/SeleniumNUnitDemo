# Chrome Popup and Notification Handling Guide

## Overview

This guide explains how the project handles Chrome browser popups and notifications that could interrupt test execution.

---

## Problem Statement

Chrome browser displays various popups that can interfere with Selenium tests:

- **Save Password Prompt** - Appears after login
- **Save Payment Method** - For forms with payment fields
- **Enable Notifications** - For sites using notification APIs
- **Auto-fill Suggestions** - Blocks element interactions
- **Password Manager Dialog** - Re-authentication prompts

These popups can:
- Block element clicks
- Cause timeouts
- Make tests flaky and unreliable
- Require additional waits

---

## Solution: Chrome Options Configuration

### ChromeDriverConfig Utility

A dedicated utility class `Utilities/ChromeDriverConfig.cs` provides centralized configuration:

```csharp
public static class ChromeDriverConfig
{
    // Configure options to suppress all popups
    public static ChromeOptions GetChromeOptions()
    
    // Configure for headless mode (CI/CD)
    public static ChromeOptions GetChromeOptionsHeadless()
    
    // Create pre-configured driver
    public static IWebDriver CreateChromeDriver()
    
    // Create headless driver
    public static IWebDriver CreateChromeDriverHeadless()
}
```

### Key Configurations

#### 1. **Disable Password Manager**
```csharp
options.AddArgument("--disable-password-manager-reauthentication");
options.AddLocalStatePreference("credentials_enable_service", false);
options.AddLocalStatePreference("profile.password_manager_enabled", false);
```
Prevents "Save password?" popup after login

#### 2. **Disable Browser Popups**
```csharp
options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
```
Blocks popup windows from opening

#### 3. **Disable Notifications**
```csharp
options.AddArgument("--disable-notifications");
options.AddUserProfilePreference("profile.managed_default_content_settings.notifications", 2);
```
Suppresses notification permission prompts

#### 4. **Disable Auto-fill Features**
```csharp
options.AddArgument("--disable-extensions");
options.AddArgument("--disable-sync");
```
Prevents auto-fill dialogs

#### 5. **Stability Arguments**
```csharp
options.AddArgument("--disable-dev-shm-usage");
options.AddArgument("--no-sandbox");
options.AddArgument("--start-maximized");
```
Ensures reliable execution in headless/CI environments

---

## Usage Examples

### Using the Utility (Recommended)

```csharp
[SetUp]
public void Setup()
{
    // Option 1: Regular driver
    _driver = ChromeDriverConfig.CreateChromeDriver();
    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
}
```

```csharp
[SetUp]
public void Setup()
{
    // Option 2: Headless driver (for CI/CD)
    _driver = ChromeDriverConfig.CreateChromeDriverHeadless();
}
```

```csharp
[SetUp]
public void Setup()
{
    // Option 3: Custom options
    var options = ChromeDriverConfig.GetChromeOptions();
    // Add additional custom options if needed
    var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options);
}
```

### Before (Repetitive Code)

```csharp
// In each test class Setup method
var service = ChromeDriverService.CreateDefaultService();
var options = new ChromeOptions();
options.AddArgument("--disable-dev-shm-usage");
options.AddArgument("--no-sandbox");
options.AddArgument("--start-maximized");
options.AddArgument("--disable-password-manager-reauthentication");
options.AddLocalStatePreference("credentials_enable_service", false);
options.AddLocalStatePreference("profile.password_manager_enabled", false);
options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
options.AddUserProfilePreference("profile.managed_default_content_settings.notifications", 2);
_driver = new ChromeDriver(service, options);
```

### After (Clean Code)

```csharp
// Simple one-liner
_driver = ChromeDriverConfig.CreateChromeDriver();
```

---

## Implementation in Project

All test classes have been updated to use `ChromeDriverConfig`:

### SwagLabsTests.cs
```csharp
[SetUp]
public void SetupTest()
{
    _driver = ChromeDriverConfig.CreateChromeDriver();
    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
}
```

### SwagLabsTests_POM.cs
```csharp
[SetUp]
public void Setup()
{
    _driver = ChromeDriverConfig.CreateChromeDriver();
    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
}
```

### SwagLabsTests_WithReports.cs
```csharp
[SetUp]
public void Setup()
{
    _driver = ChromeDriverConfig.CreateChromeDriver();
    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
}
```

---

## Benefits

✅ **No Popup Interruptions** - Tests run uninterrupted
✅ **Stable Execution** - Fewer flaky tests
✅ **Cleaner Code** - Centralized configuration
✅ **Easy Maintenance** - Update once, apply everywhere
✅ **CI/CD Ready** - Headless mode support
✅ **Reusable** - Use across all test suites

---

## Troubleshooting

### Issue: Password Manager Dialog Still Appears

**Solution:** Ensure all three preferences are set:
```csharp
options.AddArgument("--disable-password-manager-reauthentication");
options.AddLocalStatePreference("credentials_enable_service", false);
options.AddLocalStatePreference("profile.password_manager_enabled", false);
```

### Issue: Notifications Still Showing

**Solution:** Set both notification preferences:
```csharp
options.AddArgument("--disable-notifications");
options.AddUserProfilePreference("profile.managed_default_content_settings.notifications", 2);
```

### Issue: Auto-fill Fields Causing Issues

**Solution:** Add these arguments:
```csharp
options.AddArgument("--disable-extensions");
options.AddArgument("--disable-sync");
```

---

## Headless Mode for CI/CD

For continuous integration pipelines, use headless mode:

```bash
# In CI/CD pipeline
dotnet test
```

Then use:

```csharp
_driver = ChromeDriverConfig.CreateChromeDriverHeadless();
```

This provides:
- ✅ No GUI overhead
- ✅ Faster execution
- ✅ Better resource utilization
- ✅ Server/container friendly

---

## Chrome Versions Compatibility

These options work with:
- Chrome 90+
- Chromium 90+
- Edge 90+

Tested with: **Chrome 143.0.7499.40**

---

## Additional Resources

- [Chrome DevTools Protocol](https://chromedevtools.github.io/devtools-protocol/)
- [Chromium Command Line Flags](https://peter.sh/experiments/chromium-command-line-flags/)
- [Selenium Chrome Options](https://www.selenium.dev/documentation/webdriver/capabilities/chrome_options/)

---

## Summary

The project now provides robust popup and notification handling through:

1. **Centralized Configuration** - `ChromeDriverConfig` utility class
2. **Easy Integration** - One-line driver creation
3. **Comprehensive Coverage** - All popup types handled
4. **Production Ready** - Tested and reliable
5. **Maintainable** - Single source of truth for Chrome configuration

Tests will now run without interruptions from browser popups! 🎉
