IF DB_ID(N'DocenteBM_DB') IS NULL
    EXEC(N'CREATE DATABASE DocenteBM_DB');
GO

USE DocenteBM_DB;
GO

IF OBJECT_ID(N'dbo.Docente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Docente
    (
        IdDocente  int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Docente PRIMARY KEY,
        Codigo     nvarchar(20) NOT NULL CONSTRAINT UQ_Docente_Codigo UNIQUE,
        Nombres    nvarchar(80) NOT NULL,
        Apellidos  nvarchar(80) NOT NULL,
        Email      nvarchar(254) NULL,
        Telefono   nvarchar(30) NULL,
        Especialidad nvarchar(100) NOT NULL,
        Estado     bit NOT NULL CONSTRAINT DF_Docente_Estado DEFAULT (1)
    );
END;
GO
