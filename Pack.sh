#!/bin/bash

# Fail fast on errors, undefined variables, and pipeline failures
set -euo pipefail

zipFileName="SynstructProgenitorMod.zip"

# Delete archive if it already exists
if [ -f "$zipFileName" ]; then
    rm -f "$zipFileName"
fi

artifacts=(
    "1.6"
    "README.md"
    "Textures"
    "About"
)

if [ -f "LICENSE" ]; then
    artifacts+=("LICENSE")
fi

# Create the zip archive
zip -r -9 "$zipFileName" "${artifacts[@]}"
echo "Created $zipFileName"
