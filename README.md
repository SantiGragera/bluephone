# Blue Phone - Tienda Online de productos Apple

Parcial 1 - Laboratorio de Programación 3
Instituto Superior Santo Domingo (ISSD) - Tecnicatura Superior en Desarrollo de Software
Profesor: Christian Mansilla - Ciclo lectivo 2026

Sistema de gestión de productos de una tienda online (altas, bajas, modificaciones y consultas),
desarrollado con **ASP.NET Core Razor Pages (.NET 8)**, **C#** y **SQL Server** usando **ADO.NET**
(`SqlConnection`, `SqlCommand`, `SqlDataReader`) con consultas parametrizadas.

## Esquema de la base de datos

| Tabla | Campos |
|---|---|
| `categorias` | `idCategoria` (PK), `descripcion` |
| `productos` | `idProducto` (PK), `nombre`, `precio`, `idCategoria` (FK -> categorias) |

El script completo (tablas, relación y 10 productos de ejemplo) está en [`Database/BluePhone.sql`](Database/BluePhone.sql).

## Páginas

| Página | Archivo | Qué hace |
|---|---|---|
| Inicio | `Pages/Index.cshtml` | Menú con hipervínculos a Alta, Consulta, Modificación y Baja |
| Alta | `Pages/Alta.cshtml` | Carga un producto nuevo (nombre, precio y categoría) al presionar **Confirmar** |
| Consulta | `Pages/Consulta.cshtml` | Lista productos con `INNER JOIN` entre `productos` y `categorias`, con búsqueda por nombre |
| Modificación | `Pages/Modificacion.cshtml` | Edita nombre, precio y categoría; valida que se haya cambiado al menos un campo |
| Baja | `Pages/Baja.cshtml` | Elimina un producto (con mensaje de confirmación); no afecta la tabla `categorias` |

Todas las páginas incluyen un hipervínculo para volver al inicio.

## Equivalencias con el enunciado (Web Forms -> Razor Pages)

| Enunciado | Implementación |
|---|---|
| `SqlDataSource` | ADO.NET manual en el PageModel |
| `HyperLink` | `<a asp-page="...">` |
| `TextBox` | `<input asp-for="...">` + `[BindProperty]` |
| Eventos de botón | Handlers `OnGet`, `OnPost`, `OnPostGuardar`, `OnPostBorrar` |

## Cómo ejecutarlo

1. Tener instalado Visual Studio, el SDK de .NET 8 y SQL Server.
2. Ejecutar `Database/BluePhone.sql` en SQL Server Management Studio.
3. Revisar la cadena de conexión en `appsettings.json` (por defecto `Server=localhost`).
4. Instalar el paquete: `dotnet add package Microsoft.Data.SqlClient` (si no está en el proyecto).
5. Abrir el proyecto en Visual Studio y ejecutar con **F5**.

## Estilos

CSS externo en `wwwroot/css/site.css`, aplicado a todas las páginas desde `Pages/Shared/_Layout.cshtml`.
"# bluephone" 
