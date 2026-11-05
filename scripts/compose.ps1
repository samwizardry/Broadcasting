param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet("start", "start-dev", "stop", "stop-dev")]
    [string]$Action,

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ExtraArgs
)

switch ($Action) {
    "start" { docker compose -f docker-compose.yml up -d @ExtraArgs }
    "start-dev" { docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d @ExtraArgs }
    "stop" { docker compose -f docker-compose.yml down @ExtraArgs }
    "stop-dev" { docker compose -f docker-compose.yml -f docker-compose.dev.yml down @ExtraArgs }
}