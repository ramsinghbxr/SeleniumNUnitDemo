# ReportPortal Setup Checklist

Complete this checklist to get ReportPortal running with your Selenium tests.

## ✅ Files Created

- [x] `docker-compose.yml` - Docker configuration for all services
- [x] `reportportal.json` - ReportPortal configuration file
- [x] `start-reportportal.ps1` - PowerShell automation script
- [x] `.runsettings` - NUnit test runner configuration

## 📋 Quick Setup Steps

### Step 1: Start ReportPortal (2 minutes)

```powershell
# Navigate to project directory
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo

# Run the automation script
.\start-reportportal.ps1 -Start
```

Wait for "ReportPortal started successfully!" message.

### Step 2: Access ReportPortal UI (1 minute)

Open in your browser:
```
http://localhost:8081
```

Login with:
- **Email**: `superadmin@reportportal.io`
- **Password**: `erebus`

### Step 3: Create a Project (2 minutes)

1. Click **Create Project**
2. **Name**: `selenium-tests`
3. Click **Create**

### Step 4: Get API Token (2 minutes)

1. Click your profile icon (top-right corner)
2. Click **API Tokens**
3. Click **Generate New Token**
4. **Copy the token** (you'll need it next)

### Step 5: Update reportportal.json (1 minute)

Open `reportportal.json` and replace:

```json
"rp.apiKey": "YOUR_API_TOKEN_HERE"
```

With your copied API token. Example:

```json
"rp.apiKey": "ABC123DEF456GHI789"
```

### Step 6: Add NUnit Package (1 minute)

```powershell
dotnet add package ReportPortal.NUnit --version 5.1.5
```

### Step 7: Run Your Tests (2 minutes)

```powershell
dotnet test
```

Or use Visual Studio Test Explorer:
- View → Test Explorer
- Click "Run All Tests"

### Step 8: View Results (1 minute)

1. Open ReportPortal: http://localhost:8081
2. Go to **selenium-tests** project
3. Click **Launches**
4. Click your test run to see results

---

## 🛠️ Useful Commands

### Start ReportPortal
```powershell
.\start-reportportal.ps1 -Start
# or just
.\start-reportportal.ps1
```

### View Live Logs
```powershell
.\start-reportportal.ps1 -Logs
```

### Check Service Status
```powershell
.\start-reportportal.ps1 -Status
```

### Restart Services
```powershell
.\start-reportportal.ps1 -Restart
```

### Stop Services (keeps data)
```powershell
.\start-reportportal.ps1 -Stop
```

### Full Cleanup (deletes all data)
```powershell
.\start-reportportal.ps1 -Clean
```

### Get Help
```powershell
.\start-reportportal.ps1 -Help
```

---

## 🌐 Service URLs

| Service | URL | Notes |
|---------|-----|-------|
| ReportPortal UI | http://localhost:8081 | Main dashboard |
| ReportPortal API | http://localhost:8080 | Backend API |
| RabbitMQ Manager | http://localhost:15672 | Username: `rpadmin`, Password: `rpadminpass` |
| MinIO Console | http://localhost:9001 | Username: `minioadmin`, Password: `minioadmin` |
| PostgreSQL | localhost:5432 | Database backend |
| MongoDB | localhost:27017 | Log storage |

---

## 🔧 Troubleshooting

### Services won't start?
```powershell
# Check Docker is running
docker ps

# View detailed logs
.\start-reportportal.ps1 -Logs

# Restart
.\start-reportportal.ps1 -Restart
```

### Port already in use?
```powershell
# Find what's using the port (e.g., 8080)
netstat -ano | findstr :8080

# Stop services
.\start-reportportal.ps1 -Stop

# Clean up and restart
.\start-reportportal.ps1 -Start
```

### Tests not reporting?
1. Verify `reportportal.json` exists in project root
2. Check API token is correct in `reportportal.json`
3. Verify `.runsettings` exists in project root
4. Rebuild: `dotnet clean && dotnet build`
5. Run with verbose: `dotnet test -v d`

### Memory issues?
- Increase Docker memory in Docker Desktop settings
- File → Preferences → Resources → Memory: Set to 4GB+

---

## 📚 File Descriptions

### docker-compose.yml
- Defines all Docker containers (PostgreSQL, MongoDB, RabbitMQ, MinIO, ReportPortal API, ReportPortal UI)
- Automatically creates volumes for data persistence
- Includes health checks for service readiness

### reportportal.json
- Configuration file for ReportPortal integration
- Specifies endpoint, project, and authentication
- Used by NUnit extension to connect to ReportPortal

### start-reportportal.ps1
- PowerShell automation script for Docker management
- Provides easy commands to start/stop/monitor services
- Includes comprehensive help and troubleshooting

### .runsettings
- NUnit test runner configuration
- Enables ReportPortal logger integration
- Required for tests to report to ReportPortal

---

## 🎯 Next Steps

After completing the setup:

1. **Explore ReportPortal features**:
   - Custom attributes
   - Test history
   - Defect tracking
   - Analytics

2. **Enhance your tests**:
   - Add screenshots on failure
   - Log custom messages
   - Track test metrics

3. **Integrate with CI/CD**:
   - Azure Pipelines
   - GitHub Actions
   - Jenkins

4. **Share results**:
   - Team dashboard
   - Email reports
   - Slack notifications

---

## ❓ Questions?

- **ReportPortal Docs**: https://docs.reportportal.io/
- **NUnit Integration**: https://github.com/reportportal/agent-net-nunit
- **Docker Docs**: https://docs.docker.com/
- **Selenium Docs**: https://www.selenium.dev/documentation/

---

**Total Setup Time**: ~15 minutes for complete integration

**Cost**: $0 (completely free)

**Happy Testing! 🚀**
