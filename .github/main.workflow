workflow "CI" {
  on = "push"
  resolves = [
    "Azure Login",
    "Deploy Azure Function",
    "Dotnet Publish",
  ]
}

action "Dotnet Test" {
  uses = "Azure/github-actions/dotnetcore-cli@9e977220e411dbf2a3b79a8566fddb83b11584ea"
  args = "test"
  secrets = ["Apetito__EMail", "Apetito__Password"]
}

action "Dotnet Publish" {
  uses = "Azure/github-actions/dotnetcore-cli@9e977220e411dbf2a3b79a8566fddb83b11584ea"
  args = "publish -c Release"
  secrets = ["Apetito__EMail", "Apetito__Password"]
  needs = ["Dotnet Test"]
}

action "Azure Login" {
  uses = "Azure/github-actions/login@master"
  secrets = ["AZURE_SUBSCRIPTION", "AZURE_SERVICE_APP_ID", "AZURE_SERVICE_PASSWORD", "AZURE_SERVICE_TENANT"]
  needs = ["Dotnet Publish"]
}

action "Deploy Azure Function" {
  uses = "Azure/github-actions/functions@master"
  env = {
    AZURE_APP_NAME = "ApetitOMate"
    AZURE_APP_PACKAGE_LOCATION = "ApetitOMate.Function/bin/Release/netcoreapp2.2/publish/"
  }
  needs = ["Azure Login"]
}
