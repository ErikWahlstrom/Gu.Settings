﻿namespace Gu.Settings
{
    using System;

    public sealed class AutoSaveSetting
    {
        private AutoSaveSetting(AutoSaveMode mode, TimeSpan time, bool createBackup, string fileName)
        {
            Mode = mode;
            Time = time;
            CreateBackup = createBackup;
            FileName = fileName;
        }

        public AutoSaveMode Mode { get; private set; }

        public TimeSpan Time { get; private set; }

        public string FileName { get; set; }

        public bool CreateBackup { get; set; }

        /// <summary>
        /// Saves automatically every x seconds
        /// </summary>
        /// <param name="saveEvery"></param>
        /// <param name="createBackup">Creates a backup when saving</param>
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting OnSchedule(TimeSpan saveEvery, bool createBackup, string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.OnSchedule, saveEvery, createBackup, fileName);
        }

        /// <summary>
        /// Saves on propertychange
        /// </summary>
        /// <param name="createBackup">Creates a backup when saving</param>
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting OnChanged(bool createBackup, string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.OnChanged, TimeSpan.Zero, createBackup, fileName);
        }

        /// <summary>
        /// Saves on propertychange but waits buffertime after last change before saving.
        /// </summary>
        /// <param name="bufferTime"></param>
        /// <param name="createBackup">Creates a backup when saving</param> 
        /// <param name="fileName">If left empty the filename is the name of the type</param>
        /// <returns></returns>
        public static AutoSaveSetting Deferred(TimeSpan bufferTime, bool createBackup, string fileName = null)
        {
            return new AutoSaveSetting(AutoSaveMode.Deferred, bufferTime, createBackup, fileName);
        }
    }
}