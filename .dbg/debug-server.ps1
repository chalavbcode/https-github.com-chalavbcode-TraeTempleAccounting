$ErrorActionPreference = "Stop"

$sessionId = "transactions-grid-empty"
$outDir = Join-Path (Get-Location) ".dbg"
$portStart = 7777
$maxTries = 10
$idleSeconds = 1200
$clean = $true

if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }

$logFile = Join-Path $outDir ("trae-debug-log-" + $sessionId + ".ndjson")
$envFile = Join-Path $outDir ($sessionId + ".env")

if ($clean -and (Test-Path $logFile)) { Clear-Content -Path $logFile }
if (-not (Test-Path $logFile)) { New-Item -ItemType File -Path $logFile | Out-Null }

$listener = New-Object System.Net.HttpListener
$selectedPort = $null
for ($i = 0; $i -le $maxTries; $i++) {
    $p = $portStart + $i
    try {
        $listener.Prefixes.Clear()
        $listener.Prefixes.Add(("http://127.0.0.1:" + $p + "/"))
        $listener.Start()
        $selectedPort = $p
        break
    } catch {
        try { $listener.Stop() } catch {}
    }
}

if ($null -eq $selectedPort) { throw "Cannot bind port" }

$apiUrl = "http://127.0.0.1:$selectedPort/event"
"DEBUG_SERVER_URL=$apiUrl`nDEBUG_SESSION_ID=$sessionId`n" | Set-Content -Path $envFile -Encoding UTF8

$logDirAbs = (Resolve-Path $outDir).Path
$logFileAbs = (Resolve-Path $logFile).Path
$envFileAbs = (Resolve-Path $envFile).Path

Write-Output "@@DEBUG_SERVER_INFO"
Write-Output "{"
Write-Output "  `"api_url`": `"$apiUrl`","
Write-Output "  `"session_id`": `"$sessionId`","
Write-Output "  `"log_dir`": `"$logDirAbs`","
Write-Output "  `"log_file`": `"$logFileAbs`","
Write-Output "  `"env_file`": `"$envFileAbs`""
Write-Output "}"
Write-Output "@@END_DEBUG_SERVER_INFO"

$last = Get-Date
while ($true) {
    if ($idleSeconds -gt 0) {
        $elapsed = (New-TimeSpan -Start $last -End (Get-Date)).TotalSeconds
        if ($elapsed -ge $idleSeconds) { break }
    }

    $context = $listener.GetContext()
    $last = Get-Date

    $req = $context.Request
    $res = $context.Response
    $res.Headers.Add("Access-Control-Allow-Origin", "*")
    $res.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS")
    $res.Headers.Add("Access-Control-Allow-Headers", "Content-Type")

    if ($req.HttpMethod -eq "OPTIONS" -and $req.Url.AbsolutePath -eq "/event") {
        $res.StatusCode = 204
        $res.Close()
        continue
    }

    if ($req.HttpMethod -ne "POST" -or $req.Url.AbsolutePath -ne "/event") {
        $res.StatusCode = 404
        $res.Close()
        continue
    }

    $reader = New-Object System.IO.StreamReader($req.InputStream, $req.ContentEncoding)
    $body = $reader.ReadToEnd()
    $reader.Close()

    try {
        $evt = $body | ConvertFrom-Json
        if ($null -eq $evt.ts) { $evt | Add-Member -NotePropertyName ts -NotePropertyValue ([DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()) -Force }
        $line = $evt | ConvertTo-Json -Compress
        Add-Content -Path $logFile -Value $line -Encoding UTF8
        $res.StatusCode = 200
        $bytes = [System.Text.Encoding]::UTF8.GetBytes("ok")
        $res.OutputStream.Write($bytes, 0, $bytes.Length)
        $res.Close()
    } catch {
        $res.StatusCode = 400
        $bytes = [System.Text.Encoding]::UTF8.GetBytes("bad request")
        $res.OutputStream.Write($bytes, 0, $bytes.Length)
        $res.Close()
    }
}

try { $listener.Stop() } catch {}
try { $listener.Close() } catch {}
