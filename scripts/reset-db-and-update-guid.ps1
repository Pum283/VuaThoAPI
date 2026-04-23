param(
    [switch]$SkipDrop
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$infraProject = Join-Path $root "Infrastructure\Infrastructure.csproj"

Write-Host "==> Kiểm tra migration hiện có..."
dotnet ef migrations list --project $infraProject

if (-not $SkipDrop) {
    Write-Host "==> Xoá database hiện tại..."
    dotnet ef database drop --force --project $infraProject
}

Write-Host "==> Apply migration mới nhất..."
dotnet ef database update --project $infraProject

Write-Host "==> Hoàn tất reset DB + update migration."
