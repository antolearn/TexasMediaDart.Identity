$connectionString = dotnet user-secrets list `
  --project .\src\TexasMediaDart.Identity.Api\TexasMediaDart.Identity.Api.csproj |
  Where-Object { $_ -like "ConnectionStrings:DefaultConnection*" } |
  ForEach-Object { ($_ -split " = ", 2)[1] }

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    Write-Error "DefaultConnection was not found."
    exit 1
}

Write-Host "DefaultConnection loaded successfully."

$dacpacPath = ".\database\TexasMediaDart.Identity.Database\bin\Debug\TexasMediaDart.Identity.Database.dacpac"

if (-not (Test-Path $dacpacPath)) {
    Write-Error "Identity database DACPAC was not found: $dacpacPath"
    exit 1
}

sqlpackage `
  /Action:Publish `
  /SourceFile:"$dacpacPath" `
  /TargetConnectionString:"$connectionString"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Identity database deployment failed."
    exit $LASTEXITCODE
}

Write-Host "Identity database deployment completed successfully."