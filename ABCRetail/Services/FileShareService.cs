
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetail.Services
{
    public class FileShareService
    {
        private readonly ShareClient _shareClient;

        public FileShareService( string connectionString, string shareName = "Contracts") 
        {
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        //Lists the files
        public async Task<List<string>> ListFilesAsync()
        {
            var rootDir = _shareClient.GetRootDirectoryClient();
            var files = new List<string>();

            await foreach (ShareFileItem item in rootDir.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                    files.Add(item.Name);
            }

            return files;
        }
        //Download
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var rootDir = _shareClient.GetRootDirectoryClient();
            var fileClient = rootDir.GetFileClient(fileName);

            var download = await fileClient.DownloadAsync();
            MemoryStream memoryStream = new MemoryStream();
            await download.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        //File upload
        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            var rootDir = _shareClient.GetRootDirectoryClient();
            var fileClient = rootDir.GetFileClient(fileName);

            // Create the file on Azure Files and upload
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
        }
    }
}
