using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;

namespace SeleniumNUnitDemo.Utilities;

/// <summary>
/// Extent Reports Manager - Handles report generation and management
/// Generates HTML reports with screenshots on test failure
/// </summary>
public class ReportManager
{
    private static ExtentReports? _extentReports;
    private static ExtentTest? _extentTest;
    private static readonly string ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExtentReports");
    private static readonly string ReportFileName = $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html";

    /// <summary>
    /// Initialize Extent Reports
    /// </summary>
    public static void InitializeReport()
    {
        try
        {
            // Create reports directory if it doesn't exist
            if (!Directory.Exists(ReportPath))
            {
                Directory.CreateDirectory(ReportPath);
            }

            string reportFullPath = Path.Combine(ReportPath, ReportFileName);
            
            // Initialize ExtentHtmlReporter
            var htmlReporter = new ExtentHtmlReporter(reportFullPath);
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            htmlReporter.Config.DocumentTitle = "Selenium NUnit Test Report";
            htmlReporter.Config.ReportName = "Test Execution Report";

            // Initialize ExtentReports
            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(htmlReporter);

            // Add system information
            _extentReports.AddSystemInfo("OS", Environment.OSVersion.ToString());
            _extentReports.AddSystemInfo("Environment", "QA");
            _extentReports.AddSystemInfo("Browser", "Chrome");
            _extentReports.AddSystemInfo("Framework", ".NET 7.0");
            _extentReports.AddSystemInfo("Execution Date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            Console.WriteLine($"✓ Extent Report initialized at: {reportFullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error initializing Extent Report: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a test in the report
    /// </summary>
    public static void CreateTest(string testName, string description = "")
    {
        if (_extentReports != null)
        {
            _extentTest = _extentReports.CreateTest(testName, description);
        }
    }

    /// <summary>
    /// Log info message to report
    /// </summary>
    public static void LogInfo(string message)
    {
        _extentTest?.Log(Status.Info, message);
    }

    /// <summary>
    /// Log pass message to report
    /// </summary>
    public static void LogPass(string message)
    {
        _extentTest?.Log(Status.Pass, message);
    }

    /// <summary>
    /// Log fail message with screenshot
    /// </summary>
    public static void LogFail(string message, string screenshotPath = "")
    {
        if (_extentTest != null)
        {
            _extentTest.Log(Status.Fail, message);

            // Attach screenshot if provided
            if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
            {
                try
                {
                    _extentTest.AddScreenCaptureFromPath(screenshotPath, "Failure Screenshot");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error attaching screenshot: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Log warning message to report
    /// </summary>
    public static void LogWarning(string message)
    {
        _extentTest?.Log(Status.Warning, message);
    }

    /// <summary>
    /// Log error message to report
    /// </summary>
    public static void LogError(string message)
    {
        _extentTest?.Log(Status.Error, message);
    }

    /// <summary>
    /// Add screenshot to report
    /// </summary>
    public static void AddScreenshot(string screenshotPath, string title = "Screenshot")
    {
        if (_extentTest != null && File.Exists(screenshotPath))
        {
            try
            {
                _extentTest.AddScreenCaptureFromPath(screenshotPath, title);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error adding screenshot: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Mark test as pass
    /// </summary>
    public static void MarkTestPass()
    {
        _extentTest?.Log(Status.Pass, "Test Passed Successfully");
    }

    /// <summary>
    /// Mark test as fail
    /// </summary>
    public static void MarkTestFail(string reason = "Test Failed")
    {
        if (_extentTest != null)
        {
            _extentTest.Log(Status.Fail, reason);
            _extentTest.Fail(reason);
        }
    }

    /// <summary>
    /// Mark test as skip
    /// </summary>
    public static void MarkTestSkip(string reason = "Test Skipped")
    {
        _extentTest?.Log(Status.Skip, reason);
    }

    /// <summary>
    /// Flush and close the report
    /// </summary>
    public static void FlushReport()
    {
        try
        {
            if (_extentReports != null)
            {
                _extentReports.Flush();
                Console.WriteLine("✓ Extent Report generated successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error flushing report: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current report path
    /// </summary>
    public static string GetReportPath()
    {
        return Path.Combine(ReportPath, ReportFileName);
    }

    /// <summary>
    /// Get reports directory path
    /// </summary>
    public static string GetReportsDirectory()
    {
        return ReportPath;
    }
}
