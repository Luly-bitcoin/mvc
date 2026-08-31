-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 01-09-2026 a las 00:13:48
-- Versión del servidor: 8.0.43
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `alquileres_temporarios`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id` int NOT NULL,
  `id_propietario` int NOT NULL,
  `id_tipo_inmueble` int NOT NULL,
  `direccion` varchar(255) COLLATE utf8mb4_general_ci NOT NULL,
  `cupo` int NOT NULL,
  `coord` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `precio` decimal(10,2) NOT NULL,
  `activo` int DEFAULT '1',
  `foto_portada` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `fotos` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`id`, `id_propietario`, `id_tipo_inmueble`, `direccion`, `cupo`, `coord`, `precio`, `activo`, `foto_portada`, `fotos`) VALUES
(1, 2, 4, 'La Toma', 2, NULL, 100000.00, 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `id` int NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(150) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `telefono` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id` int NOT NULL,
  `id_reserva` int NOT NULL,
  `concepto` varchar(150) COLLATE utf8mb4_general_ci NOT NULL,
  `fecha_pago` date NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `activo` int DEFAULT '1',
  `creado_por_user_id` int NOT NULL,
  `anulado_por_user_id` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `id` int NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `dni` varchar(20) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(150) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `telefono` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`id`, `nombre`, `apellido`, `dni`, `email`, `telefono`) VALUES
(2, 'Lourdes', 'Gomez', '12312413', 'maria@gmail.com', '3123213'),
(3, 'Louis', 'Sosa', '21312414', 'loues@gmai.com', '21312321'),
(4, 'Lucas', 'Asakds', '21321432', 'lucas@hotmail.com', '2243523522'),
(5, 'Jose', 'Paez', '21312342', 'jose@gmail.com', '2323423524'),
(6, 'Sofia', 'Luces', '21312312', 'so@gmail.com', '24233242532'),
(7, 'Marta', 'Sanchez', '32423423', 'mar@gmail.com', '24143143431'),
(8, 'Juan', 'Lopez', '32423423', 'juan@gmail.com', '24325215252'),
(9, 'Lionel', 'Messi', '23423423', '10@gmail.com', '231412421412'),
(10, 'Leandro', 'Paredes', '32432412', 'lea@gmail.com', '241241241241'),
(11, 'Taylor', 'Swift', '32412421', 'tay@gmail.com', '214194829014'),
(12, 'Selena', 'Gomez', '41243141', 'sel@gmail.com', '94801248120');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reserva`
--

CREATE TABLE `reserva` (
  `id` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `id_inquilino` int NOT NULL,
  `fecha_desde` date NOT NULL,
  `fecha_hasta` date NOT NULL,
  `monto_diario` decimal(10,2) NOT NULL,
  `activo` int DEFAULT '1',
  `creado_por_user_id` int NOT NULL,
  `terminado_por_user_id` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_inmueble`
--

CREATE TABLE `tipo_inmueble` (
  `id` int NOT NULL,
  `descripcion` varchar(100) COLLATE utf8mb4_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo_inmueble`
--

INSERT INTO `tipo_inmueble` (`id`, `descripcion`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Monoambiente'),
(4, 'Loft'),
(5, 'Quinta'),
(6, 'Local Comercial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id` int NOT NULL,
  `nombre_usuario` varchar(50) COLLATE utf8mb4_general_ci NOT NULL,
  `nombre` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(150) COLLATE utf8mb4_general_ci NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_general_ci NOT NULL,
  `avatar` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL,
  `rol` varchar(50) COLLATE utf8mb4_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_inmueble_propietario` (`id_propietario`),
  ADD KEY `fk_inmueble_tipo` (`id_tipo_inmueble`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_pago_reserva` (`id_reserva`),
  ADD KEY `fk_pago_creado_por` (`creado_por_user_id`),
  ADD KEY `fk_pago_anulado_por` (`anulado_por_user_id`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_reserva_inmueble` (`id_inmueble`),
  ADD KEY `fk_reserva_inquilino` (`id_inquilino`),
  ADD KEY `fk_reserva_creado_por` (`creado_por_user_id`),
  ADD KEY `fk_reserva_terminado_por` (`terminado_por_user_id`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `nombre_usuario` (`nombre_usuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `reserva`
--
ALTER TABLE `reserva`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `fk_inmueble_propietario` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_inmueble_tipo` FOREIGN KEY (`id_tipo_inmueble`) REFERENCES `tipo_inmueble` (`id`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `fk_pago_anulado_por` FOREIGN KEY (`anulado_por_user_id`) REFERENCES `usuario` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pago_creado_por` FOREIGN KEY (`creado_por_user_id`) REFERENCES `usuario` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pago_reserva` FOREIGN KEY (`id_reserva`) REFERENCES `reserva` (`id`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD CONSTRAINT `fk_reserva_creado_por` FOREIGN KEY (`creado_por_user_id`) REFERENCES `usuario` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_reserva_inmueble` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_reserva_inquilino` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_reserva_terminado_por` FOREIGN KEY (`terminado_por_user_id`) REFERENCES `usuario` (`id`) ON DELETE SET NULL ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
