# Dragon API Gateway

Dragon API Gateway la entrypoint HTTP cho Dragon platform. Phase dau dung Ocelot de route toi Identity Service va giu contract phu hop Kubernetes/GitOps.

## Structure
- `src/Dragon.ApiGateway` - ASP.NET Core gateway host va Ocelot configuration.
- `.github/workflows` - CI/CD workflows cho develop, staging, production.
- `Dockerfile` - Multi-stage image build cho GHCR va Kubernetes.

## Requirements
- .NET 8 SDK
- Docker

## Local run
1. Chay Identity Service o localhost:7001.
2. Tu thu muc repo, chay `dotnet run --project src/Dragon.ApiGateway/Dragon.ApiGateway.csproj`.
3. Test gateway health tai `/health/live` va route Identity tai `/identity/auth/register`.

## Deployment contract
- Image: `ghcr.io/longpham1712578/dragon-api-gateway`
- Container port: `8080`
- Health endpoints: `/health/live`, `/health/ready`
- GitOps manifest targets: `deploy/<env>/api-gateway.yaml` trong repo `dragon-k8s-config`

## Current scope
- Route san sang: Identity Service
- Route backlog: Office, HRM, Payroll

## Next implementation
- Bo sung auth-focused downstream policies khi Identity Service duoc containerize.
- Them test project va quality gates mo rong.
