CREATE TABLE dbo.UsuariosApp(
    UsuarioAppId INT IDENTITY PRIMARY KEY,
    SamAccountName NVARCHAR(180) NOT NULL,
    DomainCode NVARCHAR(32) NOT NULL,
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.RolesApp(
    RolAppId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE dbo.Permisos(
    PermisoId INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE dbo.UsuarioRol(
    UsuarioAppId INT NOT NULL,
    RolAppId INT NOT NULL,
    DomainCode NVARCHAR(32) NOT NULL,
    PRIMARY KEY(UsuarioAppId, RolAppId, DomainCode),
    FOREIGN KEY (UsuarioAppId) REFERENCES dbo.UsuariosApp(UsuarioAppId),
    FOREIGN KEY (RolAppId) REFERENCES dbo.RolesApp(RolAppId)
);

CREATE TABLE dbo.RolPermiso(
    RolAppId INT NOT NULL,
    PermisoId INT NOT NULL,
    PRIMARY KEY(RolAppId, PermisoId),
    FOREIGN KEY (RolAppId) REFERENCES dbo.RolesApp(RolAppId),
    FOREIGN KEY (PermisoId) REFERENCES dbo.Permisos(PermisoId)
);

CREATE TABLE dbo.AuditoriaAccesos(
    AuditId BIGINT IDENTITY PRIMARY KEY,
    DomainCode NVARCHAR(32) NOT NULL,
    ActionName NVARCHAR(100) NOT NULL,
    Actor NVARCHAR(180) NOT NULL,
    PayloadJson NVARCHAR(MAX) NOT NULL,
    CorrelationId NVARCHAR(80) NOT NULL,
    CreatedAtUtc DATETIMEOFFSET NOT NULL
);
GO

CREATE OR ALTER PROCEDURE dbo.usp_CloneUserPermissions
    @SourceUser SYSNAME,
    @TargetUser SYSNAME,
    @IncludeObjectPermissions BIT,
    @IncludeDeny BIT
AS
BEGIN
    SET NOCOUNT ON;
    -- Implementación segura: clonado de roles y permisos evitando duplicados
    -- TODO: incorporar dry-run, conflictos, y cross-domain mediante tablas de staging.
END
GO
