# iFINANCE App Setup & Run Guide

This guide walks you through the steps to get the **Group8_iFINANCE_APP** up and running locally.

---

## 1. Prerequisites

1. **IDE**
   - Install [Visual Studio Code](https://code.visualstudio.com/) **or**
   - Install [Visual Studio](https://visualstudio.microsoft.com/) (Community or higher)

2. **.NET SDK**
   - Ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed (version 6.0 or later).

3. **Database**
   - Install [MySQL Workbench](https://dev.mysql.com/downloads/workbench/)

---

## 2. VS Code Extensions (if using VS Code)

If you choose **Visual Studio Code**, enhance your development experience by installing these extensions:

| Extension                | ID                                          | Description                                                                            |
|--------------------------|---------------------------------------------|----------------------------------------------------------------------------------------|
| .NET Install Tool        | `ms-dotnettools.vscode-dotnet-install-tool` | Manage and install .NET SDKs/Runtimes within VS Code.                                  |
| .NET MAUI                | `ms-dotnettools.vscode-dotnet-maui`         | Tools for building .NET MAUI (mobile/desktop) applications.                            |
| C#                       | `ms-dotnettools.csharp`                     | Core language support: syntax highlighting, IntelliSense, code navigation (OmniSharp). |
| C# Dev Kit               | `ms-dotnettools.csharp-dev-kit`             | Advanced C# tooling: refactorings, project management, IDE-like productivity features. |

**To install:**
1. Open VS Code and press `Ctrl+Shift+X` to open Extensions.
2. Search by the extension **ID** above.
3. Click **Install**, then reload if prompted.

---

## 3. Clone & Open the Project

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/your-org/Group8_iFINANCE_APP.git
   cd Group8_iFINANCE_APP
   ```

2. Open the project folder in your chosen IDE (VS Code or Visual Studio).

---

## 4. Database Setup

1. Launch **MySQL Workbench** and connect to an existing MySQL instance (or create a new connection).
2. In the SQL editor, execute the following script to create the schema, tables, and a default admin user:

   ```sql
   -- 1) Create the new database
   CREATE DATABASE IF NOT EXISTS `Group8_iFINANCEDB2`
     CHARACTER SET utf8mb4
     COLLATE utf8mb4_unicode_ci;
   USE `Group8_iFINANCEDB2`;

   -- 2) AccountCategories
   CREATE TABLE `AccountCategories` (
     `ID`   INT           NOT NULL AUTO_INCREMENT,
     `name` VARCHAR(255)  NULL,
     `type` VARCHAR(255)  NULL,
     PRIMARY KEY (`ID`)
   ) ENGINE=InnoDB;

   -- 3) UserPasswords
   CREATE TABLE `UserPasswords` (
     `ID`                    INT            NOT NULL AUTO_INCREMENT,
     `userName`              VARCHAR(255)   NOT NULL,
     `encryptedPassword`     VARCHAR(255)   NOT NULL,
     `passwordExpiryTime`    INT            NULL,
     `userAccountExpiryDate` DATETIME       NULL,
     PRIMARY KEY (`ID`)
   ) ENGINE=InnoDB;

   -- 4) Administrators
   CREATE TABLE `Administrators` (
     `ID`              INT           NOT NULL AUTO_INCREMENT,
     `name`            VARCHAR(255)  NOT NULL,
     `dateHired`       DATETIME      NULL,
     `dateFinished`    DATETIME      NULL,
     `UserPassword_ID` INT           NOT NULL,
     PRIMARY KEY (`ID`),
     INDEX (`UserPassword_ID`),
     FOREIGN KEY (`UserPassword_ID`)
       REFERENCES `UserPasswords`(`ID`)
       ON DELETE RESTRICT
   ) ENGINE=InnoDB;

   -- 5) NonAdminUsers
   CREATE TABLE `NonAdminUsers` (
     `ID`               INT           NOT NULL AUTO_INCREMENT,
     `name`             VARCHAR(255)  NOT NULL,
     `address`          VARCHAR(255)  NULL,
     `email`            VARCHAR(255)  NULL,
     `UserPassword_ID`  INT           NOT NULL,
     `Administrator_ID` INT           NOT NULL,
     PRIMARY KEY (`ID`),
     INDEX (`UserPassword_ID`),
     INDEX (`Administrator_ID`),
     FOREIGN KEY (`UserPassword_ID`)
       REFERENCES `UserPasswords`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`Administrator_ID`)
       REFERENCES `Administrators`(`ID`)
       ON DELETE RESTRICT
   ) ENGINE=InnoDB;

   -- 6) Groups
   CREATE TABLE `Groups` (
     `ID`                 INT           NOT NULL AUTO_INCREMENT,
     `name`               VARCHAR(255)  NOT NULL,
     `AccountCategory_ID` INT           NOT NULL,
     `parent_ID`          INT           NULL,
     `CreatedByUserID`    INT           NOT NULL,
     PRIMARY KEY (`ID`),
     INDEX (`AccountCategory_ID`),
     INDEX (`parent_ID`),
     INDEX (`CreatedByUserID`),
     FOREIGN KEY (`AccountCategory_ID`)
       REFERENCES `AccountCategories`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`parent_ID`)
       REFERENCES `Groups`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`CreatedByUserID`)
       REFERENCES `NonAdminUsers`(`ID`)
       ON DELETE CASCADE
   ) ENGINE=InnoDB;

   -- 7) MasterAccounts
   CREATE TABLE `MasterAccounts` (
     `ID`              INT           NOT NULL AUTO_INCREMENT,
     `name`            VARCHAR(255)  NOT NULL,
     `openingAmount`   DOUBLE        NULL,
     `closingAmount`   DOUBLE        NULL,
     `Group_ID`        INT           NOT NULL,
     `NonAdminUser_ID` INT           NOT NULL,
     PRIMARY KEY (`ID`),
     INDEX (`Group_ID`),
     INDEX (`NonAdminUser_ID`),
     FOREIGN KEY (`Group_ID`)
       REFERENCES `Groups`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`NonAdminUser_ID`)
       REFERENCES `NonAdminUsers`(`ID`)
       ON DELETE RESTRICT
   ) ENGINE=InnoDB;

   -- 8) Transactions
   CREATE TABLE `Transactions` (
     `ID`              INT           NOT NULL AUTO_INCREMENT,
     `date`            DATETIME      NOT NULL,
     `description`     VARCHAR(500)  NULL,
     `NonAdminUser_ID` INT           NOT NULL,
     PRIMARY KEY (`ID`),
     INDEX (`NonAdminUser_ID`),
     FOREIGN KEY (`NonAdminUser_ID`)
       REFERENCES `NonAdminUsers`(`ID`)
       ON DELETE CASCADE
   ) ENGINE=InnoDB;

   -- 9) TransactionLines
   CREATE TABLE `TransactionLines` (
     `ID`                 INT           NOT NULL AUTO_INCREMENT,
     `MasterAccounts_ID`  INT           NOT NULL,
     `MasterAccounts1_ID` INT           NOT NULL,
     `Transaction_ID`     INT           NOT NULL,
     `debitedAmount`      DOUBLE        NOT NULL,
     `creditedAmount`     DOUBLE        NOT NULL,
     `comment`            VARCHAR(500)  NULL,
     PRIMARY KEY (`ID`),
     INDEX (`MasterAccounts_ID`),
     INDEX (`MasterAccounts1_ID`),
     INDEX (`Transaction_ID`),
     FOREIGN KEY (`MasterAccounts_ID`)
       REFERENCES `MasterAccounts`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`MasterAccounts1_ID`)
       REFERENCES `MasterAccounts`(`ID`)
       ON DELETE RESTRICT,
     FOREIGN KEY (`Transaction_ID`)
       REFERENCES `Transactions`(`ID`)
       ON DELETE CASCADE
   ) ENGINE=InnoDB;

   -- Optional: Set default creator for existing entries
   ALTER TABLE `Groups`
     MODIFY COLUMN `CreatedByUserID` INT NOT NULL DEFAULT 1;

   USE `Group8_iFINANCEDB2`;

   -- 10) Seed initial admin user credentials
   INSERT INTO `UserPasswords` (`userName`, `encryptedPassword`, `passwordExpiryTime`, `userAccountExpiryDate`)
   VALUES ('admin', 'pass', 90, DATE_ADD(NOW(), INTERVAL 1 YEAR));
   SET @upId = LAST_INSERT_ID();

   INSERT INTO `Administrators` (`name`, `dateHired`, `dateFinished`, `UserPassword_ID`)
   VALUES ('System Administrator', NOW(), NULL, @upId);
   