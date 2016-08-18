using System;
using System.Collections.Generic;
using System.Globalization;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class UploadPackageBuilder
    {
        private List<ImageFileOperationDto> uploadFilesList;
        private List<ImageFileOperationDto> localFilesList;

        private UploadPackageBuilder()
        {
            uploadFilesList = new List<ImageFileOperationDto>();
            localFilesList = new List<ImageFileOperationDto>();
        }

        public static UploadPackageBuilder CreateBuilder()
        {
            return new UploadPackageBuilder();
        }
        
        public UploadPackageBuilder AddImagesToPackage(BeerImage beerImage)
        {
            if (beerImage == null)
            {
                return this;
            }

            if (beerImage.SourceFileFullPath != null)
            {
                localFilesList.Add(new ImageFileOperationDto()
                {
                    FileNameTemplate = beerImage.FileNameTemplate,
                    SourceFileFullName = beerImage.SourceFileFullPath
                });
            }
            
            uploadFilesList.Add(new ImageFileOperationDto()
            {
                FileNameTemplate = beerImage.FileNameTemplate,
                ImageBytes = beerImage.FullSizeBytes,
                Container = beerImage.Container,
                FileOperation = FileOperation.Save
            });

            uploadFilesList.Add(new ImageFileOperationDto()
            {
                FileNameTemplate = beerImage.PreviewFileNameTemplate,
                ImageBytes = beerImage.PreviewBytes,
                Container = beerImage.Container,
                FileOperation = FileOperation.Save
            });

            uploadFilesList.Add(new ImageFileOperationDto()
            {
                FileNameTemplate = beerImage.ThumbnailFileNameTemplate,
                ImageBytes = beerImage.ThumbnailBytes,
                Container = beerImage.Container,
                FileOperation = FileOperation.Save
            });

            return this;
        }

        public UploadPackageBuilder CreateFileNames(int beerId)
        {
            var beerNumber = beerId.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');

            foreach (var item in uploadFilesList)
            {
                item.FileName = String.Format(item.FileNameTemplate, beerNumber);
            }


            foreach (var item in localFilesList)
            {
                item.FileName = String.Format(item.FileNameTemplate, beerNumber);
            }

            return this;
        }

        public List<ImageFileOperationDto> GetUploadPackage()
        {
            return uploadFilesList;
        }

        public List<ImageFileOperationDto> GetRenamePackage()
        {
            return localFilesList;
        }
    }
}