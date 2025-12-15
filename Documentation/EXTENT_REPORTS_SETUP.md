# Extent Reports - Quick Setup Guide

## Installation (2 Steps)

### Step 1: Install NuGet Package
```bash
dotnet add package AventStack.ExtentReports
```

### Step 2: Copy Utility Files
- `Utilities/ReportManager.cs` (Already provided)

---

## Basic Usage (3 Steps)

### Step 1: Initialize Report in OneTimeSetUp
```csharp
[OneTimeSetUp]
public void OneTimeSetup()
{
    ReportManager.InitializeReport();
    ReportManager.LogInfo("Test Suite Started");
}
```

### Step 2: Create Test and Log Steps
```csharp
[Test]
public void YourTest()
{
    ReportManager.CreateTest("Test Name", "Description");
    
    try
    {
        ReportManager.LogInfo("Step 1: Action");
        // ... perform action
        ReportManager.LogPass("Step completed");
        
        ReportManager.MarkTestPass();
    }
    catch (Exception ex)
    {
        ReportManager.LogError(ex.Message);
        throw;
    }
}
```

### Step 3: Flush Report in OneTimeTearDown
```csharp
[OneTimeTearDown]
public void OneTimeTearDown()
{
    ReportManager.FlushReport();
    Console.WriteLine($"Report: {ReportManager.GetReportPath()}");
}
```

---

## Handle Failures with Screenshots

### In TearDown Method
```csharp
[TearDown]
public void TearDown()
{
    if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
    {
        string screenshot = _page.TakeScreenshot($"Failure_{TestContext.CurrentContext.Test.Name}");
        ReportManager.LogFail("Test failed", screenshot);
    }
    _driver?.Quit();
}
```

---

## Run Tests

```bash
# Run all tests
dotnet test

# Tests with reports will be in:
# bin/Debug/net7.0/ExtentReports/TestReport_*.html
```

---

## Report Output

✅ **Dashboard** - Overview of test results  
✅ **Pass/Fail/Skip** - Test status and counts  
✅ **Screenshots** - Embedded on failure  
✅ **Step Logs** - Detailed test execution  
✅ **Duration** - Execution time  
✅ **System Info** - Environment details  

---

## Key Methods

| Method | Purpose |
|--------|---------|
| `InitializeReport()` | Initialize report before tests |
| `CreateTest(name, desc)` | Create a test entry |
| `LogInfo(msg)` | Log informational message |
| `LogPass(msg)` | Log passing step |
| `LogFail(msg, screenshot)` | Log failure with screenshot |
| `LogError(msg)` | Log error message |
| `AddScreenshot(path)` | Add screenshot to report |
| `MarkTestPass()` | Mark test as passed |
| `MarkTestFail(reason)` | Mark test as failed |
| `FlushReport()` | Finalize and save report |

---

## Example Test Class

See `SwagLabsTests_WithReports.cs` for complete implementation with:

- ✅ Report initialization
- ✅ Login tests with logging
- ✅ Shopping flow tests
- ✅ Checkout tests
- ✅ Logout tests
- ✅ Automatic screenshot on failure
- ✅ Detailed step logging

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Report not generated | Call `FlushReport()` in `OneTimeTearDown` |
| Screenshots missing | Ensure `TakeScreenshot()` returns file path |
| Null reference error | Call `InitializeReport()` first |
| Report file not found | Check: `bin/Debug/net7.0/ExtentReports/` |

---

## Next Steps

1. Run `dotnet build` to verify setup
2. Run `dotnet test` to execute tests
3. Open generated HTML report in browser
4. Review test execution details and screenshots
5. Share report with stakeholders

Enjoy beautiful test reports! 🎉
