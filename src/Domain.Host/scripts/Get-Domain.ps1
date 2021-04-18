param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    $Token
)

$request = @{
    Uri = "https://localhost:6001/domain"
    Headers = @{
        Authorization = "Bearer $token"
    }
}

Invoke-RestMethod @request | Write-Output