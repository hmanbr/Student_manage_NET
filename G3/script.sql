DROP DATABASE IF EXISTS SWP;

CREATE DATABASE SWP;

USE SWP;

CREATE TABLE `User` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Email` VARCHAR(191) NOT NULL,
    `DomainSettingId` INTEGER NOT NULL,
    `RoleSettingId` INTEGER NOT NULL,
    `Hash` VARCHAR(191) NULL,
    `Confirmed` BOOLEAN NOT NULL DEFAULT false,
    `Blocked` BOOLEAN NOT NULL DEFAULT false,
    `ConfirmToken` VARCHAR(191) NULL,
    `ConfirmTokenVerifyAt` DATETIME NULL,
    `ResetPassToken` VARCHAR(191) NULL,
    `Avatar` VARCHAR(191) NULL,
    `Name` VARCHAR(191) NOT NULL,
    `DateOfBirth` DATETIME(3) NULL,
    `Phone` VARCHAR(15) NULL,
    `Address` VARCHAR(191) NULL,
    `Gender` BOOLEAN NULL,
    `CreatedAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `UpdatedAt` DATETIME(3) NOT NULL,

    INDEX `User_Id_idx`(`Id`),
    INDEX `User_Email_idx`(`Email`),
    UNIQUE INDEX `User_Email_key`(`Email`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Setting` (
    `SettingId` INTEGER NOT NULL AUTO_INCREMENT,
    `Type` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `Value` VARCHAR(191) NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT true,

    INDEX `Setting_Type_Value_idx`(`Type`, `Value`),
    INDEX `Setting_SettingId_idx`(`SettingId`),
    UNIQUE INDEX `Setting_Type_Value_key`(`Type`, `Value`),
    PRIMARY KEY (`SettingId`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Subject` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `SubjectCode` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `Status` BOOLEAN NOT NULL DEFAULT true,
    `MentorId` INTEGER NOT NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `SubjectSetting` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Type` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `Value` VARCHAR(191) NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT true,
    `subjectId` INTEGER NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- AddForeignKey
ALTER TABLE `User` ADD CONSTRAINT `User_DomainSettingId_fkey` FOREIGN KEY (`DomainSettingId`) REFERENCES `Setting`(`SettingId`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `User` ADD CONSTRAINT `User_RoleSettingId_fkey` FOREIGN KEY (`RoleSettingId`) REFERENCES `Setting`(`SettingId`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Subject` ADD CONSTRAINT `Subject_MentorId_fkey` FOREIGN KEY (`MentorId`) REFERENCES `User`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `SubjectSetting` ADD CONSTRAINT `SubjectSetting_subjectId_fkey` FOREIGN KEY (`subjectId`) REFERENCES `Subject`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Administrator', 'ADMIN');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Subject Manager', 'SUBJECT_MANAGER');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Class Manager', 'CLASS_MAMAGER');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Mentor', 'MENTOR');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Student', 'STUDENT');

INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('DOMAIN', 'fpt.edu.vn', 'fpt.edu.vn');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('DOMAIN', 'gmail.com', 'gmail.com');

INSERT INTO `SWP`.`User` (`Email`, `DomainSettingId`, `RoleSettingId`, `Hash`, `Confirmed`, `Blocked`, `ConfirmToken`, `Name`, `Gender`, `CreatedAt`, `UpdatedAt`) VALUES ('admin@fpt.edu.vn', 6, 1, '$2a$11$cxw.dCQrU8IhFUUTkti8E.J1lE4DTN623yAS4xpRSHuX9UbSVsg8K', '1', '0', '', 'Administrator', '1', '2023-09-23 14:32:45.302', '0001-01-01 00:00:00.000');

