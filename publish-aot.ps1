#!/usr/bin/env pwsh
# Script to publish the Aspire CLI tool as a native AOT executable

param (
    [string]$Configuration = "Release",
    [string]$Runtime = "",
    [string]$OutputDir = "./publish"
)

# Determine runtime identifier if not specified
if (-not $Runtime) {
    if ($IsWindows) {
        $Runtime = "win-x64"
    } elseif ($IsMacOS) {
        if ((Get-CimInstance -Class Win32_Processor).Architecture -eq 9) {
            $Runtime = "osx-arm64"
        } else {
            $Runtime = "osx-x64"
        }
    } elseif ($IsLinux) {
        $Runtime = "linux-x64"
    } else {
        Write-Error "Could not determine runtime. Please specify with -Runtime parameter."
        exit 1
    }
}

Write-Host "Publishing native AOT executable for $Runtime..."
Write-Host "Configuration: $Configuration"
Write-Host "Output directory: $OutputDir"

# Create the output directory if it doesn't exist
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# Publish the application as a self-contained native AOT executable
dotnet publish -c $Configuration -r $Runtime --self-contained `
    -p:PublishAot=true -p:InvariantGlobalization=true -o $OutputDir

# Check if the publish was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Published successfully to $OutputDir"
    
    # Display executable path and show help on how to use it
    $exePath = Join-Path -Path $OutputDir -ChildPath "aspire-cli$([System.IO.Path]::GetExtension((Get-Command cmd.exe).Path))"
    if (Test-Path $exePath) {
        Write-Host "`nExecutable path: $exePath"
        Write-Host "`nYou can now run the tool with:"
        Write-Host "  $exePath --help"
        Write-Host "  $exePath dev"
        Write-Host "  $exePath new --template webapp --output MyProject"
        Write-Host "  $exePath add myservice"
    } else {
        Write-Host "`n⚠️ Executable not found at expected location. Please check the output directory."
    }
} else {
    Write-Host "`n❌ Publish failed"
}