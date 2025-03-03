# Check if WiX Toolset is in PATH
$wixPath = $null
$possiblePaths = @(
    "C:\Program Files (x86)\WiX Toolset v3.14\bin",
    "C:\Program Files\WiX Toolset v3.14\bin"
)

foreach ($path in $possiblePaths) {
    if (Test-Path "$path\heat.exe") {
        $wixPath = $path
        break
    }
}

if ($null -eq $wixPath) {
    Write-Error "WiX Toolset v3.14 not found in expected locations. Please install it from https://wixtoolset.org/releases/ and add it to PATH"
    exit 1
}

# Ensure WiX tools are in PATH for this session
if (-not ($env:PATH -split ';' -contains $wixPath)) {
    $env:PATH = "$wixPath;$env:PATH"
}

Write-Host "Building installer..." -ForegroundColor Green

# Get absolute paths
$repoRoot = (Get-Item -Path ".\").FullName
$publishDir = Join-Path $repoRoot "projects\chronWindowsImageResizer\bin\Release\net8.0\win-x64\publish"
$setupDir = Join-Path $repoRoot "projects\ChronoImageResizer.Setup"

# Clean and publish the main application
Write-Host "Cleaning and publishing the application..." -ForegroundColor Cyan
dotnet clean (Join-Path $repoRoot "projects\chronWindowsImageResizer") -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to clean the project"
    exit 1
}

dotnet publish (Join-Path $repoRoot "projects\chronWindowsImageResizer") -c Release -r win-x64 --self-contained
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish the project"
    exit 1
}

# Generate components file using heat
Write-Host "Generating component files..." -ForegroundColor Cyan
heat.exe dir "$publishDir" -cg PublishedFilesGroup -dr INSTALLFOLDER -srd -gg -var var.SourceDir -sfrag -suid -scom -sreg -ag -out "$setupDir\PublishedFiles.wxs"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to generate component files"
    exit 1
}

# Build the installer
Write-Host "Building the installer..." -ForegroundColor Cyan
Push-Location $setupDir
try {
    candle.exe -dSourceDir="$publishDir" Product.wxs PublishedFiles.wxs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to compile WiX source files"
        exit 1
    }

    light.exe -ext WixUIExtension Product.wixobj PublishedFiles.wixobj -out "ChronoImageResizer.msi"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to link WiX objects"
        exit 1
    }
}
finally {
    Pop-Location
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green
Write-Host "Installer created at: $setupDir\ChronoImageResizer.msi" -ForegroundColor Green 