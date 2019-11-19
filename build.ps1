param(
    [Parameter(Mandatory = $True)] [string] $Version,
    [switch] $Pack
)

$ErrorActionPreference = "Stop"
Write-Host -ForegroundColor Yellow "******** Version $Version ********"
Write-Host -ForegroundColor Yellow "******** Build samples ********"

Get-ChildItem samples\**\*.sln -Recurse | %{
    msbuild $_ -p:Configuration=Release -t:rebuild -restore
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host -ForegroundColor Yellow "******** Build MvvmMicro ********"

if ($Pack) {
    msbuild -p:Configuration=Release -restore -t:rebuild -t:MvvmMicro:pack `
            -p:PackageOutputPath="$(Get-Location)\package" `
            -p:Version=$Version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
    msbuild -p:Configuration=Release -t:rebuild -restore -p:Version=$Version
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host -ForegroundColor Yellow "******** Run tests ********"

dotnet test --configuration Release --no-build --verbosity normal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }