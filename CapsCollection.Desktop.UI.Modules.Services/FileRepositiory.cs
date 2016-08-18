using CapsCollection.Business.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class FileRepository : IFileRepository
    {
        private readonly IFileSystem _fileSystem;

        public FileRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task BatchFileRename(List<ImageFileOperationDto> renamePackage)
        {
            if (renamePackage == null)
            {
                throw new AggregateException(nameof(renamePackage));
            }

            List<Task> ranamingTasks = new List<Task>();
            foreach (var rename in renamePackage)
            {
                var renameTask = Task.Run(() => RenameFile(rename.SourceFileFullName, rename.FileName));
                ranamingTasks.Add(renameTask);
            }
            await Task.WhenAll(ranamingTasks);
        }

        public void RenameFile(string oldFileFullPath, string newFileName)
        {
            if (String.IsNullOrEmpty(oldFileFullPath))
            {
                throw new ArgumentNullException(nameof(oldFileFullPath));
            }

            if (String.IsNullOrEmpty(newFileName))
            {
                throw new ArgumentNullException(nameof(newFileName));
            }

            if (!_fileSystem.FileExists(oldFileFullPath))
            {
                throw new FileNotFoundException();
            }

            var directoryName = _fileSystem.GetDirectoryName(oldFileFullPath);

            if (String.IsNullOrEmpty(directoryName) || !_fileSystem.DirectoryExists(directoryName))
            {
                throw new DirectoryNotFoundException();
            }

            var newFileFullPath = _fileSystem.PathCombine(directoryName, newFileName);


            if (_fileSystem.FileExists(newFileFullPath))
            {
                return;
            }

            _fileSystem.MoveFile(oldFileFullPath, newFileFullPath);
        }
    }
}
