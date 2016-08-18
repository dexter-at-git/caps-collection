using System.Collections.Generic;
using System.Threading.Tasks;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Desktop.UI.Modules.Services.Interfaces
{
    public interface IFileRepository
    {
        Task BatchFileRename(List<ImageFileOperationDto> renamePackage);
        void RenameFile(string oldFileFullPath, string newFileName);
    }
}
