DROP DATABASE IF EXISTS SWP;

CREATE DATABASE SWP;

USE SWP;

-- CreateTable
CREATE TABLE `User` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Email` VARCHAR(191) NOT NULL,
    `DomainSettingId` INTEGER NOT NULL,
    `RoleSettingId` INTEGER NOT NULL,
    `Hash` VARCHAR(191) NULL,
    `Status` BOOLEAN NULL,
    `ConfirmToken` VARCHAR(191) NULL,
    `ConfirmTokenVerifyAt` DATETIME(3) NULL,
    `ResetPassToken` VARCHAR(191) NULL,
    `Avatar` VARCHAR(191) NULL,
    `Name` VARCHAR(191) NOT NULL,
    `DateOfBirth` DATETIME(3) NULL,
    `Phone` VARCHAR(191) NULL,
    `Address` VARCHAR(191) NULL,
    `Gender` BOOLEAN NULL,
    `Description` TEXT NULL,
    `CreatedAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `UpdatedAt` DATETIME(3) NOT NULL,

    INDEX `User_Id_idx`(`Id`),
    INDEX `User_Email_idx`(`Email`),
    INDEX `User_ConfirmToken_idx`(`ConfirmToken`),
    INDEX `User_ResetPassToken_idx`(`ResetPassToken`),
    UNIQUE INDEX `User_Email_key`(`Email`),
    UNIQUE INDEX `User_ConfirmToken_key`(`ConfirmToken`),
    UNIQUE INDEX `User_ResetPassToken_key`(`ResetPassToken`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Setting` (
    `SettingId` INTEGER NOT NULL AUTO_INCREMENT,
    `Type` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `Value` VARCHAR(191) NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT true,
    `Description` TEXT NULL,

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
    `Description` TEXT NULL,
    `Status` BOOLEAN NOT NULL DEFAULT true,
    `MentorId` INTEGER NULL,

    INDEX `Subject_SubjectCode_idx`(`SubjectCode`),
    INDEX `Subject_Id_idx`(`Id`),
    UNIQUE INDEX `Subject_SubjectCode_key`(`SubjectCode`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `ClassStudentProject` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Status` BOOLEAN NULL,
    `UserId` INTEGER NOT NULL,
    `ProjectId` INTEGER NOT NULL,
    `ClassId` INTEGER NOT NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `SubjectSetting` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Type` VARCHAR(191) NOT NULL,
    `Value` VARCHAR(191) NOT NULL,
    `Status` BOOLEAN NULL,
    `Name` TEXT NULL,
    `SubjectId` INTEGER NOT NULL,

    INDEX `SubjectSetting_Id_idx`(`Id`),
    UNIQUE INDEX `SubjectSetting_Type_Value_key`(`Type`, `Value`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Assignment` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Title` VARCHAR(191) NOT NULL,
    `Description` TEXT NULL,
    `SubjectId` INTEGER NOT NULL,

    INDEX `Assignment_Id_idx`(`Id`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Submit` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `FileUrl` VARCHAR(191) NOT NULL,
    `SubmitTime` DATETIME(3) NOT NULL,
    `Grade` DECIMAL(65, 30) NULL,
    `Comment` TEXT NULL,
    `CommentTime` DATETIME(3) NULL,
    `StudentComment` TEXT NULL,
    `StudentCommentTime` DATETIME(3) NULL,
    `projectId` INTEGER NOT NULL,
    `ClassAssignmentId` VARCHAR(191) NOT NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Class` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(191) NOT NULL,
    `Description` TEXT NULL,
    `GitLabGroupId` INTEGER NULL,
    `SubjectId` INTEGER NULL,
    `Status` BOOLEAN NULL,

    INDEX `Class_Name_idx`(`Name`),
    INDEX `Class_GitLabGroupId_idx`(`GitLabGroupId`),
    UNIQUE INDEX `Class_Name_key`(`Name`),
    UNIQUE INDEX `Class_GitLabGroupId_key`(`GitLabGroupId`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `ClassSetting` (
    `SettingId` INTEGER NOT NULL AUTO_INCREMENT,
    `Type` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `Value` VARCHAR(191) NOT NULL,
    `Description` TEXT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT true,
    `classId` INTEGER NULL,

    PRIMARY KEY (`SettingId`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `ClassAssignment` (
    `Key` VARCHAR(191) NOT NULL,
    `AssignmentId` INTEGER NOT NULL,
    `ClassId` INTEGER NOT NULL,
    `StartDate` DATETIME(3) NULL,
    `EndDate` DATETIME(3) NULL,
    `MilestoneId` INTEGER NULL,

    INDEX `ClassAssignment_AssignmentId_ClassId_idx`(`AssignmentId`, `ClassId`),
    INDEX `ClassAssignment_Key_idx`(`Key`),
    UNIQUE INDEX `ClassAssignment_AssignmentId_ClassId_key`(`AssignmentId`, `ClassId`),
    PRIMARY KEY (`Key`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Project` (
    `Id` INTEGER NOT NULL AUTO_INCREMENT,
    `ProjectCode` VARCHAR(191) NOT NULL,
    `EnglishName` VARCHAR(191) NOT NULL,
    `GroupName` VARCHAR(191) NOT NULL,
    `VietNameseName` VARCHAR(191) NULL,
    `Status` ENUM('PENDING', 'ACTIVE', 'INACTIVE') NOT NULL DEFAULT 'INACTIVE',
    `Description` TEXT NULL,
    `MentorId` INTEGER NULL,
    `GitLabProjectId` INTEGER NULL,
    `ClassId` INTEGER NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Milestone` (
    `Id` INTEGER NOT NULL,
    `Iid` INTEGER NOT NULL,
    `Title` VARCHAR(191) NOT NULL,
    `Description` TEXT NULL,
    `DueDate` DATETIME(3) NULL,
    `GroupId` INTEGER NULL,
    `ProjectId` INTEGER NULL,
    `StartDate` DATETIME(3) NULL,
    `State` VARCHAR(191) NOT NULL,
    `CreatedAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `UpdatedAt` DATETIME(3) NOT NULL,

    UNIQUE INDEX `Milestone_Id_key`(`Id`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `GitLabUser` (
    `Id` INTEGER NOT NULL,
    `Username` VARCHAR(191) NOT NULL,
    `Name` VARCHAR(191) NOT NULL,
    `State` VARCHAR(191) NOT NULL,
    `Locked` BOOLEAN NOT NULL DEFAULT false,
    `AvatarUrl` VARCHAR(191) NULL,
    `WebUrl` VARCHAR(191) NOT NULL,
    `UserId` INTEGER NULL,

    UNIQUE INDEX `GitLabUser_Username_key`(`Username`),
    UNIQUE INDEX `GitLabUser_UserId_key`(`UserId`),
    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Assignee` (
    `IssueId` INTEGER NOT NULL,
    `GitLabUserId` INTEGER NOT NULL,

    UNIQUE INDEX `Assignee_IssueId_GitLabUserId_key`(`IssueId`, `GitLabUserId`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `Issue` (
    `Id` INTEGER NOT NULL,
    `Iid` INTEGER NOT NULL,
    `Title` VARCHAR(191) NOT NULL,
    `Description` TEXT NULL,
    `Status` ENUM('closed', 'opened') NOT NULL,
    `ClosedAt` DATETIME(3) NULL,
    `MilestoneId` INTEGER NULL,
    `ClosedById` INTEGER NULL,
    `AuthorId` INTEGER NOT NULL,
    `AssigneeId` INTEGER NOT NULL,
    `CreatedAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `UpdatedAt` DATETIME(3) NOT NULL,
    `ProjectId` INTEGER NULL,
    `Complexity` ENUM('Complex', 'Medium', 'Simple') NULL,
    `Quality` ENUM('High', 'Medium', 'Low') NULL,

    PRIMARY KEY (`Id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- AddForeignKey
ALTER TABLE `User` ADD CONSTRAINT `User_DomainSettingId_fkey` FOREIGN KEY (`DomainSettingId`) REFERENCES `Setting`(`SettingId`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `User` ADD CONSTRAINT `User_RoleSettingId_fkey` FOREIGN KEY (`RoleSettingId`) REFERENCES `Setting`(`SettingId`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Subject` ADD CONSTRAINT `Subject_MentorId_fkey` FOREIGN KEY (`MentorId`) REFERENCES `User`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassStudentProject` ADD CONSTRAINT `ClassStudentProject_UserId_fkey` FOREIGN KEY (`UserId`) REFERENCES `User`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassStudentProject` ADD CONSTRAINT `ClassStudentProject_ProjectId_fkey` FOREIGN KEY (`ProjectId`) REFERENCES `Project`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassStudentProject` ADD CONSTRAINT `ClassStudentProject_ClassId_fkey` FOREIGN KEY (`ClassId`) REFERENCES `Class`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `SubjectSetting` ADD CONSTRAINT `SubjectSetting_SubjectId_fkey` FOREIGN KEY (`SubjectId`) REFERENCES `Subject`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Assignment` ADD CONSTRAINT `Assignment_SubjectId_fkey` FOREIGN KEY (`SubjectId`) REFERENCES `Subject`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Submit` ADD CONSTRAINT `Submit_projectId_fkey` FOREIGN KEY (`projectId`) REFERENCES `Project`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Submit` ADD CONSTRAINT `Submit_ClassAssignmentId_fkey` FOREIGN KEY (`ClassAssignmentId`) REFERENCES `ClassAssignment`(`Key`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Class` ADD CONSTRAINT `Class_SubjectId_fkey` FOREIGN KEY (`SubjectId`) REFERENCES `Subject`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassSetting` ADD CONSTRAINT `ClassSetting_classId_fkey` FOREIGN KEY (`classId`) REFERENCES `Class`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassAssignment` ADD CONSTRAINT `ClassAssignment_ClassId_fkey` FOREIGN KEY (`ClassId`) REFERENCES `Class`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassAssignment` ADD CONSTRAINT `ClassAssignment_AssignmentId_fkey` FOREIGN KEY (`AssignmentId`) REFERENCES `Assignment`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `ClassAssignment` ADD CONSTRAINT `ClassAssignment_MilestoneId_fkey` FOREIGN KEY (`MilestoneId`) REFERENCES `Milestone`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Project` ADD CONSTRAINT `Project_MentorId_fkey` FOREIGN KEY (`MentorId`) REFERENCES `User`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Project` ADD CONSTRAINT `Project_ClassId_fkey` FOREIGN KEY (`ClassId`) REFERENCES `Class`(`GitLabGroupId`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Milestone` ADD CONSTRAINT `Milestone_GroupId_fkey` FOREIGN KEY (`GroupId`) REFERENCES `Class`(`GitLabGroupId`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Milestone` ADD CONSTRAINT `Milestone_ProjectId_fkey` FOREIGN KEY (`ProjectId`) REFERENCES `Project`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `GitLabUser` ADD CONSTRAINT `GitLabUser_UserId_fkey` FOREIGN KEY (`UserId`) REFERENCES `User`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Assignee` ADD CONSTRAINT `Assignee_IssueId_fkey` FOREIGN KEY (`IssueId`) REFERENCES `Issue`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Assignee` ADD CONSTRAINT `Assignee_GitLabUserId_fkey` FOREIGN KEY (`GitLabUserId`) REFERENCES `GitLabUser`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Issue` ADD CONSTRAINT `Issue_ClosedById_fkey` FOREIGN KEY (`ClosedById`) REFERENCES `GitLabUser`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Issue` ADD CONSTRAINT `Issue_MilestoneId_fkey` FOREIGN KEY (`MilestoneId`) REFERENCES `Milestone`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Issue` ADD CONSTRAINT `Issue_AuthorId_fkey` FOREIGN KEY (`AuthorId`) REFERENCES `GitLabUser`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Issue` ADD CONSTRAINT `Issue_AssigneeId_fkey` FOREIGN KEY (`AssigneeId`) REFERENCES `GitLabUser`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `Issue` ADD CONSTRAINT `Issue_ProjectId_fkey` FOREIGN KEY (`ProjectId`) REFERENCES `Project`(`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Administrator', 'ADMIN');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Subject Manager', 'SUBJECT_MANAGER');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Class Manager', 'CLASS_MANAGER');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Mentor', 'MENTOR');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('ROLE', 'Student', 'STUDENT');

INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('DOMAIN', 'fpt.edu.vn', 'fpt.edu.vn');
INSERT INTO `SWP`.`Setting` (`Type`, `Name`, `Value`) VALUES ('DOMAIN', 'gmail.com', 'gmail.com');

INSERT INTO `SWP`.`User` (`Email`, `DomainSettingId`, `RoleSettingId`, `Hash`, `Status`, `Name`, `Gender`, `CreatedAt`, `UpdatedAt`) VALUES ('admin@fpt.edu.vn', 6, 1, '$2a$11$cxw.dCQrU8IhFUUTkti8E.J1lE4DTN623yAS4xpRSHuX9UbSVsg8K',true, 'Administrator', '1', '2023-09-23 14:32:45.302', '0001-01-01 00:00:00.000');
INSERT INTO `SWP`.`User` (`Email`, `DomainSettingId`, `RoleSettingId`, `Hash`, `Status`, `Name`, `Gender`, `CreatedAt`, `UpdatedAt`) VALUES ('subject_manager@fpt.edu.vn', 6, 2, '$2a$11$DxRisl20ebF0JUabLWNyHeCxSjin6TZBLrVQyhCTHtroqCtzRLZxC',true, 'Subject Manager', '1', '2023-09-23 14:32:45.302', '0001-01-01 00:00:00.000');
INSERT INTO `SWP`.`User` (`Email`, `DomainSettingId`, `RoleSettingId`, `Hash`, `Status`, `Name`, `Gender`, `CreatedAt`, `UpdatedAt`) VALUES ('class_manager@fpt.edu.vn', 6, 3, '$2a$11$dWdwVbzKlOWKR7VHwywwH.rt0Tqxar9.8.Y2I46OSYRwymKKVTEnW',true, 'Class Manager', '1', '2023-09-23 14:32:45.302', '0001-01-01 00:00:00.000');

INSERT INTO `SWP`.`Subject` (`SubjectCode`,`Name`,`Description`) VALUES ('PRF192', 'Programming Fundamentals', 'This is Programming Fundamentals Subject');
INSERT INTO `SWP`.`Subject` (`SubjectCode`,`Name`,`Description`) VALUES ('DBI202', 'Introduction to Databases', 'This is Introduction to Databases Subject');
INSERT INTO `SWP`.`Subject` (`SubjectCode`,`Name`,`Description`) VALUES ('NWC202', 'Computer Networking', 'This is Computer Networking Subject');
INSERT INTO `SWP`.`Subject` (`SubjectCode`,`Name`,`Description`) VALUES ('PRN292', '.NET and C#', 'This is .NET and C# Subject');

INSERT INTO `SWP`.`Class` (`Name`, `Description`, `GitLabGroupId`, `SubjectId`, `Status`) VALUES ('SE1735', 'This is SE1735 class', '76753267', '1', '1');

-- minh insert random
INSERT INTO project (ProjectCode, EnglishName, GroupName, VietNameseName, Status, Description, MentorId, GitLabProjectId, ClassId)
VALUES
( 'SPP', 'student portal', 'G3', 'ngu nhu lon', 'PENDING', 'xam lon', '1', '213124', '76753267'),
( 'LMS', 'learning stuff', 'G2', 'binh thuong', 'PENDING', 'bruh', '1', '432423', '76753267'),
( 'CMS', 'CMS fake', 'G1', 'ahhhhhhh', 'PENDING', 'fpt hahaha', '1', '232242', '76753267');

INSERT INTO class (Name, Description, GitLabGroupId, SubjectId, Status)
VALUES
('JP1234', 'This is JP1234 class', '12342', '2', '1'),
('SE9999', 'This is SE9999 class', '4324324', '3', '1');

INSERT INTO assignment (Title, Description, SubjectId)
VALUES
('Inter 1', 'prf inter 1', '1'),
('Inter 2', 'prf inter 2', '1'),
('Inter 3', 'prf inter 3', '1');

INSERT INTO `SWP`.`Milestone`
(`Id`,
`Iid`,
`Title`,
`Description`,
`DueDate`,
`GroupId`,
`ProjectId`,
`StartDate`,
`State`,
`CreatedAt`,
`UpdatedAt`)
VALUES
(4329870,
5,
'this is milestone 5',
null,
'2023-10-31 00:00:00.000',
76753267,
null,
'2023-10-29 00:00:00.000',
'active',
'2023-10-29 00:00:00.000',
'2023-10-29 00:00:00.000');




INSERT INTO classassignment (`Key`, AssignmentId, ClassId, StartDate, EndDate, MilestoneId)
VALUES
('PRJ192_SE1735_ASS1', 1, 1, '2023-09-23 14:32:45.302', '2023-10-23 14:32:45.302', 4329870),
('PRJ192_SE1735_ASS2', 2, 1, '2023-10-23 14:32:45.302', '2023-11-23 14:32:45.302', 4329870),
('PRJ192_SE1735_ASS3', 3, 1, '2023-11-23 14:32:45.302', '2023-11-30 14:32:45.302', 4329870);
