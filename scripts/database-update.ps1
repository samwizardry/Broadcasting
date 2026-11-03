param (
    [string] $Project = $( Read-Host "Project dir" ),
    [string] $StartupProject = $( Read-Host "Startup Project dir" )
)

dotnet ef database update --project $Project --startup-project $StartupProject