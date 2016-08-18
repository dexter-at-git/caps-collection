using System;
using System.Collections.Generic;
using System.IO;
using CapsCollection.Common.Settings;
using CapsCollection.Data.Repositories.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CapsCollection.Data.Repositories
{
    public class ImageFileBlobRepository : IImageFileRepository
    {
        public void UploadImageFile(byte[] imageBytes, string parentContainer, string container, string fileName)
        {
            if (imageBytes == null)
            {
                return;
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CapsCollectionAzureSettings.AzureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);

            using (Stream stream = new MemoryStream(imageBytes))
            {
                blockBlob.UploadFromStream(stream);
            }

            blockBlob.Properties.CacheControl = "no-cache";
            blockBlob.Properties.ContentType = "image/png";
            blockBlob.SetProperties();
            blockBlob.FetchAttributes();
        }


        public void DeleteImageFile(string parentContainer, string container, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CapsCollectionAzureSettings.AzureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);

            using (Stream stream = new MemoryStream(new byte[0]))
            {
                blockBlob.UploadFromStream(stream);
            }

            blockBlob.Properties.CacheControl = "no-cache";
            blockBlob.Properties.ContentType = "image/png";
            blockBlob.SetProperties();
            blockBlob.FetchAttributes();
            blockBlob.DeleteIfExists();
        }


        public void DeleteAllFiles(string imageContainer)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CapsCollectionAzureSettings.AzureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(imageContainer);

            IEnumerable<IListBlobItem> blobs = container.ListBlobs();

            foreach (IListBlobItem blob in blobs)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob.Uri.ToString());
                blockBlob.DeleteIfExists();
            }
        }


        public void UpdateImageFile(string container, string newFileName, string oldFileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CapsCollectionAzureSettings.AzureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
            CloudBlockBlob newBlockBlob = blobContainer.GetBlockBlobReference(newFileName);
            CloudBlockBlob oldBlockBlob = blobContainer.GetBlockBlobReference(oldFileName);

            if (oldBlockBlob == null)
            {
                return;
            }

            newBlockBlob.StartCopy(oldBlockBlob.Uri);
            while (true)
            {
                newBlockBlob.FetchAttributes();
                if (newBlockBlob.CopyState.Status != CopyStatus.Pending)
                {
                    break;
                }
            }
            oldBlockBlob.DeleteIfExists();
        }


        public byte[] DownloadImageFile(string container, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CapsCollectionAzureSettings.AzureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);

            MemoryStream memoryStream = new MemoryStream();
            blockBlob.DownloadToStream(memoryStream);

            byte[] imageBytes = ReadAllBytes(memoryStream);

            return imageBytes;
        }


        private byte[] ReadAllBytes(Stream source)
        {
            long originalPosition = source.Position;
            source.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = source.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = source.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                source.Position = originalPosition;
            }
        }
    }
}
