# FIXMEDICAL Web — Surgical Management System

Sistema web de gestión quirúrgica para el sector de dispositivos médicos:
solicitudes de material con flujo de aprobación, programación de cirugías
con asignación de encargados, y control de implantes y gastos.

Desarrollado en solitario como único desarrollador del proyecto durante
mi etapa de practicas.

## Stack
- ASP.NET Core (Razor Pages) · C#
- Entity Framework Core · SQL Server (code-first con migraciones)
- HTML / CSS / JS propios — sistema de diseño glassmorphism responsive, sin plantillas
- Generación de documentos PDF (formatos oficiales)

## Funcionalidades
- Autenticación y autorización por 4 roles: Administrador, EQC, Auditor, Comercial
- Solicitud de material de cirugía (formato F-G-LG-003) con flujo:
  Comercial crea → Administrador aprueba y asigna EQC → EQC ejecuta
- Programación de cirugías con historial y filtros
- CRUD completo: referencias/productos, hojas de gasto, tarjetas de implante con grilla de detalle
- Guards de acceso por rol en cada página
- UI propia con fondo animado, glassmorphism y accesibilidad (prefers-reduced-motion)

## Estructura
fixmedical.1.1/
├── Modelos/        # Entidades EF Core
├── Migrations/     # Historial code-first de la BD
├── Pages/          # Razor Pages (Admin, Solicitudes, HojasGasto, TarjetasImplante)
├── Services/
└── wwwroot/        # CSS/JS/imágenes propios

## Cómo correrlo
1. Clonar el repositorio
2. Configurar cadena de conexión en `appsettings.json` (SQL Server)
3. `Update-Database` (crea esquema + seed de roles)
4. Ejecutar el proyecto

## Autor
Luis Antonio Arevalo Camacho
LinkedIn: https://www.linkedin.com/in/luis-antonio-arevalo-camacho-73b9b5383/
