param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

# Fail fast if the version is empty
if ([string]::IsNullOrWhiteSpace($Version)) {
    Write-Error "❌ Version parameter is required."
    exit 1
}

$tag = "v$Version"

Write-Host "🏷️ Preparing to tag version $tag..."

# Check if tag already exists
$existingTag = git tag --list $tag
if ($existingTag) {
    Write-Warning "⚠️ Tag $tag already exists. Skipping creation."
} else {
    git tag $tag
    Write-Host "✅ Created tag $tag."
}

# Push tag to remote
Write-Host "📤 Pushing tag to origin..."
git push origin $tag

Write-Host "🎉 Done! Tag $tag pushed to GitHub."