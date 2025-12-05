# 🚀 ReportPortal Integration - Quick Start Guide

Welcome! This guide will help you get ReportPortal up and running with your Selenium tests in **less than 15 minutes**.

## 📦 What You Get

✅ **Complete Free Setup** (Zero Cost)  
✅ **Local ReportPortal** on Docker  
✅ **Automated Script** to manage services  
✅ **Full NUnit Integration**  
✅ **Test Reporting Dashboard**  

---

## ⚡ Quick Start (5 Steps)

### Step 1️⃣: Start ReportPortal

```powershell
cd C:\mywork\iitp\SeleniumProject\SeleniumNUnitDemo
.\start-reportportal.ps1
```

Wait for the success message. ✅

### Step 2️⃣: Open ReportPortal

Open in browser: **http://localhost:8081**

Login:
- Email: `superadmin@reportportal.io`
- Password: `erebus`

### Step 3️⃣: Create Project

1. Click **Create Project**
2. Name: `selenium-tests`
3. Click **Create**

### Step 4️⃣: Get API Token

1. Click your profile (top-right)
2. **API Tokens**
3. **Generate Token**
4. **Copy** the token

### Step 5️⃣: Update reportportal.json

Edit `reportportal.json` in your project root:

```json
"rp.apiKey": "PASTE_YOUR_TOKEN_HERE"
```

---

## ✅ Installation Complete!

Now run your tests:

```powershell
dotnet test
```

Then check results in ReportPortal: **http://localhost:8081** 📊

---

## 📚 Documentation Files

Your project now includes:

| File | Purpose |
|------|---------|
| `SETUP_CHECKLIST.md` | ✅ Step-by-step setup guide |
| `REPORTPORTAL_FREE_SETUP.md` | 💰 Free setup options (Local Docker + Cloud) |
| `REPORTPORTAL_AZURE_SETUP.md` | ☁️ Azure deployment guide |
| `docker-compose.yml` | 🐳 Docker configuration |
| `reportportal.json` | ⚙️ ReportPortal config |
| `start-reportportal.ps1` | 🛠️ Automation script |
| `.runsettings` | 🧪 NUnit configuration |

---

## 🎯 Useful Commands

### Start Services
```powershell
.\start-reportportal.ps1 -Start
```

### View Live Logs
```powershell
.\start-reportportal.ps1 -Logs
```

### Check Status
```powershell
.\start-reportportal.ps1 -Status
```

### Stop Services
```powershell
.\start-reportportal.ps1 -Stop
```

### Get Help
```powershell
.\start-reportportal.ps1 -Help
```

---

## 🌐 Service URLs

| Service | URL |
|---------|-----|
| 🎯 ReportPortal | http://localhost:8081 |
| ⚙️ API | http://localhost:8080 |
| 🐰 RabbitMQ | http://localhost:15672 |
| 🪣 MinIO | http://localhost:9001 |

---

## 🔧 Troubleshooting

### Services won't start?
```powershell
# Check Docker
docker ps

# View logs
.\start-reportportal.ps1 -Logs

# Restart
.\start-reportportal.ps1 -Restart
```

### Tests not reporting?
1. Check `reportportal.json` is in project root
2. Verify API token is correct
3. Check `.runsettings` exists
4. Rebuild: `dotnet clean && dotnet build`

### Port conflicts?
```powershell
# Stop services
.\start-reportportal.ps1 -Stop

# Clean and restart
.\start-reportportal.ps1 -Start
```

---

## 📊 Next Steps

1. **Run your first test** → `dotnet test`
2. **View results** → http://localhost:8081
3. **Explore features** → Custom attributes, history, analytics
4. **Share with team** → Set up CI/CD integration
5. **Enhance tests** → Add screenshots, logs, metrics

---

## ❓ Need Help?

Check these resources:

- **Full Setup Guide**: `SETUP_CHECKLIST.md`
- **Free Setup Options**: `REPORTPORTAL_FREE_SETUP.md`
- **Azure Deployment**: `REPORTPORTAL_AZURE_SETUP.md`
- **ReportPortal Docs**: https://docs.reportportal.io/
- **NUnit Integration**: https://github.com/reportportal/agent-net-nunit

---

## 🎉 Ready to Go!

Your ReportPortal setup is complete. Start your journey with test automation insights!

**Happy Testing!** 🚀
