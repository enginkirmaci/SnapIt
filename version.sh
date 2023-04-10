truncate -s 0 latest-version.json

# Write new text to the file
echo "{\"Version\":\"$1\"}" > latest-version.json

# Commit and push the changes
git config --global user.name 'Engin KIRMACI'
git config --global user.email 'enginkirmaci@outlook.com'
git add latest-version.json
git commit -m 'Update version'
git push 