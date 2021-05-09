function clean_configurationstore {
    Get-IdentityApiScope|Remove-IdentityApiScope
    Get-IdentityApiResource|Remove-IdentityApiResource
    Get-IdentityResource|Remove-IdentityResource
    Get-IdentityClient|Remove-IdentityClient
    Get-TestUser|Remove-TestUser
}
filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}

clean_configurationstore

# scopes
$global:scopes = [PSCustomObject]@{
    domainApi = Set-IdentityApiScope -Name "domain-api"
    openid = Set-IdentityApiScope -Name "openid"
    profile = Set-IdentityApiScope -Name "profile"
}

# THE secret
$global:secret = [PSCustomObject]@{
    plain = "secret"
    hashed = ("secret"|sha256base64)
    instance = New-IdentitySecret -Value ("secret"|sha256base64)
}

# clients
$mvcClient = @{
    ClientId = "mvc" 
    ClientSecrets = $secret.instance
    AllowedGrantTypes = @("authorization_code")
    RedirectUris = "https://localhost:5002/signin-oidc" 
    PostLogoutRedirectUris = "https://localhost:5002/signout-callback-oidc" 
    AllowedScopes = @($scopes.domainApi.Name,$scopes.openid.Name,$scopes.profile.Name)
}

$psClient = @{
    ClientId = "ps" 
    ClientSecrets = $secret.instance
    AllowedGrantTypes = @("password")
    AllowedScopes = @($scopes.domainApi.Name,$scopes.openid.Name,$scopes.profile.Name)
}

$wpfClient = @{
    ClientId = "wpf" 
    ClientSecrets = $secret.instance
    AllowedGrantTypes = @("password")
    AllowedScopes = @($scopes.domainApi.Name,$scopes.openid.Name,$scopes.profile.Name)
}

$global:clients = [PSCustomObject]@{
    ps = Set-IdentityClient @psClient
    mvc = Set-IdentityClient @mvcClient
    wpf = Set-IdentityClient @wpfClient
}

# users
$alice = @{
    SubjectId = "818727"
    Username = "alice"
    Password = $secret.plain
    Claims = @(
        New-Claim -Type "name" -Value "Alice Smith"
        New-Claim -Type "given_name" -Value "Alice"
        New-Claim -Type "family_name" -Value "Smith"
        New-Claim -Type "email" -Value "AliceSmith@email.com"
        New-Claim -Type "email_verified" -Value "true"
        New-Claim -Type "web_site" -Value "http://alice.com"
        New-Claim -Type "address" -ValueType "json" -Value (@{
            street_address = "One Hacker Way"
            locality = "Heidelberg"
            postal_code = 69118
            country = "Germany"
        } | ConvertTo-Json)
    )                      
}



$bob = @{
    SubjectId = "88421113"
    Username = "bob"
    Password = $secret.plain
    Claims = @(
        New-Claim -Type "name" -Value "Bob Smith"
        New-Claim -Type "given_name" -Value "Bob"
        New-Claim -Type "family_name" -Value "Smith"
        New-Claim -Type "email" -Value "BobSmith@email.com"
        New-Claim -Type "email_verified" -Value "true"
        New-Claim -Type "web_site" -Value "http://bob.com"
        New-Claim -Type "address" -ValueType "json" -Value (@{
            street_address = "One Hacker Way"
            locality = "Heidelberg"
            postal_code = 69118
            country = "Germany"
        } | ConvertTo-Json)
    )                      
}

$global:users = [PSCustomObject]@{
    alice = Set-TestUser @alice
    bob = Set-TestUser @bob
}

# fetch env data from indentits shell
$identityShellConfguration = Invoke-IdentityDiscoveryEndpoint


# example: get token for ps cmdline client and user alice using the resource owner flow
$tokenRequest = @{
    ClientId =  $clients.ps.ClientId
    ClientSecret = $secret.plain
    UserName = $users.alice.Username
    Password = $secret.plain 
    Scopes = $scopes.domainApi.Name
    EndpointUrl = $identityShellConfguration.TokenEndpoint
}

Invoke-IdentityTokenEndpoint @tokenRequest

$tokenRequest = @{
    ClientId =  $clients.wpf.ClientId
    ClientSecret = $secret.plain
    UserName = $users.alice.Username
    Password = $secret.plain 
    Scopes = $scopes.domainApi.Name
    EndpointUrl = $identityShellConfguration.TokenEndpoint
}

Invoke-IdentityTokenEndpoint @tokenRequest

# try the same with my prv ps script module

#web-openid\Get-OpenIdToken @tokenRequest -Authority $identityShellConfguration.TokenEndpoint
#Get-OpenIdToken -ClientId $clients.ps.ClientId -ClientSecret $secret.plain -UserId $users.alice.Username -UserSecret $secret.plain -Authority "https://localhost:5001" -Scopes $scopes.domainApi.Name -GrantType password -SendToClipboardAsBearer