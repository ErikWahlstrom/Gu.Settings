﻿namespace Gu.Settings.Core
{
    using System;
    using System.Collections.Generic;

    public interface IFileNameRepository 
    {
        /// <see cref="IRepository.Delete(string, bool)"/>
        void Delete(string fileName, bool deleteBackups);

        /// <see cref="IRepository.Exists(string)"/>
        bool Exists(string fileName);

        /// <summary>
        /// Reads from file or cache if caching. 
        /// If caching every read will get the same singleton instance.
        /// Adds the instance to cache if caching.
        /// Starts tracking the if tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <returns></returns>
        T Read<T>(string fileName);

        /// <summary>
        /// Checks:
        /// If there is a file, returns deserialised contents if found one.
        /// If there is a soft delete file.
        /// If there is a backup.
        /// Restores the file and returns deserialised contents if found one.
        /// Adds the instance to cache if caching.
        /// Starts tracking the if tracking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        /// <param name="creator">How to create an instance if no file is found</param>
        /// <returns></returns>
        T ReadOrCreate<T>(string fileName, Func<T> creator);

        /// <summary>
        /// Saves the item to file.
        /// Adds it to cache if caching and first time it is read/saved
        /// Starts tracking if tracking changes.
        /// </summary>
        /// <typeparam name="T">Not used for anything, maybe useful for constraining?</typeparam>
        /// <param name="item"></param>
        /// <param name="fileName">
        /// Filename can be either of:
        /// C:\Temp\FileName.cfg
        /// FileName.cfg
        /// FileName
        /// </param>
        void Save<T>(T item, string fileName);

        /// <summary>
        /// Checks if the instance has any changes compared to the cached value.
        /// The comparer used serializes and compares the bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        /// <returns>Null if not found in cache</returns>
        bool IsDirty<T>(T item, string fileName);

        /// <summary>
        /// Checks if the instance has any changes compared to the cached value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="fileName"></param>
        /// <param name="comparer"></param>
        /// <returns>Null if not found in cache</returns>
        bool IsDirty<T>(T item, string fileName, IEqualityComparer<T> comparer);

        /// <summary>
        /// Returns a deep copy via serialize > derserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        T Clone<T>(T item);
    }
}
