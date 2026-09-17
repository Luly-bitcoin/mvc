# Proyecto Inmobiliaria

Sistema web para la gestión de alquileres temporarios de una inmobiliaria.

## Integrantes

- Lourdes Villegas - villegasmarialuly@gmail.com - https://github.com/Luly-bitcoin
- Milena Miselli - milivicmp@gmail.com - https://github.com/milemise

## Tecnologías

- ASP.NET Core MVC
- C#
- MySQL / MariaDB
- MySqlConnector
- HTML
- CSS
- Bootstrap
- JavaScript

## Modelado de Datos

![DER](./docs/DER.png)

## Usuarios de prueba

## Administrador

Usuario: lulu
Contraseña: mari123

## Empleado

Usuario: mile
Contraseña: empleado123

## Base de datos

La aplicación utiliza una base de datos llamada:

alquileres_temporarios

El script para crear e inicializar la base de datos se encuentra en:

alquileres_temporarios.sql

### Instalación de la base de datos

1. Abrir XAMPP.
2. Iniciar Apache y MySQL.
3. Abrir phpMyAdmin.
4. Crear/importar la base de datos utilizando el archivo:
   
   alquileres_temporarios.sql

5. Verificar la cadena de conexión en `appsettings.json`. 
(actualmente tiene usuario="root" y contraseña="")

 ---

## Ejecución del proyecto

Desde la carpeta del proyecto ejecutar:

dotnet restore

dotnet run

Luego abrir la dirección indicada por ASP.NET Core.

## Funcionalidades

- Gestión de propietarios.
- Gestión de inquilinos.
- Gestión de inmuebles.
- Gestión de tipos de inmueble.
- Gestión de reservas.
- Gestión de pagos.
- Gestión de usuarios.
- Autenticación.
- Control de acceso según rol.
- Búsqueda y paginación.
- Historial de reservas.
- Gestión de imágenes de inmuebles.