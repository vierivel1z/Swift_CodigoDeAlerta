param([string]$EditorPath)
# Read-only inventory. Does not certify Unity compilation or APK readiness.
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$manifest = Get-Content (Join-Path $root 'Packages/manifest.json') -Raw | ConvertFrom-Json
$lock = Get-Content (Join-Path $root 'Packages/packages-lock.json') -Raw | ConvertFrom-Json
Get-Content (Join-Path $root 'ProjectSettings/ProjectVersion.txt') | Select-Object -First 1
foreach ($entry in $manifest.dependencies.PSObject.Properties) {
    $locked = $lock.dependencies.PSObject.Properties[$entry.Name]
    if ($null -eq $locked -or $locked.Value.version -ne $entry.Value) { throw "Manifest/lock mismatch: $($entry.Name)" }
}
Write-Output 'Manifest/lock: direct dependency versions match.'
foreach ($name in @('com.unity.render-pipelines.universal','com.unity.inputsystem','com.unity.ugui','com.google.xr.cardboard','com.unity.xr.management')) {
    $entry = $lock.dependencies.PSObject.Properties[$name]
    if ($entry) { Write-Output "$name : locked $($entry.Value.version)" }
    else { Write-Output "$name : absent" }
}
Get-Content (Join-Path $root 'ProjectSettings/GraphicsSettings.asset') | Select-String 'm_CustomRenderPipeline:'
Get-Content (Join-Path $root 'ProjectSettings/ProjectSettings.asset') | Select-String 'AndroidMinSdkVersion:|AndroidTargetSdkVersion:|AndroidTargetArchitectures:|activeInputHandler:'
Get-ChildItem (Join-Path $root 'Assets') -Filter '*.unity' -Recurse | ForEach-Object { Write-Output "Scene: $($_.FullName)" }
if ($EditorPath) {
    if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) { throw 'Editor executable does not exist.' }
    $android = Join-Path (Split-Path $EditorPath -Parent) 'Data/PlaybackEngines/AndroidPlayer'
    foreach ($part in @('SDK','NDK','OpenJDK')) { Write-Output "Android $part present: $(Test-Path -LiteralPath (Join-Path $android $part) -PathType Container)" }
} else { Write-Output 'Android tools not checked: supply -EditorPath.' }
Write-Output "Local .git exists: $(Test-Path -LiteralPath (Join-Path $root '.git'))"
Write-Output 'Manual checks pending: Console, Android Build Profile, APK on real phone.'
