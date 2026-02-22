# --------------------------------------------------------------------------------
# AkkaSync Windows Service Deployment Script
# --------------------------------------------------------------------------------

function Remove-ServiceSafe {
    param (
        [string]$ServiceName,
        [int]$TimeoutSeconds = 20
    )

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($service) {
        Write-Host "Stopping service '$ServiceName'..."
        if ($service.Status -ne 'Stopped') {
            Stop-Service -Name $ServiceName -Force
        }

        Write-Host "Deleting service '$ServiceName'..."
        sc.exe delete $ServiceName | Out-Null

        Write-Host "Waiting for service to be fully removed..."

        $elapsed = 0
        while ($elapsed -lt $TimeoutSeconds) {
            $exists = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
            if (-not $exists) {
                Write-Host "Service successfully removed."
                return
            }

            Start-Sleep -Seconds 1
            $elapsed++
        }

        throw "Service '$ServiceName' was not removed within timeout."
    }
}

# ===== Configuration =====
$projectPath      = "C:\Development\AkkaSync\src\AkkaSync.Host"
$publishPath      = "C:\Publishment\AkkaSync"
$serviceName      = "AkkaSync"
$runtime          = "win-x64"
$configuration    = "Release"

# ===== 1️⃣ Stop existing Service =====
Remove-ServiceSafe -ServiceName $serviceName

# ===== 2️⃣ Clean publish directory BEFORE build =====
if (Test-Path $publishPath) {
    Write-Host "Cleaning publish directory..."
    Get-ChildItem $publishPath -Recurse | Remove-Item -Recurse -Force
} else {
    Write-Host "Publish directory does not exist. Creating..."
    New-Item -ItemType Directory -Path $publishPath | Out-Null
}

# ===== 3️⃣ Build & publish =====
Write-Host "Publishing AkkaSync.Host..."
dotnet publish $projectPath -c $configuration -r $runtime --self-contained true -o $publishPath

# ===== 4️⃣ Create Service if not exists =====
if (-not $service) {
    Write-Host "Creating Windows Service '$serviceName'..."
    sc.exe create $serviceName binPath= "`"$publishPath\AkkaSync.Host.exe`"" start= auto

    # Wait until service appears
    $maxWait = 15
    $elapsed = 0
    while (-not (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) -and $elapsed -lt $maxWait) {
    Write-Host "Waiting for service registration..."
    Start-Sleep -Seconds 1
    $elapsed++
}

$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
if (-not $service) {
    Write-Error "Service '$serviceName' did not register in time."
    exit 1
}
}

# ===== 5️⃣ Start the Service =====
Write-Host "Starting Windows Service '$serviceName'..."
Start-Service -Name $serviceName

Write-Host "Deployment complete. Service is running."