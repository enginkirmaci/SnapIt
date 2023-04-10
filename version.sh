truncate -s 0 latest-version.json

# Write new text to the file
echo "{\"Version\":\"$1\"}" > latest-version.json