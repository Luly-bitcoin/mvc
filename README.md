# Proyecto Inmobiliaria
Sistema web desarrollado en ASP.NET Core MVC para la gestión integral de alquileres temporarios de una inmobiliaria.

## Integrantes

- Lourdes Villegas - villegasmarialuly@gmail.com - https://github.com/Luly-bitcoin
- Milena Miselli - milivicmp@gmail.com - https://github.com/milemise


## Tecnologías
* ASP.NET Core MVC
* C#
* MySQL / MariaDB
* MySqlConnector
* BCrypt.Net.BCrypt (Seguridad y encriptación de contraseñas)
* HTML5, CSS3, Bootstrap
* JavaScript

## Modelo de Datos (Diagrama Entidad-Relación)
El sistema utiliza una base de datos relacional normalizada que incluye las entidades de Propietarios, Inmuebles, Inquilinos, Reservas, Pagos y Usuarios, vinculadas mediante claves foráneas para garantizar la integridad referencial.

![DER](./docs/DER.png)

## Usuarios de prueba
* **Administrador**
  * Usuario: `lulu` (o `admin`)
  * Contraseña: `mari123` (Acceso de emergencia alternativo: `123456`)
* **Empleado**
  * Usuario: `mile`
  * Contraseña: `empleado123`

## Base de Datos
* **Nombre de la base de datos:** `alquileres_temporarios`
* **Ubicación del script:** El archivo SQL completo con la estructura y los datos iniciales se encuentra en la carpeta `database/alquileres_temporarios.sql`.

### Instalación de la base de datos
1. Abrir XAMPP e iniciar **Apache** y **MySQL**.
2. Abrir **phpMyAdmin**.
3. Crear e importar la base de datos utilizando el archivo ubicado en `database/alquileres_temporarios.sql`.
4. Verificar y configurar la cadena de conexión en el archivo `appsettings.json` (por defecto con usuario `root` y contraseña vacía `""`).

## Ejecución del Proyecto
Desde la carpeta raíz del proyecto, ejecutar los siguientes comandos en la terminal:

dotnet restore
dotnet run

Luego, abrir en el navegador web la dirección indicada por la consola de ASP.NET Core.

## Funcionalidades
* Gestión completa de propietarios, inquilinos, inmuebles y tipos de inmueble.
* Gestión de reservas e historial detallado.
* Control y registro de pagos asociados a las reservas.
* Gestión de usuarios con control de acceso basado en roles (Administrador / Empleado).
* Paginación y filtros de búsqueda avanzados integrados en los listados principales.
* Seguridad y encriptación de contraseñas mediante `BCrypt.Net.BCrypt`.
* Gestión y carga de imágenes asociadas a los inmuebles.
