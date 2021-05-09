fs-dirs\Invoke-AtContainer -Path "$PSScriptRoot\submodules\IdentityShell\src\IdentityShell" {
    
    $publish = $true

    if("$PWD\bin\debug\net5.0\publish"|Test-Path) {
        # get the last time the project was published
        $maxPublishTime = (fs-files\Get-ItemModification -Path "$PWD\bin\debug\net5.0\publish").MaxLastWriteTime
        $maxModificationTime = (fs-files\Get-ItemModification -Path $PWD).MaxLastWriteTime
        $publish = $maxModificationTime -gt $maxPublishTime
    } 
    
    if($publish) {
        "Publishing the IdentityShell again"
        dotnet publish 
    }

    # and run the published IdentitiyShell
    Start-Process -FilePath "$PWD\bin\debug\net5.0\publish\IdentityShell.exe" -ArgumentList @("--urls=`"https://localhost:7777`"")
}
