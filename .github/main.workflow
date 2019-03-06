workflow "CI" {
  on = "push"
  resolves = ["Azure/github-actions/login@master"]
}

action "Test Core" {
  uses = "Azure/github-actions/dotnetcore-cli@9e977220e411dbf2a3b79a8566fddb83b11584ea"
  args = "test"
  secrets = ["Apetito__EMail", "Apetito__Password"]
}

action "Azure/github-actions/login@master" {
  uses = "Azure/github-actions/login@master"
  needs = ["Test Core"]
  secrets = ["AZURE_SUBSCRIPTION", "AZURE_SERVICE_APP_ID", "AZURE_SERVICE_PASSWORD", "AZURE_SERVICE_TENANT"]
}
