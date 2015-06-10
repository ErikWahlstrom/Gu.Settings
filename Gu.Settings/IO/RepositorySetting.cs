﻿namespace Gu.Settings
{
    using System;
    using System.IO;

    public class RepositorySetting : IRepositorySetting
    {
        public RepositorySetting(
            bool createBackupOnSave,
            bool isTrackingDirty,
            DirectoryInfo directory,
            string extension = ".cfg",
            string backupExtension = ".old")
        {
            Ensure.NotNullOrEmpty(extension, "extension");
            Ensure.NotNull(directory, "directory");
            if (CreateBackupOnSave)
            {
                Ensure.NotNullOrEmpty(backupExtension, "backupExtension");
            }

            CreateBackupOnSave = createBackupOnSave;
            IsTrackingDirty = isTrackingDirty;
            Directory = directory;
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            Extension = extension;
            if (CreateBackupOnSave && !backupExtension.StartsWith("."))
            {
                backupExtension = "." + backupExtension;
            }
            BackupExtension = backupExtension;
        }

        public bool CreateBackupOnSave { get; private set; }

        public DirectoryInfo Directory { get; private set; }

        public string Extension { get; private set; }

        public string BackupExtension { get; private set; }
        
        public bool IsTrackingDirty { get; private set; }
    }
}
