# Especificación e Instrucciones de Implementación: Sistema DocenteBM

Este documento contiene las especificaciones técnicas verificadas y las directrices necesarias para que un agente o desarrollador independiente reconstruya e implemente el proyecto desde cero, respetando la arquitectura original y las restricciones del entorno.

---

## 1. Arquitectura del Sistema (3 Capas)

El proyecto debe implementarse bajo una **Arquitectura en 3 Capas** estricta en C# / .NET:

```
┌────────────────────────────────────────────────────────┐
│         Capa de Presentación (WinDocenteBMA)           │
│  - Interfaz de usuario (Windows Forms / Formulario)   │
│  - Formulario principal: frmDocenteBM                  │
│  - Depende de: CapaLogicaNegocioBM y CapaEntiedadesBM  │
└───────────────────────────┬────────────────────────────┘
                            │
                            ▼
┌────────────────────────────────────────────────────────┐
│             Capa de Lógica de Negocio                  │
│               (CapaLogicaNegocioBM)                    │
│  - Reglas de validación y orquestación                 │
│  - Operaciones CRUD y lógica de persistencia           │
│  - Depende de: CapaEntiedadesBM                        │
└───────────────────────────┬────────────────────────────┘
                            │
                            ▼
┌────────────────────────────────────────────────────────┐
│                 Capa de Entidades                      │
│                (CapaEntiedadesBM)                      │
│  - Modelos de datos / Clases POCO                      │
│  - Entidad base: Docente                               │
│  - Independiente (sin dependencias a capas superiores) │
└────────────────────────────────────────────────────────┘
```

---

## 2. Infraestructura y Base de Datos

* **Motor de Base de Datos:** Microsoft SQL Server Express (`SQLEXPRESS`).
* **Nombre de Base de Datos sugerido:** `DocenteBM_DB`.
* **Cadena de Conexión (Connection String):**
  Debe configurarse en el archivo de configuración (`App.config` o `appsettings.json`) de la capa de presentación:
  ```xml
  <connectionStrings>
    <add name="CadenaConexion"
         connectionString="Server=localhost\SQLEXPRESS;Database=DocenteBM_DB;Integrated Security=True;TrustServerCertificate=True;"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  ```
  *(O con autenticación SQL: `Server=localhost\SQLEXPRESS;Database=DocenteBM_DB;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;`)*.

---

## 3. Estructura de Proyectos y Responsabilidades

### A. Capa de Entidades (`CapaEntiedadesBM`)
* **Tipo:** Biblioteca de Clases (`Class Library`).
* **Responsabilidad:** Representar las estructuras de datos sin lógica de infraestructura ni UI.
* **Componentes mínimos:**
  * Clase `Docente.cs`: Propiedades del docente.

### B. Capa de Lógica de Negocio (`CapaLogicaNegocioBM`)
* **Tipo:** Biblioteca de Clases (`Class Library`).
* **Responsabilidad:**
  * Validar reglas de negocio (ej. campos obligatorios, validación de identificación).
  * Conexión y persistencia hacia la base de datos SQL Server Express (usando ADO.NET con consultas parametrizadas o Procedimientos Almacenados).
  * Métodos requeridos de gestión:
    * `ListarDocentes()`
    * `BuscarDocentePorId(int id)` o por código/criterio
    * `RegistrarDocente(Docente docente)`
    * `ActualizarDocente(Docente docente)`
    * `EliminarDocente(int id)` (o anulación lógica)

### C. Capa de Presentación (`WinDocenteBMA`)
* **Tipo:** Aplicación de Escritorio (Windows Forms).
* **Responsabilidad:** Interacción con el usuario final.
* **Componentes mínimos:**
  * Formulario principal: `frmDocenteBM`
  * Controles de UI:
    * Grilla/Tabla (`DataGridView`) para listar los docentes registrados.
    * Campos de entrada (`TextBox`, `ComboBox`, etc.) para los atributos del docente.
    * Botones de acción: Guardar, Modificar, Eliminar, Buscar, Limpiar.
  * Recursos gráficos: Incorporar los iconos disponibles en el repositorio para los botones de la barra de herramientas / acciones:
    * `buscar.png` / `BusRapida.png` (Búsqueda)
    * `anulaciondocr.PNG` (Anular / Eliminar)
    * `procesosr.png` (Ejecutar / Procesar)
    * `aplicarservidorr.PNG` (Configuración de servidor / conexión)

---

## 4. Dudas Críticas y Parámetros Pendientes de Definición

Para no asumir ni alucinar especificaciones, el agente que ejecute la implementación debe consultar y confirmar los siguientes puntos antes de codificar:

1. **Esquema Exacto de la Tabla `Docente`:**
   * ¿Cuáles son las columnas requeridas? (Ejemplo habitual: `IdDocente`, `Codigo`, `Nombres`, `Apellidos`, `Email`, `Telefono`, `Especialidad`, `Estado`).
2. **Versión del Framework .NET:**
   * ¿Se debe implementar en **.NET Framework 4.8** (compatible con Windows Forms legado) o en **.NET 8.0 Windows** (`net8.0-windows`)?
   * *Nota de compatibilidad:* Si el agente opera en un entorno Linux, Windows Forms requiere Mono o compilar en Windows. Alternativamente, definir si se permite una interfaz multiplataforma (ej. Avalonia o Web API + frontend web).
3. **Manejo de Transacciones de Base de Datos:**
   * ¿Se utilizarán Procedimientos Almacenados (`Stored Procedures`) en SQL Server o sentencias parametrizadas directas en C#?
4. **Propósito de Iconos Adicionales:**
   * El repositorio contiene iconos como `cambioprecior.PNG` y `camedida.png`. ¿Existen módulos adicionales planeados en este sistema (ej. tarifas, horas de clase, facturación docente) o deben ignorarse por ahora?

---

## 5. Checklist de Verificación para el Agente

- [ ] Crear la base de datos `DocenteBM_DB` y las tablas correspondientes en la instancia `SQLEXPRESS`.
- [ ] Crear la solución `.sln` y los 3 proyectos correspondientes.
- [ ] Configurar las referencias entre proyectos:
  - `WinDocenteBMA` -> `CapaLogicaNegocioBM` y `CapaEntiedadesBM`
  - `CapaLogicaNegocioBM` -> `CapaEntiedadesBM`
- [ ] Implementar la cadena de conexión hacia `SQLEXPRESS` con manejo de excepciones de conexión.
- [ ] Implementar operaciones CRUD completas probadas con la base de datos.
- [ ] Vincular los iconos del repositorio a los botones de la interfaz `frmDocenteBM`.
