#!/bin/bash

# Set the size threshold in bytes (50MB)
SIZE_THRESHOLD=52428800

# Find and track large files
find . -type f -size +${SIZE_THRESHOLD}c | while read file; do
  if ! git lfs ls-files | grep -q "$file"; then
    echo "Tracking large file with Git LFS: $file"
    git lfs track "$file"
  fi
done

# Add the .gitattributes file
git add .gitattributes
