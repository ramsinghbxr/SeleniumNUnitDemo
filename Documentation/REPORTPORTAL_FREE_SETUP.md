# ReportPortal Free Setup Guide for Selenium Project (Zero Cost)

This guide walks you through setting up ReportPortal completely **FREE** using:
- **Free GitHub Account** (for cloud hosting)
- **Free Render.com** (serverless deployment)
- **Docker Hub** (free private repos with free tier)
- **Local Docker** (completely free, runs on your machine)

---

## Table of Contents
1. [Option A: Local Docker (Easiest, Completely Free)](#option-a-local-docker-easiest-completely-free)
2. [Option B: Free Cloud on Render.com](#option-b-free-cloud-on-rendercom)
3. [Configure Your Selenium Project](#configure-your-selenium-project)
4. [Run Tests with ReportPortal](#run-tests-with-reportportal)
5. [View Results](#view-results)

---

## Option A: Local Docker (Easiest, Completely Free)

### Prerequisites
- Install [Docker Desktop](https://www.docker.com/products/docker-desktop) (Free)
- Windows 10/11, macOS, or Linux

### Step 1: Create docker-compose.yml

Create a file named `docker-compose.yml` in your project root:

```yaml
version: '3.8'

services:
  # PostgreSQL Database
  postgres:
    image: postgres:13-alpine
    container_name: reportportal-postgres
    restart: always
    environment:
      POSTGRES_DB: reportportal
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  # MongoDB for logs
  mongodb:
    image: mongo:5.0
    container_name: reportportal-mongodb
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db

  # RabbitMQ for messaging
  rabbitmq:
    image: rabbitmq:3.8-management-alpine
    container_name: reportportal-rabbitmq
    restart: always
    environment:
      RABBITMQ_DEFAULT_USER: rpadmin
      RABBITMQ_DEFAULT_PASS: rpadminpass
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq

  # MinIO for file storage
  minio:
    image: minio/minio:latest
    container_name: reportportal-minio
    restart: always
    command: server /minio_data --console-address ":9001"
    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin
    ports:
      - "9000:9000"
      - "9001:9001"
    volumes:
      - minio_data:/minio_data

  # ReportPortal API
  api:
    image: reportportal/service-api:5.8.0
    container_name: reportportal-api
    restart: always
    depends_on:
      postgres:
        condition: service_started
      mongodb:
        condition: service_started
      rabbitmq:
        condition: service_started
      minio:
        condition: service_started
    environment:
      RP_DB_HOST: postgres
      RP_DB_USER: postgres
      RP_DB_PASS: postgres
      RP_BINARYSTORE_TYPE: minio
      RP_BINARYSTORE_MINIO_ENDPOINT: http://minio:9000
      RP_BINARYSTORE_MINIO_ACCESSKEY: minioadmin
      RP_BINARYSTORE_MINIO_SECRETKEY: minioadmin
      RP_AMQP_HOST: rabbitmq
      RP_AMQP_USER: rpadmin
      RP_AMQP_PASS: rpadminpass
      RP_AMQP_API_ADDRESS: http://rabbitmq:15672
      RP_AMQP_API_USER: rpadmin
      RP_AMQP_API_PASSWORD: rpadminpass
      RP_MONGO_DB: reportportal
      RP_MONGO_HOST: mongodb
    ports:
      - "8080:8080"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 5

  # ReportPortal UI
  ui:
    image: reportportal/service-ui:5.8.0
    container_name: reportportal-ui
    restart: always
    depends_on:
      api:
        condition: service_started
    environment:
      RP_API_URL: http://api:8080
    ports:
      - "8081:8080"

volumes:
  postgres_data:
  mongodb_data:
  rabbitmq_data:
  minio_data:
```

### Step 2: Start ReportPortal Locally

Open PowerShell and run:

```powershell
# Navigate to your project directory
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo

# Start all containers
docker-compose up -d

# Wait 30 seconds for startup
Start-Sleep -Seconds 30

# Check if all services are running
docker-compose ps

# View API logs (to verify startup)
docker-compose logs api
```

### Step 3: Access ReportPortal

Open your browser:
- **ReportPortal UI**: http://localhost:8081
- **ReportPortal API**: http://localhost:8080
- **RabbitMQ Management**: http://localhost:15672 (username: `rpadmin`, password: `rpadminpass`)

**Default Login**:
- Email: `superadmin@reportportal.io`
- Password: `erebus`

### Step 4: Create a Project

1. Click **Create Project**
2. Name: `selenium-tests`
3. Click **Create**

### Step 5: Get API Token

1. Click your profile icon (top right)
2. Click **API Tokens**
3. Click **Generate New Token**
4. Copy the token and save it

### Step 6: Stop Services (When Done)

```powershell
# Stop all containers
docker-compose down

# Remove volumes (optional, to clean up data)
docker-compose down -v
```

---

## Option B: Free Cloud on Render.com

### Why Render.com?
- **Free tier**: Includes 750 free hours per month (enough for 24/7 deployment!)
- **Auto-deploys** from GitHub
- **No credit card** required
- **PostgreSQL** included for free
- **Perfect for testing**

### Step 1: Create GitHub Repository

1. Go to [GitHub](https://github.com)
2. Create a new public repository named `reportportal-deployment`
3. Clone it locally

### Step 2: Create Dockerfile and Files

In your GitHub repo, create:

**Dockerfile**:
```dockerfile
FROM reportportal/service-api:5.8.0
EXPOSE 8080
```

**docker-compose.yml** (same as Option A above)

**render.yaml**:
```yaml
services:
  - type: web
    name: reportportal-api
    env: docker
    plan: free
    healthCheckPath: /health
    envVars:
      - key: RP_DB_HOST
        value: dpg-xxx.render.internal  # Get from Render PostgreSQL
      - key: RP_DB_USER
        value: postgres
      - key: RP_DB_PASS
        fromDatabase:
          name: reportportal_db
          property: password
```

### Step 3: Deploy on Render.com

1. Sign up at [Render.com](https://render.com) (free, no credit card)
2. Click **New** → **Web Service**
3. Connect your GitHub repository
4. Select free plan
5. Deploy

⚠️ **Note**: Render free tier may sleep after 15 minutes of inactivity. For continuous testing, use Option A.

---

## Configure Your Selenium Project

### Step 1: Create reportportal.json

Create `reportportal.json` in your project root:

```json
{
  "rp.endpoint": "http://localhost:8080",
  "rp.project": "selenium-tests",
  "rp.username": "superadmin",
  "rp.apiKey": "YOUR_API_TOKEN_HERE",
  "rp.launch": "Selenium NUnit Tests",
  "rp.description": "Automated Selenium tests with NUnit",
  "rp.attributes": [
    "selenium",
    "nunit",
    "automation"
  ]
}
```

**Important**: Replace `YOUR_API_TOKEN_HERE` with your copied API token.

### Step 2: Add NUnit Package

```powershell
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo

# Add ReportPortal NUnit integration
dotnet add package ReportPortal.NUnit --version 5.1.5
```

### Step 3: Create .runsettings File

Create `.runsettings` in project root:

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

### Step 4: Update Your Test File

Open `SwagLabsTests.cs` and ensure structure:

```csharp
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

[TestFixture]
public class SwagLabsTests
{
    private IWebDriver driver;

    [OneTimeSetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://www.saucedemo.com");
    }

    [Test]
    public void TestLoginWithValidCredentials()
    {
        var usernameField = driver.FindElement(By.Id("user-name"));
        usernameField.SendKeys("standard_user");
        
        var passwordField = driver.FindElement(By.Id("password"));
        passwordField.SendKeys("secret_sauce");
        
        var loginButton = driver.FindElement(By.Id("login-button"));
        loginButton.Click();
        
        // Assert successful login
        Assert.That(driver.Url, Contains.Substring("inventory"));
    }

    [Test]
    public void TestAddProductToCart()
    {
        driver.Navigate().GoToUrl("https://www.saucedemo.com/inventory.html");
        
        var addToCartButton = driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack"));
        addToCartButton.Click();
        
        var cartBadge = driver.FindElement(By.ClassName("shopping_cart_badge"));
        Assert.That(cartBadge.Text, Is.EqualTo("1"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
```

---

## Run Tests with ReportPortal

### Step 1: Start ReportPortal (if using local Docker)

```powershell
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo
docker-compose up -d
Start-Sleep -Seconds 30
```

### Step 2: Run Tests

```powershell
# Run all tests
dotnet test --logger "console;verbosity=detailed"

# Or run specific test
dotnet test --filter TestLoginWithValidCredentials
```

### Step 3: Monitor Execution

Open http://localhost:8081 in browser and watch tests run in real-time.

---

## View Results

### In ReportPortal UI

1. Open http://localhost:8081
2. Go to **selenium-tests** project
3. Click **Launches**
4. View your latest test run
5. Click on individual tests to see:
   - Test status (passed/failed)
   - Execution time
   - Logs
   - Screenshots (if captured)

### Example: Add Screenshots to Tests

```csharp
[Test]
public void TestWithScreenshot()
{
    // Your test code
    
    // Take screenshot on failure
    if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
    {
        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        screenshot.SaveAsFile("screenshot.png", ScreenshotImageFormat.Png);
    }
}
```

---

## Common Commands

### Docker Management
```powershell
# Start services
docker-compose up -d

# Stop services (keeps data)
docker-compose down

# Delete everything (clean slate)
docker-compose down -v

# View logs
docker-compose logs -f api

# View specific service
docker-compose logs rabbitmq
```

### Testing
```powershell
# Run all tests
dotnet test

# Run with verbosity
dotnet test -v d

# Run and stop on first failure
dotnet test --fail-fast
```

---

## Troubleshooting (Free Setup)

### Issue: Services won't start
```powershell
# Check Docker is running
docker ps

# Check port conflicts
netstat -ano | findstr :8080

# View detailed logs
docker-compose logs --tail=50
```

### Issue: API not accessible
```powershell
# Restart API only
docker-compose restart api

# Wait and check health
Start-Sleep -Seconds 10
docker-compose ps
```

### Issue: Tests not reporting
1. Verify `reportportal.json` exists and is readable
2. Check API token is correct in `reportportal.json`
3. Check `.runsettings` is in project root
4. Rebuild: `dotnet clean && dotnet build`
5. Run with verbose: `dotnet test -v d`

### Issue: Out of memory on Docker
- Increase Docker memory in Docker Desktop settings
- Stop other containers: `docker-compose down`
- Restart Docker Desktop

---

## Cost Breakdown (Option A: Local Docker)

| Component | Cost |
|-----------|------|
| Docker Desktop | **FREE** |
| PostgreSQL | **FREE** (included) |
| MongoDB | **FREE** (included) |
| RabbitMQ | **FREE** (included) |
| MinIO | **FREE** (included) |
| ReportPortal | **FREE** (open source) |
| **Total Monthly Cost** | **$0** |

---

## Cost Breakdown (Option B: Render.com)

| Component | Cost |
|-----------|------|
| Render Web Service | **FREE** (750 hrs/month) |
| Render PostgreSQL | **FREE** (256MB) |
| Bandwidth | **FREE** (100GB/month) |
| **Total Monthly Cost** | **$0** |

⚠️ **Note**: Free tier may sleep. Keep-alive checks needed.

---

## Next Steps

1. ✅ Install Docker Desktop
2. ✅ Create docker-compose.yml
3. ✅ Start ReportPortal locally
4. ✅ Create project
5. ✅ Get API token
6. ✅ Update Selenium project files
7. ✅ Run first test
8. ✅ View results in ReportPortal
9. 📊 Explore ReportPortal features:
   - Custom attributes
   - Test history
   - Defect tracking
   - Analytics

---

## Advanced: Keep Local ReportPortal Running Always

Create a PowerShell script `start-reportportal.ps1`:

```powershell
param(
    [switch]$Stop,
    [switch]$Logs
)

$projectPath = "C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo"

if ($Stop) {
    Write-Host "Stopping ReportPortal..." -ForegroundColor Yellow
    docker-compose -f "$projectPath\docker-compose.yml" down
    exit
}

if ($Logs) {
    Write-Host "Showing ReportPortal logs..." -ForegroundColor Cyan
    docker-compose -f "$projectPath\docker-compose.yml" logs -f
    exit
}

Write-Host "Starting ReportPortal..." -ForegroundColor Green
docker-compose -f "$projectPath\docker-compose.yml" up -d

Write-Host "Waiting for services to start..." -ForegroundColor Cyan
Start-Sleep -Seconds 30

Write-Host "Checking services..." -ForegroundColor Cyan
docker-compose -f "$projectPath\docker-compose.yml" ps

Write-Host "`nReportPortal is ready!" -ForegroundColor Green
Write-Host "Access at: http://localhost:8081" -ForegroundColor Cyan
Write-Host "API at: http://localhost:8080" -ForegroundColor Cyan
Write-Host "`nStop with: .\start-reportportal.ps1 -Stop" -ForegroundColor Yellow
```

Usage:
```powershell
# Start
.\start-reportportal.ps1

# View logs
.\start-reportportal.ps1 -Logs

# Stop
.\start-reportportal.ps1 -Stop
```

---

## Resources

- [Docker Documentation](https://docs.docker.com/)
- [ReportPortal Documentation](https://docs.reportportal.io/)
- [ReportPortal NUnit Agent](https://github.com/reportportal/agent-net-nunit)
- [Render.com Documentation](https://render.com/docs)

---

**✅ That's it! You now have a completely free ReportPortal setup!**
