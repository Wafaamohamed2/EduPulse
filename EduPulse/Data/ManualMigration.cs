using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EduPulse.Data
{
    public class ManualMigration
    {
        private readonly string _connectionString;

        public ManualMigration(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Executes a SQL script file against the database
        /// </summary>
        /// <param name="scriptPath">Path to the SQL script file</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> ExecuteScriptFileAsync(string scriptPath)
        {
            try
            {
                string script = await File.ReadAllTextAsync(scriptPath);
                return await ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing script file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Executes SQL commands against the database
        /// </summary>
        /// <param name="sql">SQL commands to execute</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> ExecuteScriptAsync(string sql)
        {
            try
            {
                // Split script on GO command
                IEnumerable<string> commandStrings = SplitSqlStatements(sql);
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    foreach (string commandString in commandStrings)
                    {
                        if (!string.IsNullOrWhiteSpace(commandString))
                        {
                            using (SqlCommand command = new SqlCommand(commandString, connection))
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing SQL: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Creates the InitialMigration tables in the database
        /// </summary>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> ExecuteInitialMigrationAsync()
        {
            string migrationSql = @"
-- Create tables
CREATE TABLE [Students] (
    [Id] int NOT NULL IDENTITY,
    [FingerId] nvarchar(450) NULL,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [ProfilePictureUrl] nvarchar(max) NOT NULL,
    [Grade] int NOT NULL,
    [Level] int NOT NULL,
    [Absences] int NOT NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
);

CREATE TABLE [Teachers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [ProfilePictureUrl] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id])
);

CREATE TABLE [UserDevices] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [DeviceToken] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_UserDevices] PRIMARY KEY ([Id])
);

CREATE TABLE [Parents] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [ProfilePictureUrl] nvarchar(max) NOT NULL,
    [Relationship] nvarchar(max) NOT NULL,
    [studentId] int NOT NULL,
    CONSTRAINT [PK_Parents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Parents_Students_studentId] FOREIGN KEY ([studentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Attendances] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [IsPresent] bit NOT NULL,
    [FingerId] nvarchar(max) NULL,
    [studentId] int NOT NULL,
    [TeacherId] int NOT NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_Students_studentId] FOREIGN KEY ([studentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Attendances_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AttendenceRecords] (
    [Id] int NOT NULL IDENTITY,
    [AttendenceDate] datetime2 NOT NULL,
    [IsPresent] bit NOT NULL,
    [FingerId] nvarchar(max) NOT NULL,
    [StudentId] int NOT NULL,
    [TeacherId] int NOT NULL,
    CONSTRAINT [PK_AttendenceRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AttendenceRecords_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttendenceRecords_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Exams] (
    [Id] int NOT NULL IDENTITY,
    [ExamName] nvarchar(max) NOT NULL,
    [TimeLimitInMinutes] int NOT NULL,
    [GoogleFormLink] nvarchar(max) NOT NULL,
    [DeadlineDate] datetime2 NOT NULL,
    [TeacherId] int NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Exams_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id])
);

CREATE TABLE [StudentSubjects] (
    [StudentId] int NOT NULL,
    [TeacherId] int NOT NULL,
    CONSTRAINT [PK_StudentSubjects] PRIMARY KEY ([StudentId], [TeacherId]),
    CONSTRAINT [FK_StudentSubjects_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentSubjects_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ExamResults] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [ExamId] int NOT NULL,
    [Grade] int NOT NULL,
    [DateTaken] datetime2 NOT NULL,
    CONSTRAINT [PK_ExamResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamResults_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamResults_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ExamStudents] (
    [ExamsId] int NOT NULL,
    [StudentsId] int NOT NULL,
    CONSTRAINT [PK_ExamStudents] PRIMARY KEY ([ExamsId], [StudentsId]),
    CONSTRAINT [FK_ExamStudents_Exams_ExamsId] FOREIGN KEY ([ExamsId]) REFERENCES [Exams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamStudents_Students_StudentsId] FOREIGN KEY ([StudentsId]) REFERENCES [Students] ([Id]) ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX [IX_Attendances_studentId] ON [Attendances] ([studentId]);
CREATE INDEX [IX_Attendances_TeacherId] ON [Attendances] ([TeacherId]);
CREATE INDEX [IX_AttendenceRecords_StudentId] ON [AttendenceRecords] ([StudentId]);
CREATE INDEX [IX_AttendenceRecords_TeacherId] ON [AttendenceRecords] ([TeacherId]);
CREATE INDEX [IX_ExamResults_ExamId] ON [ExamResults] ([ExamId]);
CREATE INDEX [IX_ExamResults_StudentId] ON [ExamResults] ([StudentId]);
CREATE INDEX [IX_Exams_TeacherId] ON [Exams] ([TeacherId]);
CREATE INDEX [IX_ExamStudents_StudentsId] ON [ExamStudents] ([StudentsId]);
CREATE INDEX [IX_Parents_studentId] ON [Parents] ([studentId]);
CREATE INDEX [IX_StudentSubjects_TeacherId] ON [StudentSubjects] ([TeacherId]);

-- Create EF migrations history table
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

-- Add migration record
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250512141307_InitialMigration', N'7.0.15');
";

            return await ExecuteScriptAsync(migrationSql);
        }

        /// <summary>
        /// Checks if the database exists and is accessible
        /// </summary>
        /// <returns>True if the database exists and is accessible</returns>
        public async Task<bool> CheckDatabaseConnectionAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Splits a SQL script into individual statements based on GO statements
        /// </summary>
        private IEnumerable<string> SplitSqlStatements(string script)
        {
            return script.Split(new[] { "\nGO", "\ngo", "\nGo", "\ngO" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
} 