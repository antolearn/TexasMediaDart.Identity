$baseUrl = "http://localhost:5248"

Write-Host "Logging in..."

$loginBody = @{
    email = "testuser@texasmediadart.com"
    password = "Password123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod `
    -Uri "$baseUrl/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $loginBody

$refreshToken = $loginResponse.refreshToken

Write-Host "Fresh refresh token obtained."
Write-Host "Starting two concurrent refresh requests..."

$job1 = Start-Job -ScriptBlock {

    param($baseUrl, $refreshToken)

    $body = @{
        refreshToken = $refreshToken
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest `
            -Uri "$baseUrl/api/auth/refresh" `
            -Method Post `
            -ContentType "application/json" `
            -Body $body `
            -UseBasicParsing

        [PSCustomObject]@{
            Request = "Request 1"
            Status  = [int]$response.StatusCode
            Result  = "Success"
        }
    }
    catch {

        $statusCode = 0

        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }

        [PSCustomObject]@{
            Request = "Request 1"
            Status  = $statusCode
            Result  = "Rejected"
        }
    }

} -ArgumentList $baseUrl, $refreshToken


$job2 = Start-Job -ScriptBlock {

    param($baseUrl, $refreshToken)

    $body = @{
        refreshToken = $refreshToken
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest `
            -Uri "$baseUrl/api/auth/refresh" `
            -Method Post `
            -ContentType "application/json" `
            -Body $body `
            -UseBasicParsing

        [PSCustomObject]@{
            Request = "Request 2"
            Status  = [int]$response.StatusCode
            Result  = "Success"
        }
    }
    catch {

        $statusCode = 0

        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }

        [PSCustomObject]@{
            Request = "Request 2"
            Status  = $statusCode
            Result  = "Rejected"
        }
    }

} -ArgumentList $baseUrl, $refreshToken


Wait-Job $job1, $job2 | Out-Null

$result1 = Receive-Job $job1
$result2 = Receive-Job $job2

Remove-Job $job1, $job2

Write-Host ""
Write-Host "Results:"
Write-Host ""

$result1
$result2