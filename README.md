# Stokio

Stokio es una plataforma SaaS de gestión de inventario para pymes que necesitan control de stock en tiempo real. Permite administrar sucursales, depósitos, compras, ventas y movimientos desde una interfaz simple y moderna. Modular y escalable, diseñada para crecer con tu negocio.

## Arquitectura

El proyecto está organizado en dos módulos principales:

### Backend - ASP.NET Core 10 (Clean Architecture)
- **Stokio.Domain**: Entidades del dominio y interfaces
- **Stokio.Application**: Lógica de negocio y casos de uso
- **Stokio.Infrastructure**: Implementación de persistencia y servicios externos
- **Stokio.Api**: API REST con JWT y Swagger

### Frontend - Angular 21 LTS
- Interfaz de usuario moderna con Angular 21
- Comunicación con la API mediante HTTP
- Autenticación JWT

## Características Principales

- **Multi-tenancy**: Soporte para múltiples inquilinos con aislamiento de datos
- **Autenticación JWT**: Seguridad basada en tokens
- **EF Core + PostgreSQL**: Persistencia de datos robusta
- **Clean Architecture**: Separación de responsabilidades y fácil mantenimiento
- **Swagger/OpenAPI**: Documentación automática de la API

## Entidades Principales

- **Tenant**: Gestión de inquilinos (multi-tenancy)
- **User**: Usuarios del sistema
- **Role**: Roles y permisos
- **Product**: Productos con SKU y precios
- **Category**: Categorización de productos
- **Warehouse**: Almacenes/depósitos
- **StockMovement**: Movimientos de inventario (compras, ventas, transferencias, ajustes)

## Requisitos Previos

### Backend
- .NET 10 SDK
- PostgreSQL 14 o superior

### Frontend
- Node.js 18 o superior
- npm 9 o superior

## Configuración y Ejecución

### Backend

1. Navegar al directorio del backend:
```bash
cd backend
```

2. Configurar la cadena de conexión en `src/Stokio.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=stokio;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "CHANGE_THIS_TO_A_SECURE_RANDOM_STRING_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "StokioApi",
    "Audience": "StokioClient",
    "ExpirationHours": "24"
  }
}
```

⚠️ **Importante**: Cambia el `Secret` en JwtSettings por una cadena aleatoria segura de al menos 32 caracteres antes de usar en producción.

3. Crear la base de datos con migraciones:
```bash
cd src/Stokio.Api
dotnet ef migrations add InitialCreate --project ../Stokio.Infrastructure
dotnet ef database update
```

4. Ejecutar la API:
```bash
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7000/swagger`

### Frontend

1. Navegar al directorio del frontend:
```bash
cd frontend
```

2. Instalar dependencias (si es necesario):
```bash
npm install
```

3. Ejecutar la aplicación:
```bash
npm start
```

La aplicación estará disponible en `http://localhost:4200`

## Estructura del Proyecto

```
stokio/
├── backend/
│   ├── src/
│   │   ├── Stokio.Api/           # API REST
│   │   ├── Stokio.Application/   # Casos de uso
│   │   ├── Stokio.Domain/        # Entidades
│   │   └── Stokio.Infrastructure/# Persistencia
│   └── Stokio.slnx
├── frontend/
│   ├── src/
│   │   ├── app/                  # Componentes Angular
│   │   └── environments/         # Configuración de entornos
│   └── angular.json
└── README.md
```

## Próximos Pasos

1. Crear migraciones de EF Core
2. Implementar endpoints de autenticación
3. Agregar módulos CRUD para entidades principales
4. Crear componentes Angular para la UI
5. Implementar guards de autenticación
6. Agregar tests unitarios e integración

## Contribuir

Este proyecto está en desarrollo activo. Las contribuciones son bienvenidas.
