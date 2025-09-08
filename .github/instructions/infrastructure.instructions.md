# Infrastructure (Bicep) — senior quick guide

Scope
- Manage Azure resources for this service using Bicep at subscription scope. Primary entry: `./.bicep/main.bicep`. Naming modules live under `./.bicep/naming/`.
- Pipelines automate lint/what-if/deploy. Secrets/parameters flow via Azure DevOps variable groups.

Prereqs
- Azure CLI with Bicep support and access to the target subscription and service connection.
- Variable groups configured per env: `environment-<env>-variable-group` and `workflow-<env>-variable-group`.

Pipelines overview
- Lint: `pipeline/stages/lint_bicep.yml` — validates and publishes bicep artifact.
- What-if: `pipeline/stages/deploy_what_if.yml` — runs subscription-scope what-if with parameters.
- Deploy: `pipeline/stages/deploy_iac.yml` — executes subscription-scope create with parameters via `pipeline/tasks/bicep-deploy-task.yml`.
- Parameters passed in pipelines: `azureEnvironment`, `name`, `environmentNumber`, `location`, `sveveUsername`, `svevePassword`, `sendgridApiKey`, `whitelistedIPAddresses`.

Local workflow (validate → what-if → deploy)
- Keep deployments reproducible by using a parameter file or a concise `--parameters` list.

```powershell
# Variables
$deploymentName = "ws-$(Get-Date -Format yyyyMMdd-HHmm)"
$location = "norwayeast"    # match your env
$template = ".\.bicep\main.bicep"
$paramFile = ".\.bicep\parameters.dev.json"  # or pass key=value pairs

# Validate
az deployment sub validate `
  --name $deploymentName `
  --location $location `
  --template-file $template `
  --parameters @$paramFile

# What-if (dry-run)
az deployment sub what-if `
  --name $deploymentName `
  --location $location `
  --template-file $template `
  --parameters @$paramFile

# Deploy (create)
az deployment sub create `
  --name $deploymentName `
  --location $location `
  --template-file $template `
  --parameters @$paramFile
```

Parameterization and secrets
- Prefer parameter files checked-in per env with non-secret values. Store secrets in Azure DevOps variable groups or Key Vault.
- In pipelines, secrets (`svevePassword`, `sendgridApiKey`, etc.) are injected from variable groups. Avoid hardcoding.
- If referencing Key Vault from Bicep, use secret references (KV must already exist or be created safely via separate change).

Quality and safety
- Review what-if output for destructive changes (delete/replace) before approving deploy.
- Keep resource names centralized in naming modules; update once, reuse.
- Align regions between Bicep parameters and application runtime (Linux `norwayeast` in CI by default).

Troubleshooting
- Subscription permission errors: confirm the Azure DevOps service connection and your local `az account show` context.
- Template validation failures: run `az bicep build --file ./.bicep/main.bicep` to surface compile-time Bicep errors.
- Drift/no-op: what-if shows no changes; verify parameter values and variable group contents.

References
- Pipelines: `pipeline/azure-pipelines-iac-main.yml`, `pipeline/stages/*.yml`, `pipeline/tasks/bicep-deploy-task.yml`.
- Variables: `pipeline/variables/*.yml` and ADO variable groups `environment-<env>-variable-group`, `workflow-<env>-variable-group`.
