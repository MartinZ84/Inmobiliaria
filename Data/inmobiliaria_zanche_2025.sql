-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 22-08-2025 a las 23:21:23
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria_zanche_2025`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `estado` varchar(15) NOT NULL,
  `precio` int(11) NOT NULL,
  `inquilinoId` int(11) NOT NULL,
  `inmuebleId` int(11) NOT NULL,
  `fechaFinAnt` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `fechaInicio`, `fechaFin`, `estado`, `precio`, `inquilinoId`, `inmuebleId`, `fechaFinAnt`) VALUES
(49, '2022-04-21', '2024-04-21', 'Vigente', 30000, 3, 1, NULL),
(51, '2022-04-22', '2024-04-22', 'Vigente', 30000, 3, 3, NULL),
(52, '2022-05-01', '2022-07-01', 'Vigente', 30000, 3, 7, NULL),
(53, '2022-07-02', '2022-08-01', 'Vigente', 50000, 3, 7, NULL),
(55, '2023-04-23', '2024-04-22', 'Vigente', 80000, 13, 14, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(100) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `superficie` decimal(10,0) DEFAULT NULL,
  `tipo` varchar(50) NOT NULL,
  `uso` varchar(20) NOT NULL,
  `precio` int(11) NOT NULL,
  `latitud` decimal(10,0) DEFAULT NULL,
  `longitud` decimal(10,0) DEFAULT NULL,
  `estado` varchar(50) NOT NULL,
  `propietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `ambientes`, `superficie`, `tipo`, `uso`, `precio`, `latitud`, `longitud`, `estado`, `propietarioId`) VALUES
(1, 'Mitre 878, Ciudad de San Luis', 3, 50, 'Departamento', 'Residencial', 25000, 10, 20, 'Disponible', 1),
(3, 'Rivadavia 523, Ciudad de San Luis', 5, 120, 'Casa', 'Residencial', 50000, 8522552, 5855252, 'Disponible', 6),
(7, 'San Justo 4555, Villa Larca, San Luis', 2, 70, 'Local', 'Comercial', 35000, 0, 0, 'Disponible', 4),
(11, 'San Telmo 123, CABA', 3, 76, 'Departamento', 'Residencial', 12345678, 0, 0, 'No disponible', 11),
(13, 'Colon 2542, Mendoza Capital', 4, 76, 'Oficina', 'Comercial', 67000, 0, 0, 'Disponible', 13),
(14, 'Belgrano 45, San Luis', 5, 90, 'Oficina', 'Comercial', 80000, 0, 0, 'Disponible', 14);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `dni` varchar(10) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `nombre`, `apellido`, `dni`, `telefono`, `email`) VALUES
(3, 'Laura', 'Inquilino', '45666878', '45468888', 'jose@inquilino.com'),
(7, 'Jose', 'Sexto', '45666878', '2645548844', 'marilauzan@gmail.com'),
(9, 'Roberto', 'Monez Ruiz', '5343233', '45455444', 'monezruiz@gobernator.com'),
(13, 'Carla', 'Granados', '45666878', '45468888', 'juna@palomino.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `nroPago` int(11) NOT NULL,
  `fechaPago` datetime NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `contratoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `nroPago`, `fechaPago`, `importe`, `contratoId`) VALUES
(33, 1, '2022-04-22 00:00:00', 30000.00, 49),
(34, 2, '2022-04-22 00:00:00', 30000.00, 49),
(35, 1, '2022-04-22 00:00:00', 30000.00, 51);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `dni` varchar(10) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`id`, `nombre`, `apellido`, `dni`, `telefono`, `email`) VALUES
(1, 'Marcos', 'Paz', '25355655', '266421254', 'marcos@mail.com'),
(4, 'Camilo', 'Sexto', '78998545', '52455755122', 'camilos@mail.com'),
(6, 'Juan', 'Palomino', '12550550', '264545444', 'juan@palomino.com'),
(11, 'Jaime', 'Guido', '10545221', '1577858788', 'jaime@mail.com'),
(13, 'Carlos', 'Fernandez', '28990654', '45455444', 'carlos@fernandez.com'),
(14, 'Pablo', 'Granados', '45666878', '0', 'pablo@granados.com'),
(29, 'Pablo', 'perez<', '436', '436', 'martinzanche@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `avatar` varchar(200) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `clave` varchar(50) NOT NULL,
  `rol` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id`, `nombre`, `apellido`, `avatar`, `email`, `clave`, `rol`) VALUES
(1, 'Carla Sofia', 'Peterson', '/Uploads\\avatar_1.jpg', 'carla@peterson.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 3),
(2, 'Pablo', 'Perez', '/Uploads\\avatar_2.jfif', 'pablo@perez.com', 'Es8xLXaQWGPhWN3ndWBEt8ZN7E8T+pDeqi210bMoJsI=', 3),
(3, 'Martin', 'Reich', '/Uploads\\avatar_3.png', 'martin@reich.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 3),
(5, 'Dora', 'Exloradora', '/Uploads\\avatar_5.png', 'dora@mail.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 2);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `FK_INQUILINOID` (`inquilinoId`),
  ADD KEY `FK_INMUEBLEID` (`inmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `DELETE_INMUEBLE_CONTRATOS` (`propietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `FK_CONTRATOID` (`contratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=56;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=36;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_INMUEBLEID` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `FK_INQUILINOID` FOREIGN KEY (`inquilinoId`) REFERENCES `inquilinos` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `DELETE_INMUEBLE_CONTRATOS` FOREIGN KEY (`propietarioId`) REFERENCES `propietarios` (`id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FK_CONTRATOID` FOREIGN KEY (`contratoId`) REFERENCES `contratos` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
