-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 20-09-2025 a las 20:32:06
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
  `fechaFinAnt` date DEFAULT NULL,
  `usuarioIdAlta` int(11) DEFAULT NULL,
  `usuarioIdBaja` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `fechaInicio`, `fechaFin`, `estado`, `precio`, `inquilinoId`, `inmuebleId`, `fechaFinAnt`, `usuarioIdAlta`, `usuarioIdBaja`) VALUES
(49, '2022-04-21', '2025-04-21', 'No vigente', 20555, 3, 1, '2025-04-21', 1, NULL),
(51, '2022-04-22', '2024-04-22', 'No vigente', 30000, 3, 3, '2024-04-22', 1, NULL),
(52, '2022-05-01', '2022-07-01', 'Revocado', 30000, 3, 7, '2025-09-20', 1, 5),
(53, '2025-07-05', '2026-07-01', 'Revocado', 50000, 3, 7, '2025-09-09', 2, NULL),
(65, '2025-09-16', '2026-09-22', 'Revocado', 250000, 3, 26, '2025-09-20', 1, 3),
(66, '2025-07-02', '2026-09-02', 'Revocado', 55555, 3, 22, '2025-09-11', 1, NULL),
(67, '2025-09-02', '2027-09-02', 'Vigente', 345235, 7, 23, '2027-09-02', 1, NULL),
(68, '2025-09-05', '2025-10-05', 'Revocado', 5000000, 9, 1, '2025-09-09', 1, NULL),
(74, '2025-09-05', '2025-11-05', 'Vigente', 1299900, 3, 20, '2025-11-05', 1, NULL),
(80, '2025-09-11', '2027-09-11', 'Vigente', 12347878, 24, 27, NULL, 3, NULL),
(81, '2026-09-23', '2027-09-23', 'Vigente', 55500000, 3, 26, NULL, 1, NULL),
(83, '2025-09-20', '2027-09-20', 'Vigente', 89999999, 7, 13, '2027-09-20', 5, NULL),
(84, '2026-09-23', '2027-09-23', 'Vigente', 787999, 3, 26, '2027-09-23', 5, NULL),
(85, '2025-09-20', '2027-09-20', 'Vigente', 65788, 3, 29, '2027-09-20', 5, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(100) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `superficie` decimal(10,0) DEFAULT NULL,
  `tipInmId` int(5) NOT NULL,
  `uso` varchar(20) NOT NULL,
  `precio` int(11) NOT NULL,
  `latitud` decimal(10,0) DEFAULT NULL,
  `longitud` decimal(10,0) DEFAULT NULL,
  `estado` int(3) NOT NULL,
  `propietarioId` int(11) NOT NULL,
  `imagenes` varchar(5000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `ambientes`, `superficie`, `tipInmId`, `uso`, `precio`, `latitud`, `longitud`, `estado`, `propietarioId`, `imagenes`) VALUES
(1, 'Mitre 878, Ciudad de San Luis', 3, 50, 1, 'Residencial', 25000, 10, 20, 1, 1, NULL),
(3, 'Rivadavia 523, Merlo', 1, 10, 2, 'Comercial', 45500, 45, 65, 2, 11, '/uploads/inmuebles/5ea86898-ddca-4a40-9000-4593d9dc4657_INMO.png'),
(7, 'San Justo 4555, Villa Larca, San Luis', 2, 70, 1, 'Comercial', 35000, 0, 0, 3, 4, NULL),
(13, 'Colon 2542, Mendoza Capital', 4, 76, 2, 'Comercial', 67000, 0, 0, 1, 13, '/uploads/inmuebles/11e57bcf-c516-44a6-96f5-eb01c9d54029_Ubicacion terreno.png'),
(14, 'Belgrano 45, San Luis', 5, 90, 1, 'Comercial', 80000, 0, 0, 3, 14, NULL),
(20, 'Chacabuco 584', 3, 100, 1, 'Residencial', 1000000, NULL, NULL, 1, 14, '/uploads/inmuebles/ed8c04bc-c1fd-4fdf-b10b-dd882bc64e1b_Captura de pantalla 2025-07-13 123409.png'),
(22, 'Este voy a dar de baja', 4, 152, 2, 'Residencial', 4545, 53, 54, 3, 4, '/uploads/inmuebles/916529eb-81ee-4537-a56d-0cbfc58ad753_Terreno venta.png'),
(23, 'Peron 111, Mercedes', 2, 30, 3, 'Residencial', 100000, 45445, 56445, 1, 4, '/uploads/inmuebles/7e50ceee-67af-480e-88d4-cd20f3779d85_Terreno Barrancas.png'),
(24, 'Salta 458, Salta Capital', 5, 100, 1, 'Residencial', 5222, 546, 56, 2, 11, '/uploads/inmuebles/69d76abd-5c7c-4c8d-932c-b6e59e380ce5_sudamerica.jpg'),
(25, 'Mexico 6347, San Luis Capital', 2, 100, 3, 'Residencial', 45455, 45, 45, 3, 6, NULL),
(26, 'Junin 545 Capital', 5, 40, 2, 'Residencial', 46666000, 88, 8707, 1, 1, NULL),
(27, 'Ruta 7 km 800 puerta 6', 10, 5000, 3, 'Comercial', 86000000, 545, 87451, 1, 1, '/uploads/inmuebles/67938a3e-2818-41a4-b739-cb342177677b_casagob.jpg'),
(28, 'Lopez 4545, San Martin', 5, 20, 1, 'Residencial', 4656878, 346, 564, 3, 34, '/uploads/inmuebles/bfa3326c-82bb-45ec-8244-fa167e89212e_sudamerica.png'),
(29, 'hghgh 5678, Ciudad de San Luis', 5, 98, 2, 'Comercial', 87, 365437, 7656765, 3, 34, NULL);

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
(7, 'Jose', 'Sexto', '45453533', '2645548844', 'marilau@gmail.com'),
(9, 'Roberto', 'Monez Ruiz', '5343233', '45455444', 'monezruiz@gobernator.com'),
(13, 'Carla', 'Peterson', '1234567', '45468888', 'juna@palomino.com'),
(20, 'Pablo', 'Podesta', '6776576', '5844558748', 'mirco@gmail.com'),
(21, 'Sofia', 'Lorens', '47874554', '0000', 'sofia@lorenz.com.ar'),
(24, 'Cluadio', 'Coggi', '12435435', '24243213', 'claudio@gov.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `nroPago` int(11) NOT NULL,
  `fechaPago` datetime NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `contratoId` int(11) NOT NULL,
  `usuarioIdAlta` int(11) DEFAULT NULL,
  `usuarioIdBaja` int(11) DEFAULT NULL,
  `concepto` varchar(500) DEFAULT NULL,
  `estado` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `nroPago`, `fechaPago`, `importe`, `contratoId`, `usuarioIdAlta`, `usuarioIdBaja`, `concepto`, `estado`) VALUES
(33, 1, '2022-04-22 00:00:00', 30000.00, 49, 1, 5, NULL, 'Anulado'),
(35, 1, '2022-04-22 00:00:00', 30000.00, 51, NULL, NULL, 'c', 'Abonado'),
(36, 0, '2025-09-04 00:00:00', 5454.00, 65, NULL, NULL, 'Pago alquiler bienes 1', 'Abonado'),
(37, 0, '2025-09-04 10:28:18', 5454.00, 65, NULL, 5, NULL, 'Anulado'),
(39, 1, '2025-09-04 00:00:00', 30000.00, 52, NULL, NULL, 'Pago alquiler 1', 'Abonado'),
(40, 2, '2025-09-04 00:00:00', 30000.00, 51, NULL, NULL, 'PAgo 2', 'Anulado'),
(41, 1, '2025-09-04 00:00:00', 55555.00, 66, NULL, NULL, 'Pago 1', 'Anulado'),
(42, 0, '2025-09-05 09:51:58', 345235.00, 67, NULL, NULL, 'Pago por revocación antes de la mitad del contrato', 'Debe'),
(43, 0, '2025-09-05 09:51:58', 345235.00, 67, NULL, NULL, 'Pago por revocación antes de la mitad del contrato', 'Debe'),
(44, 1, '2025-09-05 00:00:00', 5000000.00, 68, NULL, NULL, 'PAgo 1', 'Anulado'),
(45, 0, '2025-09-05 21:04:19', 11223.00, 74, NULL, NULL, 'Pago por revocación antes de la mitad del contrato', 'Anulado'),
(46, 0, '2025-09-05 21:04:19', 11223.00, 74, NULL, NULL, 'Pago por revocación antes de la mitad del contrato', 'Anulado'),
(47, 3, '2025-09-06 00:00:00', 11223.00, 74, NULL, NULL, 'Pago alquiler 3', 'Abonado'),
(49, 2, '2025-09-07 00:00:00', 5000000.00, 68, NULL, NULL, 'Pago alquiler 1', 'Abonado'),
(54, 2, '2025-09-09 00:00:00', 55555.00, 66, NULL, NULL, 'PAgo 2', 'Anulado'),
(55, 3, '2025-09-09 00:00:00', 55555.00, 66, NULL, NULL, 'Pago alquiler bienes 1', 'Abonado'),
(56, 4, '2025-09-09 00:00:00', 55555.00, 66, NULL, NULL, 'Pago alquiler bienes 1', 'Abonado'),
(57, 3, '2025-09-09 00:00:00', 5000000.00, 68, NULL, NULL, 'Pago 3', 'Pendiente'),
(62, 1, '2025-09-09 00:00:00', 50000.00, 53, NULL, NULL, 'Pago atrassado 1', 'Abonado'),
(63, 2, '2025-09-09 00:00:00', 50000.00, 53, NULL, NULL, 'Pago atrasado 2', 'Abonado'),
(65, 3, '2025-09-09 00:00:00', 100000.00, 53, NULL, NULL, 'Pago multa por revocación de contrato', 'Abonado'),
(66, 3, '2025-09-11 00:00:00', 30000.00, 51, NULL, NULL, 'Pago 3', 'Abonado'),
(67, 5, '2025-09-11 00:00:00', 111110.00, 66, NULL, NULL, 'Pago multa por revocación de contrato', 'Abonado'),
(68, 3, '2025-09-20 00:00:00', 250000.00, 65, 3, NULL, 'Pago alquiler 3', 'Abonado'),
(69, 2, '2025-09-20 00:00:00', 30000.00, 52, 5, NULL, 'PAgo 2', 'Abonado'),
(70, 3, '2025-09-20 00:00:00', 30000.00, 52, NULL, NULL, 'Pago multa por revocación de contrato', 'Abonado'),
(71, 2, '2025-09-20 00:00:00', 20555.00, 49, 5, NULL, 'Pago alquiler bienes 1', 'Abonado'),
(72, 3, '2025-09-20 00:00:00', 20555.00, 49, 5, 5, 'PAgo 2', 'Anulado'),
(73, 4, '2025-09-20 00:00:00', 500000.00, 65, NULL, NULL, 'Pago multa por revocación de contrato', 'Abonado');

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
(34, 'juan', 'Salvo', '32524564', '325246', 'retete@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposinmuebles`
--

CREATE TABLE `tiposinmuebles` (
  `id` int(5) NOT NULL,
  `descripcion` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposinmuebles`
--

INSERT INTO `tiposinmuebles` (`id`, `descripcion`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Oficina'),
(4, 'Galpon'),
(5, 'Dormitorio');

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
(1, 'Carla ', 'Perez', '/Uploads\\avatar_1.jpg', 'carla@peterson.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 3),
(2, 'Pablo', 'Perez', '/Uploads\\avatar_2.png', 'pablo@perez.com', 'Es8xLXaQWGPhWN3ndWBEt8ZN7E8T+pDeqi210bMoJsI=', 3),
(3, 'Mauro', 'Rol', '/Uploads/avatar_3.jpg', 'maurorol@gmail.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 3),
(5, 'Dora', 'Gutierrez', '/Uploads/avatar_5.jpg', 'dora@gmail.com', 'GjzAhuy78NH4O47XGFAHPsEk/lJVCR72X7szOdJVPJA=', 2),
(7, 'Francisco Solano', 'Lopez', '/Uploads/avatar_7.jpg', 'francisco@ulp.edu', 'Psz07X3Jwky8U64oY9eZW6TiPo7XEAIrJVv6GCCmPzw=', 3);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `usuarioIdAlta` (`usuarioIdAlta`,`usuarioIdBaja`),
  ADD KEY `FK_INQUILINOID` (`inquilinoId`),
  ADD KEY `FK_INMUEBLEID` (`inmuebleId`),
  ADD KEY `FKUSUARIOS_CONTRATOS_BAJA` (`usuarioIdBaja`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `DELETE_INMUEBLE_CONTRATOS` (`propietarioId`),
  ADD KEY `tipInmId` (`tipInmId`);

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
  ADD UNIQUE KEY `usuarioIdAlta` (`usuarioIdAlta`,`usuarioIdBaja`),
  ADD KEY `FK_CONTRATOID` (`contratoId`),
  ADD KEY `FKUSUARIOS_PAGOS_BAJA` (`usuarioIdBaja`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=86;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=74;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
  MODIFY `id` int(5) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FKUSUARIOS_CONTRATOS_ALTA` FOREIGN KEY (`usuarioIdAlta`) REFERENCES `usuarios` (`id`),
  ADD CONSTRAINT `FKUSUARIOS_CONTRATOS_BAJA` FOREIGN KEY (`usuarioIdBaja`) REFERENCES `usuarios` (`id`),
  ADD CONSTRAINT `FK_INMUEBLEID` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `FK_INQUILINOID` FOREIGN KEY (`inquilinoId`) REFERENCES `inquilinos` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `DELETE_INMUEBLE_CONTRATOS` FOREIGN KEY (`propietarioId`) REFERENCES `propietarios` (`id`),
  ADD CONSTRAINT `FK_TIPINM_INMUEBLES` FOREIGN KEY (`tipInmId`) REFERENCES `tiposinmuebles` (`id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FKUSUARIOS_PAGOS_BAJA` FOREIGN KEY (`usuarioIdBaja`) REFERENCES `usuarios` (`id`),
  ADD CONSTRAINT `FK_CONTRATOID` FOREIGN KEY (`contratoId`) REFERENCES `contratos` (`id`),
  ADD CONSTRAINT `FK_USUARIOS_PAGOS_ALTA` FOREIGN KEY (`usuarioIdAlta`) REFERENCES `usuarios` (`id`);

DELIMITER $$
--
-- Eventos
--
CREATE DEFINER=`root`@`localhost` EVENT `actualizar_estado_contratos` ON SCHEDULE EVERY 1 DAY STARTS '2025-09-20 13:25:58' ON COMPLETION NOT PRESERVE ENABLE DO UPDATE contratos
    SET estado = 'No vigente'
    WHERE estado = 'Vigente'
      AND fechaFinAnt IS NOT NULL
      AND fechaFinAnt < CURDATE()$$

DELIMITER ;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
