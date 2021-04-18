fs-dirs\Invoke-AtContainer -Path "$PSScriptRoot\submodules\IdentityShell\src\IdentityShell" {
    
    $publish = $true

    if("$PWD\bin\debug\netcoreapp3.1\publish"|Test-Path) {
        # get the last time the project was published
        $maxPublishTime = (fs-files\Get-ItemModification -Path "$PWD\bin\debug\netcoreapp3.1\publish").MaxLastWriteTime
        $maxModificationTime = (fs-files\Get-ItemModification -Path $PWD).MaxLastWriteTime
        $publish = $maxModificationTime -gt $maxPublishTime
    } 
    
    if($publish) {
        "Publishing the IdentityShell again"
        dotnet publish 
    }

    # and run the published IdentitiyShell
    Start-Process -FilePath "$PWD\bin\debug\netcoreapp3.1\publish\IdentityShell.exe"
}
