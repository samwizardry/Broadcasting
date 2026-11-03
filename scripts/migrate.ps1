param (
    [string] $MigrationName = $( Read-Host "Migration name" ),
    [string] $Project = $( Read-Host "Project dir" ),
    [string] $StartupProject = $( Read-Host "Startup Project dir" ),
    [string] $Output = "Data/Migrations"
)

dotnet ef migrations add $MigrationName --project $Project --startup-project $StartupProject -o $Output