# Simple Installer for TempleAccounting
$appName = "ระบบบัญชีวัด (TempleAccounting)"
$targetDir = Join-Path $env:ProgramFiles $appName
$publishDir = Join-Path $PSScriptRoot "bin\Release\publish"
$iconPath = Join-Path $targetDir "app_icon.ico"
$exePath = Join-Path $targetDir "TempleAccounting.exe"

Write-Host "Installing $appName..." -ForegroundColor Cyan

# Create directory
if (!(Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force
}

# Copy files
Copy-Item -Path "$publishDir\*" -Destination $targetDir -Recurse -Force

# Create Desktop Shortcut
$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut([System.IO.Path]::Combine([System.Environment]::GetFolderPath("Desktop"), "$appName.lnk"))
$Shortcut.TargetPath = $exePath
$Shortcut.WorkingDirectory = $targetDir
$Shortcut.IconLocation = $iconPath
$Shortcut.Save()

Write-Host "Installation Complete!" -ForegroundColor Green
Write-Host "Shortcut created on Desktop."
