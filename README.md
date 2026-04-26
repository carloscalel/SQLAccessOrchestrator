# Plataforma Centralizada de Gestión de Accesos SQL Server Multi-Dominio

Arquitectura empresarial basada en Clean Architecture para gobierno de accesos SQL Server.

## Capas
- **Api**: ASP.NET Core Web API (Negotiate + policy-based auth + middlewares de dominio/correlation/error).
- **Application**: casos de uso de alta seguridad (creación/deshabilitación/clonado).
- **Domain**: entidades y contratos de negocio.
- **Infrastructure**: Dapper + SQL Server + guardas de comandos críticos.
- **UI**: Blazor Server + MudBlazor.
- **deploy/sql**: modelo de seguridad y auditoría.

## Seguridad
- Bloqueo explícito de `WITH GRANT OPTION`, `db_owner`, `db_securityadmin`, `ALTER ANY USER`, `ALTER ANY ROLE`, `CONTROL`.
- Auditoría total por dominio + correlationId.
- Separación por `X-Active-Domain` obligatorio.

## Arranque
```bash
dotnet restore SqlAccessOrchestrator.sln
dotnet build SqlAccessOrchestrator.sln
dotnet test tests/SqlAccessOrchestrator.UnitTests/SqlAccessOrchestrator.UnitTests.csproj
```
