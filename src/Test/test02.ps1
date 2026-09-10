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
            -Body $body

        [PSCustomObject]@{
            Request = "Request 1"
            Status  = $response.StatusCode
            Result  = "Success"
        }
    }
    catch {
        [PSCustomObject]@{
            Request = "Request 1"
            Status  = $_.Exception.Response.StatusCode.value__
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
            -Body $body

        [PSCustomObject]@{
            Request = "Request 2"
            Status  = $response.StatusCode
            Result  = "Success"
        }
    }
    catch {
        [PSCustomObject]@{
            Request = "Request 2"
            Status  = $_.Exception.Response.StatusCode.value__
            Result  = "Rejected"
        }
    }
} -ArgumentList $baseUrl, $refreshToken