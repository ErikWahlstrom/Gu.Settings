﻿namespace Gu.Settings.Core.Tests.IO
{
    using System.IO;
    using System.Threading.Tasks;
    using Gu.Settings.Core;

    using NUnit.Framework;

    public class FileHelperTests
    {
        private readonly DirectoryInfo Directory;
        private FileInfo _file;
        private FileInfo _softDeleteFile;
        private FileInfo _backup;
        private FileInfo _backupSoftDelete;

        public FileHelperTests()
        {
            Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            Directory.CreateIfNotExists();

        }

        [SetUp]
        public void SetUp()
        {
            _file = Directory.CreateFileInfoInDirectory("Setting.cfg");
            _softDeleteFile = _file.GetSoftDeleteFileFor();
            _backup = _file.WithNewExtension(BackupSettings.DefaultExtension);
            _backupSoftDelete = _backup.GetSoftDeleteFileFor();
            _backup.VoidCreate();
        }

        [TearDown]
        public void TearDown()
        {
            _file.Delete();
            _backup.Delete();
            _softDeleteFile.Delete();
        }

        [Test]
        public void HardDeleteWhenNoFile()
        {
            _file.HardDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void HardDeleteWhenNoSoftFile()
        {
            _file.VoidCreate();
            _file.HardDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void HardDeleteWhenHasSoftFile()
        {
            _file.VoidCreate();
            _softDeleteFile.VoidCreate();
            _file.HardDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void HardDeleteWhenOnlySoftFile()
        {
            _softDeleteFile.VoidCreate();
            _file.HardDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void SoftDeleteWhenNoFile()
        {
            _file.SoftDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void SoftDeleteWhenNoSoftFile()
        {
            _file.VoidCreate();
            _file.SoftDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _softDeleteFile);
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void SoftDeleteWhenHasSoftFile()
        {
            _file.WriteAllText("File");
            _softDeleteFile.WriteAllText("Soft");
            _file.SoftDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _softDeleteFile);
            Assert.AreEqual("File", _softDeleteFile.ReadAllText());
            AssertFile.Exists(true, _backup);
        }

        [Test]
        public void BackupWhenNoFile()
        {
            _backup.Delete();
            AssertFile.Exists(false, _file);
            FileHelper.Backup(_file, _backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _backup);
        }

        [Test]
        public void BackupWhenNoFileButHasBackup()
        {
            _backup.WriteAllText("Backup");
            AssertFile.Exists(false, _file);
            FileHelper.Backup(_file, _backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _backup);
            Assert.AreEqual("Backup", _backup.ReadAllText());
        }

        [Test]
        public void BackupWhenNoBackupFile()
        {
            _file.WriteAllText("File");
            FileHelper.Backup(_file, _backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _backup);
            Assert.AreEqual("File", _backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFile()
        {
            _file.WriteAllText("File");
            _backup.WriteAllText("Backup");
            FileHelper.Backup(_file, _backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _backup);
            Assert.AreEqual("File", _backup.ReadAllText());
        }

        [Test]
        public void BackupWhenHasBackupFileAndBackupHasSoftDelete()
        {
            _backupSoftDelete.WriteAllText("OldSoft");
            _file.WriteAllText("File");
            _backup.WriteAllText("Backup");
            FileHelper.Backup(_file, _backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _backup);
            Assert.AreEqual("File", _backup.ReadAllText());
            AssertFile.Exists(true, _backupSoftDelete);
            Assert.AreEqual("Backup", _backupSoftDelete.ReadAllText());
        }

        [Test]
        public void RestoreWhenNoFile()
        {
            _backup.Delete();
            AssertFile.Exists(false, _file);
            _file.Restore(_backup);
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _backup);
        }

        [Test]
        public void RestoreWhenSoftDeleteFile()
        {
            _softDeleteFile.WriteAllText("Soft");
            _backup.Delete();
            AssertFile.Exists(false, _file);
            _file.Restore(_softDeleteFile);
            AssertFile.Exists(true, _file);
            Assert.AreEqual("Soft", _file.ReadAllText());
            AssertFile.Exists(false, _backup);
            AssertFile.Exists(false, _softDeleteFile);
        }

        [Test]
        public void RestoreWhenNoRestoreFile()
        {
            _backup.Delete();
            _file.WriteAllText("File");
            _file.Restore(_backup);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            Assert.AreEqual("File", _file.ReadAllText());
        }

        [Test]
        public void RestoreWhenHasRestoreFile()
        {
            _file.WriteAllText("File");
            _backup.WriteAllText("Restore");
            _file.Restore(_backup);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            Assert.AreEqual("Restore", _file.ReadAllText());
        }

        [Test]
        public void SoftDeleteWhenOnlySoftFile()
        {
            _softDeleteFile.VoidCreate();
            _file.SoftDelete();
            AssertFile.Exists(false, _file);
            AssertFile.Exists(true, _softDeleteFile);
        }

        [TestCase(@"C:\Temp", "Setting", "cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(@"C:\Temp", "Setting.xml", ".cfg", @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.cfg", null, @"C:\Temp\Setting.cfg")]
        [TestCase(null, @"C:\Temp\Setting.xml", "cfg", @"C:\Temp\Setting.cfg")]
        public void CreateFileInfo(string dir, string fn, string ext, string expected)
        {
            if (dir != null)
            {
                var directory = new DirectoryInfo(dir);
                var fileInfo = FileHelper.CreateFileInfo(directory, fn, ext);
                Assert.AreEqual(expected, fileInfo.FullName);
            }
            else
            {
                var fileInfo = FileHelper.CreateFileInfo(null, fn, ext);
                Assert.AreEqual(expected, fileInfo.FullName);
            }
        }

        [Test]
        public void Save()
        {
            var fileInfo = Directory.CreateFileInfoInDirectory("SaveTest.cfg");
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("1 2");
                writer.Flush();
                stream.Position = 0;
                FileHelper.Save(fileInfo, stream);
            }

            var text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("1 2", text);

            stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("3");
                writer.Flush();
                stream.Position = 0;
                FileHelper.Save(fileInfo, stream);
            }

            text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("3", text);
        }

        [Test]
        public async Task SaveAsync()
        {
            var fileInfo = Directory.CreateFileInfoInDirectory("SaveAsyncTest.cfg");
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("1 2");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            var text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("1 2", text);

            stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("3");
                writer.Flush();
                stream.Position = 0;
                await FileHelper.SaveAsync(fileInfo, stream).ConfigureAwait(false);
            }

            text = File.ReadAllText(fileInfo.FullName);
            Assert.AreEqual("3", text);
        }
    }
}