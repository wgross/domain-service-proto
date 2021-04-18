if(!(Test-Path "$PSScriptRoot\src\Auth.PsMOdule\bin\Debug\net5.0\publish\Auth.PsModule.psd1")) {
    # the powershell module must be build and published
    fs-dirs\Invoke-AtContainer -Path "$PSScriptRoot\src\Auth.PsModule" { dotnet publish }
}

if(Test-Path "$PSScriptRoot\src\Auth.PsMOdule\bin\Debug\net5.0\publish\Auth.PsModule.psd1") {
    # the powershell module must be build and published
    
    Import-Module "$PSScriptRoot\src\Auth.PsModule\bin\Debug\net5.0\publish\Auth.PsModule.psd1"
} else {
    throw "This script requires a built an publihed powershell module"
}

function clean_configurationstore {
    Get-IdentityApiScope|Remove-identityApiScope
    Get-IdentityApiResource|Remove-IdentityApiResource
    Get-IdentityResource|Remove-IdentityResource
    Get-IdentityClient|Remove-IdentityClient
}

function clean_aspidentitystore {
    Get-AspNetIdentityUser|ForEach-Object {
        Get-AspNetIdentityUserClaim -UserName $_.UserName|Remove-AspNetIdentityUserClaim -UserName $_.UserName
    }
    Get-AspNetIdentityUser|Remove-AspNetIdentityUser
}

filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}
