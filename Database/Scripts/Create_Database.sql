﻿IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RoleDb')
BEGIN
    CREATE DATABASE RoleDb
END
GO