workflow "CI" {
  on = "push"
  resolves = ["Test Core"]
}

action "Test Core" {
  uses = "Azure/github-actions/dotnetcore-cli@9e977220e411dbf2a3b79a8566fddb83b11584ea"
  args = "test"
}
