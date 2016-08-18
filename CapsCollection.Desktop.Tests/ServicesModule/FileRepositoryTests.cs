using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CapsCollection.Desktop.Tests.ServicesModule
{
    [TestClass]
    public class FileRepositoryTests
    {
        private Mock<IFileSystem> _fileSystemMock = new Mock<IFileSystem>();
        private IFileRepository _fileRepository;
        private string _oldFileFullPath;
        private string _oldFileFakePath;
        private string _oldInvalidFolderPath;
        private string _newFileName;
        private string _newFileNameDuplicate;
        private string _directory;
        private string _newFileFullPath;
        private string _newFileFullPathDuplicate;

        [TestInitialize]
        public void TestInitialize()
        {
            _fileRepository = new FileRepository(_fileSystemMock.Object);
            _oldFileFullPath = @"c:\folder\old.filename";
            _oldFileFakePath = @"c:\fake\old.filename";
            _oldInvalidFolderPath = @"c:\invalidfolder\old.filename";
            _newFileName = "new.filename";
            _newFileNameDuplicate = "new.filename.duplicate";
            _directory = @"c:\folder";
            _newFileFullPath = @"c:\folder\new.filename";
            _newFileFullPathDuplicate = @"c:\folder\new.filename.duplicate";
        }
        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileRepository_RenameFile_FileInfoNull()
        {
            _fileRepository.RenameFile(null, _newFileName);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileRepository_RenameFile_NewFileNameNull()
        {
            _fileRepository.RenameFile(_oldFileFullPath, null);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileRepository_RenameFile_OldFileNotExists()
        {
            _fileSystemMock.Setup(x => x.FileExists(_oldFileFakePath)).Returns(false);

            _fileRepository.RenameFile(_oldFileFakePath, _newFileName);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void FileRepository_RenameFile_InvalidFolder()
        {
            _fileSystemMock.Setup(x => x.FileExists(_oldInvalidFolderPath)).Returns(true);
            _fileSystemMock.Setup(x => x.DirectoryExists(_oldInvalidFolderPath)).Returns(false);

            _fileRepository.RenameFile(_oldInvalidFolderPath, _newFileName);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void FileRepository_RenameFile_GetDirectoryReturnsEmptyString()
        {
            _fileSystemMock.Setup(x => x.FileExists(_oldFileFullPath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetDirectoryName(It.IsAny<string>())).Returns(String.Empty);

            _fileRepository.RenameFile(_oldFileFullPath, _newFileName);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public void FileRepository_RenameFile_NewFileAlreadyExists()
        {
            _fileSystemMock.Setup(x => x.FileExists(_oldFileFullPath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetDirectoryName(_oldFileFullPath)).Returns(_directory);
            _fileSystemMock.Setup(x => x.FileExists(_newFileNameDuplicate)).Returns(true);
            _fileSystemMock.Setup(x => x.DirectoryExists(_directory)).Returns(true);
            _fileSystemMock.Setup(x => x.PathCombine(_directory, _newFileNameDuplicate)).Returns(_newFileFullPathDuplicate);
            _fileSystemMock.Setup(x => x.FileExists(_newFileFullPathDuplicate)).Returns(true);

            _fileRepository.RenameFile(_oldFileFullPath, _newFileNameDuplicate);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public void FileRepository_RenameFile_RenamingCorrectly()
        {
            _fileSystemMock.Setup(x => x.FileExists(_oldFileFullPath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetDirectoryName(_oldFileFullPath)).Returns(_directory);
            _fileSystemMock.Setup(x => x.FileExists(_newFileName)).Returns(true);
            _fileSystemMock.Setup(x => x.DirectoryExists(_directory)).Returns(true);
            _fileSystemMock.Setup(x => x.PathCombine(_directory, _newFileName)).Returns(_newFileFullPath);
            _fileSystemMock.Setup(x => x.FileExists(_newFileFullPath)).Returns(false);

            _fileRepository.RenameFile(_oldFileFullPath, _newFileName);

            _fileSystemMock.Verify(x => x.MoveFile(_oldFileFullPath, _newFileFullPath), Times.Once);
        }
        

        [TestMethod]
        public void FileRepository_BatchFileRename_FileInfoNull()
        {
            var task = _fileRepository.BatchFileRename(null);

            _fileSystemMock.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.IsTrue(task.IsFaulted);
            Assert.IsTrue(task.Exception.InnerExceptions.Any());
        }


        [TestMethod]
        public void FileRepository_BatchFileRename_RenameIsCalledNTimes()
        {
            var imageList = new List<ImageFileOperationDto>()
            {
                new ImageFileOperationDto() {SourceFileFullName = _oldFileFullPath, FileName = _newFileName},
                new ImageFileOperationDto() {SourceFileFullName = _oldFileFullPath, FileName = _newFileName},
                new ImageFileOperationDto() {SourceFileFullName = _oldFileFullPath, FileName = _newFileName},
            };

            _fileSystemMock.Setup(x => x.FileExists(_oldFileFullPath)).Returns(true);
            _fileSystemMock.Setup(x => x.GetDirectoryName(_oldFileFullPath)).Returns(_directory);
            _fileSystemMock.Setup(x => x.FileExists(_newFileName)).Returns(true);
            _fileSystemMock.Setup(x => x.DirectoryExists(_directory)).Returns(true);
            _fileSystemMock.Setup(x => x.PathCombine(_directory, _newFileName)).Returns(_newFileFullPath);
            _fileSystemMock.Setup(x => x.FileExists(_newFileFullPath)).Returns(false);

            _fileRepository.BatchFileRename(imageList);

            _fileSystemMock.Verify(x => x.MoveFile(_oldFileFullPath, _newFileFullPath), Times.Exactly(imageList.Count));
        }
    }
}
