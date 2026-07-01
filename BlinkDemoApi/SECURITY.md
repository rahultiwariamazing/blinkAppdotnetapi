# Security

## Secrets Management Policy

Never commit real secrets to source control.

This includes:
- Database connection strings
- Passwords
- JWT signing keys
- API keys
- Tokens
- Private certificates or private keys

Why:
- Git history is durable and widely replicated.
- Once committed, a secret can be exposed through forks, logs, backups, CI artifacts, and developer machines.
- Secret leaks can allow unauthorized database access, token forgery, and account compromise.

## Local Development Setup

Use .NET User Secrets for local development.

Initialize secrets store for the API project:

```bash
dotnet user-secrets init --project src/BlinkDemoApi.Api
```

Set local-only values:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "<your-local-connection-string>" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:SigningKey" "<your-long-random-signing-key>" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:Issuer" "BlinkDemoApi" --project src/BlinkDemoApi.Api
dotnet user-secrets set "Jwt:Audience" "BlinkDemoApiClients" --project src/BlinkDemoApi.Api
```

Inspect local secrets (optional):

```bash
dotnet user-secrets list --project src/BlinkDemoApi.Api
```

## Production Setup

Use environment variables (or secret managers injected as env vars) in production:

- ConnectionStrings__Default
- Jwt__SigningKey
- Jwt__Issuer
- Jwt__Audience
- Jwt__AccessTokenMinutes
- Jwt__RefreshTokenDays

Example:

```bash
export ConnectionStrings__Default="<prod-connection-string>"
export Jwt__SigningKey="<prod-long-random-signing-key>"
export Jwt__Issuer="BlinkDemoApi"
export Jwt__Audience="BlinkDemoApiClients"
```

## Configuration Precedence

Application configuration loads in this order:
1. appsettings.json
2. appsettings.Development.json
3. User Secrets (Development only)
4. Environment Variables

Later sources override earlier ones.
