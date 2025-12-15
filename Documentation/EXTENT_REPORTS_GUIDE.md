# Extent Reports Integration Guide

## Overview

This document provides comprehensive guidance on using **Extent Reports** with the Selenium NUnit automation framework to generate beautiful HTML reports with screenshots and detailed test execution logs.

---

## Table of Contents

1. [What is Extent Reports?](#what-is-extent-reports)
2. [Features](#features)
3. [Project Structure](#project-structure)
4. [Installation](#installation)
5. [Implementation](#implementation)
6. [Usage Examples](#usage-examples)
7. [Report Features](#report-features)
8. [Troubleshooting](#troubleshooting)

---

## What is Extent Reports?

Extent Reports is a modern, open-source reporting library for test automation that generates beautiful, interactive HTML reports. It provides:

- **Rich HTML Reports** with detailed test information
- **Screenshots and Media** attached to test reports
- **Real-time Test Logging** with multiple severity levels
- **Test Categorization** and filtering capabilities
- **Execution History** and trend analysis
- **Multi-environment Support** for CI/CD pipelines

---

## Features

### ✅ Implemented Features

1. **Automatic Screenshot Capture**
   - Screenshots captured on test failure
   - Saved with timestamp for easy identification
   - Embedded in HTML report with clickable links

2. **Detailed Test Logging**
   - Step-by-step test execution tracking
   - Multiple log levels: Info, Pass, Fail, Warning, Error
   - Real-time status updates

3. **Beautiful HTML Reports**
   - Professional dark theme
   - Interactive test results
   - System information captured
   - Test execution summary

4. **Report Organization**
   - Tests categorized by functionality
   - Test descriptions and metadata
   - Timestamp-based report naming

5. **Screenshot Management**
   - Screenshots stored in dedicated folder
   - Relative paths for report portability
   - Automatic folder creation

---

## Project Structure

```
SeleniumNUnitDemo/
├── Utilities/
│   ├── ReportManager.cs          ← Report management utility
│   └── [Other utilities]
├── Pages/
│   ├── BasePage.cs               ← Base page with screenshot method
│   ├── LoginPage.cs
│   ├── InventoryPage.cs
│   ├── CartPage.cs
│   └── [Other pages]
├── SwagLabsTests_WithReports.cs  ← Test class with report integration
├── bin/
│   └── Debug/
│       └── net7.0/
│           ├── ExtentReports/    ← Generated HTML reports
│           └── Screenshots/       ← Captured screenshots
└── [Other project files]
```

---

## Installation

### Step 1: Install NuGet Package

```bash
dotnet add package AventStack.ExtentReports
```

Or via Package Manager:

```powershell
Install-Package AventStack.ExtentReports
```

### Step 2: Verify Installation

```bash
dotnet list package
```

You should see `AventStack.ExtentReports 4.1.0` or later in the output.

---

## Implementation

### ReportManager Class

The `ReportManager` utility class provides static methods for managing Extent Reports:

```csharp
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

public class ReportManager
{
    private static ExtentReports? _extentReports;
    private static ExtentTest? _extentTest;
    
    // Key Methods:
    public static void InitializeReport()         // Initialize reports
    public static void CreateTest(string name)    // Create a test
    public static void LogInfo(string message)    // Log info message
    public static void LogPass(string message)    // Log pass message
    public static void LogFail(string message)    // Log fail message
    public static void LogWarning(string message) // Log warning
    public static void LogError(string message)   // Log error
    public static void AddScreenshot(string path) // Add screenshot
    public static void MarkTestPass()             // Mark test as pass
    public static void MarkTestFail(string reason) // Mark test as fail
    public static void FlushReport()              // Finalize and save report
    public static string GetReportPath()          // Get report file path
}
```

### BasePage Enhancement

The `BasePage` class has been updated with improved screenshot functionality:

```csharp
/// <summary>
/// Take screenshot and return the file path
/// </summary>
public string TakeScreenshot(string fileName)
{
    try
    {
        var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
        string screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
        Directory.CreateDirectory(screenshotDir);
        string filePath = Path.Combine(screenshotDir, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        screenshot.SaveAsFile(filePath);
        return filePath;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to take screenshot: {ex.Message}");
        return string.Empty;
    }
}
```

---

## Usage Examples

### Example 1: Basic Test with Reporting

```csharp
[TestFixture]
public class SwagLabsTests_WithReports
{
    private IWebDriver _driver;
    private LoginPage _loginPage;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        ReportManager.InitializeReport();
        ReportManager.LogInfo("Test Suite Started");
    }

    [SetUp]
    public void Setup()
    {
        _driver = InitializeDriver();
        _loginPage = new LoginPage(_driver);
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            string screenshotPath = _loginPage.TakeScreenshot($"Failure_{TestContext.CurrentContext.Test.Name}");
            ReportManager.LogFail($"Test Failed: {TestContext.CurrentContext.Result.Message}", screenshotPath);
        }
        _driver?.Quit();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ReportManager.FlushReport();
        Console.WriteLine($"Report generated at: {ReportManager.GetReportPath()}");
    }

    [Test]
    public void Test_LoginWithValidCredentials()
    {
        ReportManager.CreateTest("Login with Valid Credentials");
        
        try
        {
            ReportManager.LogInfo("Navigating to login page");
            _loginPage.NavigateToLoginPage();
            
            ReportManager.LogInfo("Entering credentials");
            _loginPage.EnterUsername("standard_user");
            _loginPage.EnterPassword("secret_sauce");
            
            ReportManager.LogInfo("Clicking login button");
            _loginPage.ClickLoginButton();
            
            ReportManager.LogPass("Login successful");
            ReportManager.MarkTestPass();
        }
        catch (Exception ex)
        {
            ReportManager.LogError($"Test failed: {ex.Message}");
            throw;
        }
    }
}
```

### Example 2: Multi-step Test with Screenshots

```csharp
[Test]
public void Test_CompleteCheckoutFlow()
{
    string testName = "Complete Checkout Flow";
    ReportManager.CreateTest(testName, "Verify end-to-end checkout");

    try
    {
        // Login
        ReportManager.LogInfo("Step 1: Login with valid credentials");
        _loginPage.NavigateToLoginPage();
        _loginPage.Login("standard_user", "secret_sauce");
        ReportManager.LogPass("Login successful");

        // Add to cart
        ReportManager.LogInfo("Step 2: Add items to cart");
        _inventoryPage.AddProductToCart(0);
        _inventoryPage.AddProductToCart(1);
        ReportManager.LogPass("Items added to cart");

        // Checkout
        ReportManager.LogInfo("Step 3: Navigate to cart and checkout");
        _inventoryPage.ClickCart();
        _cartPage.ClickCheckout();
        ReportManager.LogPass("Checkout page displayed");

        // Fill form
        ReportManager.LogInfo("Step 4: Fill checkout form");
        _checkoutPage.EnterFirstName("John");
        _checkoutPage.EnterLastName("Doe");
        _checkoutPage.EnterPostalCode("12345");
        ReportManager.LogPass("Checkout form filled");

        // Complete order
        ReportManager.LogInfo("Step 5: Complete order");
        _checkoutPage.ClickContinue();
        _checkoutOverviewPage.ClickFinish();
        ReportManager.LogPass("Order completed successfully");

        ReportManager.MarkTestPass();
    }
    catch (Exception ex)
    {
        ReportManager.LogError($"Checkout failed: {ex.Message}");
        throw;
    }
}
```

### Example 3: Test with Screenshots at Multiple Points

```csharp
[Test]
public void Test_WithMultipleScreenshots()
{
    ReportManager.CreateTest("Multiple Screenshots Test");

    try
    {
        // Take screenshot of login page
        ReportManager.LogInfo("Navigating to login page");
        _loginPage.NavigateToLoginPage();
        string loginPageScreenshot = _loginPage.TakeScreenshot("LoginPage");
        ReportManager.AddScreenshot(loginPageScreenshot, "Login Page State");

        // Perform login
        ReportManager.LogInfo("Performing login");
        _loginPage.Login("standard_user", "secret_sauce");

        // Take screenshot of inventory page
        string inventoryPageScreenshot = _loginPage.TakeScreenshot("InventoryPage");
        ReportManager.AddScreenshot(inventoryPageScreenshot, "Inventory Page After Login");

        ReportManager.MarkTestPass();
    }
    catch (Exception ex)
    {
        ReportManager.LogError($"Test failed: {ex.Message}");
        throw;
    }
}
```

---

## Report Features

### 1. Report Location

Reports are generated in: `bin/Debug/net7.0/ExtentReports/`

Filename format: `TestReport_YYYYMMdd_HHmmss.html`

Example: `TestReport_20240115_143022.html`

### 2. Screenshots Location

Screenshots are saved in: `bin/Debug/net7.0/Screenshots/`

Filename format: `TestName_YYYYMMdd_HHmmss.png`

Example: `LoginFailure_20240115_143022.png`

### 3. Report Contents

- **Dashboard**: Overview of all tests (Pass/Fail/Skip counts)
- **Test Details**: Individual test information
  - Test name and description
  - Execution time
  - Status (Pass/Fail/Skip)
  - Category and severity
- **Step Logs**: Detailed execution steps with timestamps
- **Screenshots**: Embedded images with descriptions
- **Stack Traces**: Error details for failed tests
- **System Information**: OS, Browser, Framework details

### 4. Log Levels

```csharp
ReportManager.LogInfo("General information");      // Blue
ReportManager.LogPass("Step completed");           // Green
ReportManager.LogWarning("Warning message");       // Yellow
ReportManager.LogError("Error occurred");          // Red
ReportManager.LogFail("Test step failed");         // Red with fail mark
```

---

## Running Tests with Reports

### Execute All Tests

```bash
dotnet test
```

### Execute Specific Test Class

```bash
dotnet test --filter "ClassName=SwagLabsTests_WithReports"
```

### Execute Tests with Specific Category

```bash
dotnet test --filter "Category=Login"
```

### View Generated Report

After test execution, open the generated HTML report:

```powershell
# On Windows
Start-Process (Get-ChildItem "bin/Debug/net7.0/ExtentReports" -Filter "*.html" | Sort-Object LastWriteTime -Descending | Select-Object -First 1).FullName
```

---

## Best Practices

### 1. Always Initialize and Flush Reports

```csharp
[OneTimeSetUp]
public void Setup()
{
    ReportManager.InitializeReport();
}

[OneTimeTearDown]
public void Cleanup()
{
    ReportManager.FlushReport();
}
```

### 2. Create Test Before Execution

```csharp
[Test]
public void TestMethod()
{
    ReportManager.CreateTest("Test Name", "Test Description");
    // ... test code
}
```

### 3. Log at Critical Points

```csharp
try
{
    ReportManager.LogInfo("Starting operation");
    // Perform action
    ReportManager.LogPass("Operation successful");
}
catch (Exception ex)
{
    ReportManager.LogError($"Operation failed: {ex.Message}");
    throw;
}
```

### 4. Attach Screenshots on Failure

```csharp
if (testFailed)
{
    string screenshotPath = _page.TakeScreenshot("FailureScreenshot");
    ReportManager.LogFail("Test assertion failed", screenshotPath);
}
```

### 5. Use Meaningful Test Names

```csharp
// Good
ReportManager.CreateTest("User can add item to cart");

// Bad
ReportManager.CreateTest("Test 1");
```

---

## Troubleshooting

### Issue: Report Not Generated

**Solution:** Ensure `ReportManager.FlushReport()` is called in `OneTimeTearDown`:

```csharp
[OneTimeTearDown]
public void Cleanup()
{
    ReportManager.FlushReport();
}
```

### Issue: Screenshots Not Appearing in Report

**Solution:** Verify screenshot file path is correct:

```csharp
string screenshotPath = _page.TakeScreenshot("TestName");
if (File.Exists(screenshotPath))
{
    ReportManager.AddScreenshot(screenshotPath);
}
```

### Issue: NullReferenceException for ReportManager

**Solution:** Ensure `InitializeReport()` is called before any tests:

```csharp
[OneTimeSetUp]
public void Setup()
{
    ReportManager.InitializeReport();
}
```

### Issue: Report File Not Found

**Solution:** Check the exact path:

```csharp
string reportPath = ReportManager.GetReportPath();
Console.WriteLine($"Report at: {reportPath}");
```

---

## Advanced Customization

### Custom Report Title and Theme

You can modify `ReportManager.InitializeReport()` to customize:

```csharp
public static void InitializeReport()
{
    var htmlReporter = new ExtentHtmlReporter(reportFullPath);
    htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
    htmlReporter.Config.DocumentTitle = "Custom Report Title";
    htmlReporter.Config.ReportName = "Custom Report Name";
}
```

### System Information

Add custom system information:

```csharp
_extentReports.AddSystemInfo("Test Environment", "Staging");
_extentReports.AddSystemInfo("Tester Name", "John Doe");
_extentReports.AddSystemInfo("Build Number", "1.0.0");
```

---

## Summary

Extent Reports integration provides:

✅ Beautiful HTML reports with real-time test execution data  
✅ Automatic screenshot capture on test failure  
✅ Detailed step-by-step test logging  
✅ Professional reporting for stakeholders  
✅ Easy integration with NUnit framework  
✅ CI/CD pipeline friendly  

For more information, visit the [AventStack Extent Reports GitHub](https://github.com/extent-framework/extentreports-dotnet)
