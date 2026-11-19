# 📦 SimpleInventarioNet8

**SimpleInventarioNet8** es una aplicación web desarrollada en **.NET 8** bajo el patrón de arquitectura **MVC (Model-View-Controller)** diseñada para gestionar de manera sencilla y eficiente el control de inventarios, movimientos de almacén y reportes de existencias.

---

## 🚀 Funcionalidades Principales

A partir de la estructura del proyecto, el sistema cuenta con los siguientes módulos funcionales:

### 1. Gestión de Catálogos Base
* **Productos (`ProductosController`):** Administración completa (CRUD: Crear, Leer, Actualizar, Eliminar) del catálogo de artículos. Incluye vistas especializadas para el monitoreo de inventario crítico:
  * Consulta general de productos con paginación.
  * **Alertas de Stock Bajo (`StockBajo.cshtml`):** Visualización rápida de artículos próximos a agotarse.
  * **Productos Agotados (`SinExistencia.cshtml`):** Listado de artículos con inventario en cero.
* **Proveedores (`ProveedoresController`):** Gestión integral (CRUD) del directorio de proveedores asociados a las entradas de mercancía.
* **Unidades de Medida (`UnidadesController`):** Administración (CRUD) de las unidades métricas utilizadas para cuantificar los productos (ej. piezas, cajas, kilogramos, litros).

### 2. Control de Movimientos de Inventario
* **Entradas de Almacén (`EntradasController`):** Registro y consulta histórica de ingresos de mercancía al inventario, vinculando productos y proveedores.
* **Salidas de Almacén (`SalidasController`):** Registro y listado de egresos o despachos de mercancía del inventario.
* **Lógica de Negocio Desacoplada (`InventarioService` / `IInventarioService`):** Encapsula las reglas de negocio para el cálculo y actualización automática de existencias ante cada movimiento.

### 3. Reportes y Auditoría
* **Reporte de Existencias (`ReportesController` -> `Existencias.cshtml`):** Vista consolidada del estado actual del stock disponible por producto.
* **Kardex de Movimientos (`ReportesController` -> `Kardex.cshtml`):** Historial detallado y cronológico de entradas y salidas por artículo (`TipoMovimiento`), permitiendo auditar la trazabilidad del inventario.

### 4. Interfaz y Experiencia de Usuario (UI/UX)
* **Dashboard Principal (`HomeController`):** Vista de inicio con acceso rápido a los indicadores y módulos principales del sistema.
* **Paginación Reutilizable:** Implementación de componentes genéricos (`Paginar.cs`, `PaginacionViewModel.cs` y la vista parcial `_Pagination.cshtml`) para navegar eficientemente a través de grandes volúmenes de registros.

---

## 🏗️ Arquitectura y Estructura del Proyecto

La solución (`SimpleInventario.sln`) está dividida en dos proyectos principales:

### 1. `SimpleInventario.Web` (Aplicación Principal)
Organizada siguiendo las convenciones estándar de ASP.NET Core MVC con una capa de servicios:

```text
SimpleInventarioNet8/
├── Controllers/       # Controladores MVC (Home, Productos, Proveedores, Unidades, Entradas, Salidas, Reportes)
├── Data/              # Contexto de Entity Framework Core (ApplicationDbContext.cs)
├── Models/            # Entidades de dominio y helpers de paginación (Productos, Entradas, Salidas, etc.)
├── Services/          # Capa de lógica de negocio (InventarioService, ReportesService y sus interfaces)
├── ViewModels/        # Modelos de vista especializados (MovimientoViewModels.cs)
├── Views/             # Vistas Razor (.cshtml) organizadas por controlador y componentes compartidos
├── wwwroot/           # Recursos estáticos (CSS y JavaScript personalizado)
└── Program.cs         # Punto de entrada, configuración de servicios e inyección de dependencias
```

### 2. `SimpleInventario.Tests` (Pruebas Unitarias)
Proyecto dedicado a garantizar la calidad y el cumplimiento de reglas de validación en los modelos de datos (`ModelsValidationTests.cs`).

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 8 (ASP.NET Core MVC)
* **Acceso a Datos:** Entity Framework Core (`ApplicationDbContext`)
* **Frontend:** Razor Views, HTML5, CSS3, JavaScript (con validación del lado del cliente mediante `_ValidationScriptsPartial.cshtml`)
* **Pruebas:** Framework de pruebas unitarias para .NET (`SimpleInventario.Tests`)

---

## ⚙️ Instalación y Ejecución

1. **Clonar el repositorio o descomprimir el proyecto:**
   Asegúrate de contar con el **SDK de .NET 8** instalado en tu equipo.

2. **Configurar la cadena de conexión:**
   Abre el archivo `appsettings.json` (o `appsettings.Development.json`) dentro de `SimpleInventarioNet8/` y verifica o actualiza la cadena de conexión hacia tu servidor de base de datos.

3. **Aplicar migraciones (si aplica):**
   Ejecuta el siguiente comando en la terminal dentro de la carpeta del proyecto web para crear la base de datos:
   ```bash
   dotnet ef database update
   ```

4. **Ejecutar la aplicación:**
   Inicia el servidor de desarrollo mediante la CLI de .NET o Visual Studio:
   ```bash
   dotnet run --project SimpleInventarioNet8/SimpleInventario.Web.csproj
   ```

5. **Ejecutar las pruebas unitarias:**
   Para validar el correcto funcionamiento de los modelos:
   ```bash
   dotnet test SimpleInventarioNet8.Tests/SimpleInventario.Tests.csproj
   ```