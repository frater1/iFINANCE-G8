-- 1. Insert into AccountCategory
INSERT INTO AccountCategory (Name, Type) VALUES ('Assets', 'Debit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Liabilities', 'Credit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Income', 'Credit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Expenses', 'Debit');

-- 2. Insert into Group_tbl (Groupe)
-- Assuming the AccountCategory IDs are 1 (Assets), 2 (Liabilities), 3 (Income), 4 (Expenses)
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Cash & Equivalents', 1);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Credit Cards', 2);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Salary', 3);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Utilities', 4);

-- 3. Insert into MasterAccount
-- Use Group IDs from above: 1 for Cash & Equivalents, 2 for Credit Cards, etc.
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Checking Account', 1000.00, 1500.00, 1);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Visa Credit', 0.00, -200.00, 2);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Monthly Salary', 0.00, 5000.00, 3);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Electricity Bill', 0.00, -120.00, 4);

-- 4. Insert into UserPassword
-- These records simulate the login credentials; in production, you would store hashed passwords.
INSERT INTO UserPassword (UserName, EncryptedPassword, PasswordExpiryTime, UserAccountExpiryDate)
VALUES ('admin', 'adminpass', '2026-01-01 00:00:00', '2025-12-31 00:00:00');
INSERT INTO UserPassword (UserName, EncryptedPassword, PasswordExpiryTime, UserAccountExpiryDate)
VALUES ('user1', 'user1pass', '2026-01-01 00:00:00', '2025-12-31 00:00:00');

-- 5. Insert into Administrator
-- Here we assume that the admin’s corresponding UserPassword record got ID=1.
INSERT INTO Administrator (Name, DateHired, UserShadow)
VALUES ('Admin One', '2020-01-01', 1);

-- 6. Insert into NonAdminUser
-- Assume that user1’s UserPassword record got ID=2, and that AdministratorID will reference the admin created above (ID=1).
INSERT INTO NonAdminUser (Name, Address, Email, DateHired, DateFinished, AdministratorID, UserShadow)
VALUES ('User One', '123 Main St', 'user1@example.com', '2021-01-01', '2025-12-31', 1, 2);

-- 7. Insert into Transaction
-- Assume NonAdminUser for "User One" has ID=1.
INSERT INTO Transaction (Date, Description, NonAdminUserID)
VALUES ('2025-03-20 10:00:00', 'Deposit paycheck', 1);
INSERT INTO Transaction (Date, Description, NonAdminUserID)
VALUES ('2025-03-21 14:30:00', 'Pay electricity bill', 1);

-- 8. Insert into TransactionLine
-- For the deposit transaction (assume Transaction ID=1), add a line posting to Checking Account (MasterAccount ID=1).
INSERT INTO TransactionLine (CreditedAmount, DebitedAmount, Comment, TransactionID, MasterAccountID)
VALUES (500.00, 0.00, 'Deposit from employer', 1, 1);
-- For the electricity bill transaction (assume Transaction ID=2), add a line deducting payment from Electricity Bill account (MasterAccount ID=4).
INSERT INTO TransactionLine (CreditedAmount, DebitedAmount, Comment, TransactionID, MasterAccountID)
VALUES (0.00, 120.00, 'Paid electricity provider', 2, 4);-- 1. Insert into AccountCategory
INSERT INTO AccountCategory (Name, Type) VALUES ('Assets', 'Debit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Liabilities', 'Credit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Income', 'Credit');
INSERT INTO AccountCategory (Name, Type) VALUES ('Expenses', 'Debit');

-- 2. Insert into Group_tbl (Groupe)
-- Assuming the AccountCategory IDs are 1 (Assets), 2 (Liabilities), 3 (Income), 4 (Expenses)
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Cash & Equivalents', 1);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Credit Cards', 2);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Salary', 3);
INSERT INTO Group_tbl (Name, AccountCategoryID) VALUES ('Utilities', 4);

-- 3. Insert into MasterAccount
-- Use Group IDs from above: 1 for Cash & Equivalents, 2 for Credit Cards, etc.
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Checking Account', 1000.00, 1500.00, 1);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Visa Credit', 0.00, -200.00, 2);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Monthly Salary', 0.00, 5000.00, 3);
INSERT INTO MasterAccount (Name, OpeningAmount, ClosingAmount, GroupID)
VALUES ('Electricity Bill', 0.00, -120.00, 4);

-- 4. Insert into UserPassword
-- These records simulate the login credentials; in production, you would store hashed passwords.
INSERT INTO UserPassword (UserName, EncryptedPassword, PasswordExpiryTime, UserAccountExpiryDate)
VALUES ('admin', 'adminpass', '2026-01-01 00:00:00', '2025-12-31 00:00:00');
INSERT INTO UserPassword (UserName, EncryptedPassword, PasswordExpiryTime, UserAccountExpiryDate)
VALUES ('user1', 'user1pass', '2026-01-01 00:00:00', '2025-12-31 00:00:00');

-- 5. Insert into Administrator
-- Here we assume that the admin’s corresponding UserPassword record got ID=1.
INSERT INTO Administrator (Name, DateHired, UserShadow)
VALUES ('Admin One', '2020-01-01', 1);

-- 6. Insert into NonAdminUser
-- Assume that user1’s UserPassword record got ID=2, and that AdministratorID will reference the admin created above (ID=1).
INSERT INTO NonAdminUser (Name, Address, Email, DateHired, DateFinished, AdministratorID, UserShadow)
VALUES ('User One', '123 Main St', 'user1@example.com', '2021-01-01', '2025-12-31', 1, 2);

-- 7. Insert into Transaction
-- Assume NonAdminUser for "User One" has ID=1.
INSERT INTO Transaction (Date, Description, NonAdminUserID)
VALUES ('2025-03-20 10:00:00', 'Deposit paycheck', 1);
INSERT INTO Transaction (Date, Description, NonAdminUserID)
VALUES ('2025-03-21 14:30:00', 'Pay electricity bill', 1);

-- 8. Insert into TransactionLine
-- For the deposit transaction (assume Transaction ID=1), add a line posting to Checking Account (MasterAccount ID=1).
INSERT INTO TransactionLine (CreditedAmount, DebitedAmount, Comment, TransactionID, MasterAccountID)
VALUES (500.00, 0.00, 'Deposit from employer', 1, 1);
-- For the electricity bill transaction (assume Transaction ID=2), add a line deducting payment from Electricity Bill account (MasterAccount ID=4).
INSERT INTO TransactionLine (CreditedAmount, DebitedAmount, Comment, TransactionID, MasterAccountID)
VALUES (0.00, 120.00, 'Paid electricity provider', 2, 4);


SELECT * FROM AccountCategory;

