$projects = @(
    @{ 
        Name = "Auth"
        Path = ("$PSScriptRoot\src\Auth.Host\Auth.Host.csproj"|Resolve-Path).ProviderPath
    }
    @{ 
        Name = "Domain"
        Path = ("$PSScriptRoot\src\Domain.Host\Domain.Host.csproj"|Resolve-Path).ProviderPath
    }
    @{
        Name = "DomainUi" 
        Path = ("$PSScriptRoot\src\Domain.UI\Domain.UI.csproj"|Resolve-Path).ProviderPath
    }
)

filter run_project {
    $project = $_
    $project.Job = Start-Job -Name $project.Name -ScriptBlock { dotnet run $project.Path } -WorkingDirectory ($project.Path|Split-Path -Parent)
}

filter show_project {
    $_|Select-Object Name,@{ Name="Job.Id"; Expression={$_.Job.Id} }
}

$projects|run_project
$projects|show_project
while($true){
    Start-Sleep -Seconds 10
    $projects | Where-Object { $_.Job.State -ne "Running" } | ForEach-Object -Process {
        "$_.Name --------------> "|Write-Host
        $_.Job | Receive-Job | Write-Host
        $_.Job | Remove-Job 
        $_ | run_project
    }
    $projects|show_project
}