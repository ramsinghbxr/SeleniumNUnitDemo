# ReportPortal Setup on Azure for Selenium Project

This guide walks you through setting up ReportPortal on Azure to integrate with your Selenium NUnit tests.

## Table of Contents
1. [Create ReportPortal on Azure](#create-reportportal-on-azure)
2. [Configure ReportPortal Project](#configure-reportportal-project)
3. [Update Your Selenium Project](#update-your-selenium-project)
4. [Run Tests with ReportPortal Integration](#run-tests-with-reportportal-integration)
5. [View Test Results](#view-test-results)

---

## Step 1: Create ReportPortal on Azure

### Option A: Using Azure App Service (Recommended for Quick Setup)

#### 1.1 Create an Azure Account
- Go to [Azure Portal](https://portal.azure.com)
- Sign in or create a free account (includes $200 credits)

#### 1.2 Create a Resource Group
1. Click **Create a resource**
2. Search for **Resource Group**
3. Click **Create**
4. Fill in:
   - **Subscription**: Select your subscription
   - **Resource group name**: `rp-selenium-rg`
   - **Region**: Choose closest to you (e.g., `East US`)
5. Click **Review + create** → **Create**

#### 1.3 Create an App Service Plan
1. In Resource Group, click **Create a resource**
2. Search for **App Service Plan**
3. Click **Create**
4. Fill in:
   - **Resource Group**: `rp-selenium-rg`
   - **Name**: `rp-selenium-plan`
   - **Operating System**: Linux
   - **Region**: Same as resource group
   - **Sku and size**: Change to **B1** (Basic, ~$15/month)
5. Click **Review + create** → **Create**

#### 1.4 Create an App Service with ReportPortal
1. Go to Resource Group → Click **Create a resource**
2. Search for **Web App**
3. Click **Create**
4. Fill in:
   - **Resource Group**: `rp-selenium-rg`
   - **Name**: `rp-selenium-app` (must be unique - e.g., `rp-selenium-app-yourname`)
   - **Publish**: Docker Container
   - **Operating System**: Linux
   - **App Service Plan**: `rp-selenium-plan`
5. Click **Next: Docker**

#### 1.5 Configure Docker Settings
1. **Image Source**: Docker Hub
2. **Image and Optional Tag**: `reportportal/service-ui:5.8.0`
3. Click **Review + create** → **Create**

#### 1.6 Wait for Deployment
- Wait 2-5 minutes for the deployment to complete
- Go to the App Service resource
- Copy the **URL** (e.g., `https://rp-selenium-app-yourname.azurewebsites.net`)

---

### Option B: Using Docker + Azure Container Instances (For Full Control)

#### 2.1 Create Container Registry
1. Go to Resource Group
2. Search for **Container Registry**
3. Click **Create**
4. Fill in:
   - **Registry name**: `rpseleniumreg` (must be lowercase, unique)
   - **Resource Group**: `rp-selenium-rg`
   - **Sku**: Basic
5. Click **Create**

#### 2.2 Build and Push Docker Image
1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. In your project folder, create a `Dockerfile`:

```dockerfile
FROM reportportal/service-ui:5.8.0
EXPOSE 8080
```

3. Build and push:
```powershell
# Login to Azure Container Registry
$registry = "rpseleniumreg.azurecr.io"
az acr login --name rpseleniumreg

# Build image
docker build -t $registry/reportportal:latest .

# Push to registry
docker push $registry/reportportal:latest
```

#### 2.3 Create Container Instance
1. Go to Resource Group
2. Search for **Container Instances**
3. Click **Create**
4. Fill in:
   - **Container name**: `rp-selenium-container`
   - **Container image source**: Azure Container Registry
   - **Registry**: `rpseleniumreg`
   - **Image**: `reportportal:latest`
   - **OS type**: Linux
   - **Restart policy**: Always
   - **CPU cores**: 1
   - **Memory (GB)**: 1.5
   - **Ports**: 8080
5. Click **Review + create** → **Create**

---

## Step 2: Configure ReportPortal Project

### 2.1 Access ReportPortal UI
1. Open the App Service URL in browser: `https://rp-selenium-app-yourname.azurewebsites.net`
2. Default login:
   - **Email**: `superadmin@reportportal.io`
   - **Password**: `erebus`

### 2.2 Create a New Project
1. Click **Create Project**
2. Fill in:
   - **Project Name**: `selenium-tests`
   - **Project Type**: Choose based on your needs
3. Click **Create**

### 2.3 Get API Token
1. Click your profile icon (top right)
2. Click **API Tokens**
3. Click **Generate Token**
4. Copy the **API Token** (save it securely)

### 2.4 Get Project Details
In your ReportPortal project:
1. Go to **Settings** (gear icon)
2. Copy:
   - **Project UUID** or name: `selenium-tests`
   - **Base URL**: `https://rp-selenium-app-yourname.azurewebsites.net`

---

## Step 3: Update Your Selenium Project

### 3.1 Create reportportal.json
Create a file named `reportportal.json` in your project root:

```json
{
  "rp.endpoint": "https://rp-selenium-app-yourname.azurewebsites.net",
  "rp.project": "selenium-tests",
  "rp.username": "superadmin",
  "rp.apiKey": "YOUR_API_TOKEN_HERE",
  "rp.launch": "Selenium NUnit Tests",
  "rp.description": "Automated Selenium tests"
}
```

**⚠️ Important**: Replace:
- `https://rp-selenium-app-yourname.azurewebsites.net` with your actual App Service URL
- `YOUR_API_TOKEN_HERE` with your copied API token

### 3.2 Update .csproj File
Open `SeleniumNUnitDemo.csproj` and ensure you have:

```xml
<ItemGroup>
    <PackageReference Include="ReportPortal.NUnit" Version="5.1.5" />
</ItemGroup>
```

Run:
```powershell
dotnet add package ReportPortal.NUnit --version 5.1.5
```

### 3.3 Create .runsettings File
Create `.runsettings` file in project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <LoggerRunSettings>
    <Loggers>
      <Logger friendlyName="ReportPortal" uri="logger://ReportPortal.NUnitExtension.ReportPortalLogger" assemblyQualifiedName="ReportPortal.NUnitExtension.ReportPortalLogger, ReportPortal.NUnitExtension" enabled="True" />
    </Loggers>
  </LoggerRunSettings>
</RunSettings>
```

### 3.4 Update SwagLabsTests.cs
Ensure your test class has:

```csharp
[TestFixture]
public class SwagLabsTests
{
    private IWebDriver driver;

    [OneTimeSetUp]
    public void Setup()
    {
        // Your setup code
    }

    [Test]
    public void YourTestName()
    {
        // Your test code
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
```

---

## Step 4: Run Tests with ReportPortal Integration

### 4.1 Run Tests from Command Line
```powershell
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo

# Run all tests
dotnet test --logger "console;verbosity=detailed"

# Or use NUnit directly
nunit3-console.exe SeleniumNUnitDemo.dll --config=Debug
```

### 4.2 Run Tests from Visual Studio
1. Open **Test Explorer** (Test → Test Explorer)
2. Click **Run All Tests**
3. Tests will execute and report to ReportPortal

### 4.3 Monitor Test Execution
1. Open ReportPortal in browser
2. Go to your project: **selenium-tests**
3. Click **Launches**
4. You should see your test run appearing in real-time

---

## Step 5: View Test Results

### 5.1 Check Test Report
1. Go to ReportPortal: `https://rp-selenium-app-yourname.azurewebsites.net`
2. Navigate to **selenium-tests** project
3. Click on the latest **Launch** (e.g., "Selenium NUnit Tests")
4. View:
   - Test results (passed/failed/skipped)
   - Execution time
   - Screenshots and logs
   - Test details and stack traces

### 5.2 Analyze Failures
1. Click on a failed test
2. View:
   - Error message
   - Stack trace
   - Execution logs
   - Screenshots (if captured)

---

## Troubleshooting

### Issue: "401 Unauthorized" or "Invalid API Token"
- Verify API token in `reportportal.json`
- Ensure token hasn't expired (regenerate if needed)
- Check endpoint URL is correct

### Issue: ReportPortal UI Loads Slowly
- Check Azure App Service CPU/memory usage
- Consider upgrading to higher SKU (B2, B3)
- Check network connectivity

### Issue: Tests Not Appearing in ReportPortal
1. Verify `reportportal.json` exists and is valid
2. Check .runsettings file is in project root
3. Rebuild project: `dotnet clean && dotnet build`
4. Run tests with verbose logging
5. Check Azure App Service logs

### Issue: "Project Not Found"
- Verify project name in `reportportal.json` matches ReportPortal
- Create project in ReportPortal UI if missing
- Restart the test run

---

## Cost Optimization

### Azure Pricing
- **App Service (B1)**: ~$15/month
- **Data transfer**: First 5GB free, then ~$0.12/GB
- **Total monthly cost**: ~$15-20 (for low usage)

### Cost-Saving Tips
1. Use **B1 (Basic)** plan for development
2. Use **Free tier** if only testing ReportPortal
3. Stop App Service when not in use (saves 30-50%)
4. Monitor resource usage in Azure Portal

---

## Next Steps

1. ✅ Create Azure resources
2. ✅ Configure ReportPortal
3. ✅ Update your Selenium project
4. ✅ Run first test
5. ✅ View results in ReportPortal
6. 📊 Set up CI/CD integration (Azure Pipelines)
7. 📧 Configure notifications (email on test failure)

---

## Additional Resources

- [ReportPortal Documentation](https://docs.reportportal.io/)
- [ReportPortal NUnit Integration](https://github.com/reportportal/agent-net-nunit)
- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service/)
- [Selenium Documentation](https://www.selenium.dev/documentation/)

---

## Support

For issues or questions:
1. Check ReportPortal logs in Azure App Service → **Log Stream**
2. Review Azure Monitor → **Logs** for application insights
3. Check GitHub issues: [ReportPortal Agent .NET NUnit](https://github.com/reportportal/agent-net-nunit/issues)
