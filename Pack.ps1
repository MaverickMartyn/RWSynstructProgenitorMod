$ModName = "SynstructProgenitorMod"
$ZipFileName = "$($ModName).zip"

# Delete archive if it already exists
if (Test-Path $ZipFileName) {
    Remove-Item $ZipFileName -Force
}

$artifacts =
    "1.6",
    "README.md",
    "Textures",
    "About"

if (Test-Path "LICENSE") {
    $artifacts += "LICENSE"
}

$compress = @{
	LiteralPath = $artifacts
	CompressionLevel = "Fastest"
	DestinationPath = $ZipFileName
}

# Create the zip archive
Compress-Archive @compress
Write-Host "Mod packaged as zip file: $ZipFileName"
