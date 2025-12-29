-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: %
-- Erstellungszeit: 29. Dez 2025 um 21:20
-- Server-Version: 11.8.2-MariaDB-1 from Debian-log
-- PHP-Version: %

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `birthdaybot`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `birthdays`
--

CREATE TABLE `birthdays` (
  `id` int(11) NOT NULL,
  `uid` bigint(11) NOT NULL,
  `guid` bigint(11) NOT NULL,
  `month` int(11) NOT NULL,
  `day` int(11) NOT NULL,
  `year` int(11) DEFAULT NULL,
  `lastPosted` bigint(20) NOT NULL DEFAULT -1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `server`
--

CREATE TABLE `server` (
  `id` int(11) NOT NULL,
  `guid` bigint(20) NOT NULL,
  `channelid` bigint(20) DEFAULT 0,
  `modrole` bigint(20) DEFAULT 0,
  `posthour` int(11) NOT NULL DEFAULT 0,
  `postminute` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `birthdays`
--
ALTER TABLE `birthdays`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `user-guild-unique` (`guid`,`uid`);

--
-- Indizes für die Tabelle `server`
--
ALTER TABLE `server`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `guildid` (`guid`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `birthdays`
--
ALTER TABLE `birthdays`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `server`
--
ALTER TABLE `server`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
