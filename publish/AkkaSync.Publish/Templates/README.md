# 📦 AkkaSync Demo (Docker)

This package contains everything you need to run an **AkkaSync demo** locally using Docker.

No build required.  
No code required.  
Just Docker.

---

## 🚀 Prerequisites

Make sure you have the following installed:

- **Docker Desktop**  
  https://www.docker.com/products/docker-desktop/
- **Docker Compose v2**  
  (Usually included with Docker Desktop)

Verify installation:

```bash
docker --version
docker compose version
```


## 📂 Package Contents

After extracting the release archive, you should see:

```text
.
├── docker-compose.yml
├── .env
├── pipelines/
├── data/
└── README.md
```

### What’s included?

| Item | Description |
|------|-------------|
| `docker-compose.yml` | Starts AkkaSync Host & Dashboard |
| `.env` | Defines the Docker image version |
| `pipelines/` | Demo pipelines configuration files |
| `data/` | Demo data used by pipelines |



## ▶️ Run the Demo

From this directory, run:

```bash
docker compose up
```

Docker will:

1. Pull AkkaSync images from **GitHub Container Registry**  
   (version: assets-0.1.0-mvp.1)

2. Start:  
   - **AkkaSync Host** (API on port 5000)  
   - **AkkaSync Dashboard** (UI on port 3000)

3. Load demo configuration and data automatically

⏳ First run may take a few minutes due to image download.

## 🌐 Access the Dashboard

Once started, open your browser:
```bash
http://localhost:3000
```


You should see the **AkkaSync Dashboard** running with demo pipelines and data.

## 🧪 What This Demo Shows

- AkkaSync pipeline scheduling

- Data ingestion from demo sources

- Pipeline lifecycle events

- Real-time status visible in the dashboard

- Hot-pluggable plugins: move/replace DLLs between plugins(active) and plugins-swap **in docker** to trigger runtime reloads.

This demo runs in **demo** mode, using bundled configuration and test data.

## 🛑 Stop the Demo

Press Ctrl + C in the terminal, or run:

```bash
docker compose down
```

## 🔁 Run Again

To restart with clean containers:
```bash
docker compose up
```

To also remove volumes (if any):
```bash
docker compose down -v
```

## 🧹 Troubleshooting
### Docker images cannot be pulled

If you see permission errors pulling images from ghcr.io:

- Ensure Docker is logged in to GitHub Container Registry

- For public images, login is usually not required

Optional login:
```bash
docker login ghcr.io
```
---
### Port already in use

If ports are already occupied:

- Dashboard: 3000

- Host API: 5000

Stop conflicting services or edit docker-compose.yml.

## 👀 Viewing Data with SQLite Clients

### Using SqliteStudio

SqliteStudio is a user-friendly GUI tool for browsing and managing SQLite databases:

1. **Download and Install**  
   https://sqlitestudio.pl/

2. **Locate the Database File**  
   - The demo creates a SQLite database in the `data/output/` directory
   - Look for `.db` files (e.g., `demo.db`)

3. **Connect in SqliteStudio**
   - Open SqliteStudio
   - Click **File** → **Add database** (or use the connection icon)
   - Select the `.db` file from your demo package's `data/output/` directory
   - Browse tables, views, and data

### Alternative SQLite Clients

You can use any of these tools to view the data:

| Tool | Platform | URL |
|------|----------|-----|
| **SqliteStudio** | Windows, macOS, Linux | https://sqlitestudio.pl/ |
| **DBeaver** | Windows, macOS, Linux | https://dbeaver.io/ |
| **TablePlus** | macOS, Windows | https://tableplus.com/ |

### Command Line Option

You can also use the SQLite CLI:

```bash
sqlite3 data/output/demo.db
```

Then use standard SQL queries:
```sql
.tables                    -- List all tables
SELECT * FROM table_name;  -- View data
.schema table_name         -- View table structure
```

## �📖 Next Steps

- Explore the configuration files in config/

- Modify demo data in data/

- Check out the source code:
  
  👉 https://github.com/oninebx/AkkaSync

## 🧠 About This Release

This demo package was generated using **AkkaSync.Publish**,
an automated publishing tool that ensures:

- Reproducible releases

- Version-aligned Docker images

- Zero manual setup
---
Happy syncing 🚀


