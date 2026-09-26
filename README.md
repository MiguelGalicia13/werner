# DocenteBM

Sistema de escritorio para administrar docentes con Windows Forms, SQL Server Express y tres capas.

## Requisitos

- Windows y .NET 8 SDK.
- SQL Server Express con la instancia `SQLEXPRESS`.
- Visual Studio 2022 o `dotnet` CLI.

## Preparación

1. Ejecuta [`database/DocenteBM_DB.sql`](database/DocenteBM_DB.sql) en SQL Server Management Studio.
2. Revisa `WinDocenteBMA/appsettings.json` y ajusta `ConnectionStrings:CadenaConexion` si tu instancia usa otra configuración.
3. Desde la carpeta del proyecto ejecuta:

```powershell
dotnet restore DocenteBM.sln
dotnet run --project WinDocenteBMA
```

La aplicación permite registrar, buscar, modificar y anular docentes. Anular cambia `Estado` a inactivo; los registros se conservan.

Para abrir una comprobación rápida de las reglas de validación:

```powershell
dotnet run --project WinDocenteBMA -- --self-check
```
