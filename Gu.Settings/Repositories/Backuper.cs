﻿namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Internals;
    using IO;

    public class Backuper : IBackuper
    {
        public Backuper(BackupSettings setting)
        {
            Setting = setting;
            setting.Directory.CreateIfNotExists();
        }

        public static readonly IBackuper None = new NullBackuper();

        public IRepository Repository { get; set; }

        public BackupSettings Setting { get; private set; }

        public virtual void Backup<T>()
        {
            var file = FileHelper.CreateFileInfo<T>(Repository.Setting);
            Backup(file);
        }

        public virtual void Backup(FileInfo file)
        {
            if (!Setting.CreateBackups)
            {
                return;
            }
            var backupFile = BackupFile.CreateFor(file, Setting);
            Backup(file, backupFile);
        }

        public virtual void Backup(FileInfo file, FileInfo backup)
        {
            PurgeBackups(file);
            FileHelper.Backup(file, backup);
        }

        public virtual void Restore<T>()
        {
            var file = FileHelper.CreateFileInfo<T>(Repository.Setting);
            Restore(file);
        }

        public virtual void Restore(FileInfo file)
        {
            var backup = BackupFile.GetRestoreFileFor(file, Setting);
            Restore(file, backup);
        }

        public virtual void Restore(FileInfo file, FileInfo backup)
        {
            FileHelper.Restore(file, backup);
        }

        public virtual void PurgeBackups(FileInfo file)
        {
            var allBackups = BackupFile.GetAllBackupsFor(file, Setting);
            if (allBackups.Count == 0)
            {
                return;
            }
            if (Setting.NumberOfBackups > 0)
            {
                while (allBackups.Count > Setting.NumberOfBackups) // this is not efficient but the number of backups should be low
                {
                    var backupFile = allBackups.MinBy(x => x.TimeStamp);
                    backupFile.File.HardDelete();
                    allBackups.Remove(backupFile);
                }
            }
            if (Setting.MaxAgeInDays > 0 && Setting.MaxAgeInDays < Int32.MaxValue)
            {
                while (true) // this is not efficient but the number of backups should be low
                {
                    var backupFile = allBackups.MinBy(x => x.TimeStamp);
                    var days = (DateTime.Now - backupFile.TimeStamp).Days;
                    if (days < Setting.MaxAgeInDays)
                    {
                        break;
                    }
                    backupFile.File.HardDelete();
                    allBackups.Remove(backupFile);
                }
            }
        }

        [Obsolete("Implement")]
        private void MoveBackups<T>()
        {
        }
    }
}
