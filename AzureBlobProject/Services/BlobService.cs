using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services;

public class BlobService: IBlobService
{
    private readonly BlobServiceClient _blobClient;

    public BlobService(BlobServiceClient blobClient)
    {
        _blobClient = blobClient;
    }

    public async Task<string> GetBlob(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        return blobClient.Uri.AbsoluteUri;

    }

    public async Task<List<string>> GetAllBlobs(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobs = blobContainerClient.GetBlobsAsync();

        var blobString = new List<string>();
        await foreach (var item in blobs)
        {
            blobString.Add(item.Name);
        }

        return blobString;
    }

    public async Task<List<Blob>> GetAllBlobsWIthUri(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobs = blobContainerClient.GetBlobsAsync();
        var blobList = new List<Blob>();
        await foreach (var item in blobs)
        {
            var blobClient = blobContainerClient.GetBlobClient(item.Name);

            Blob blobInvidual = new()
            {
                Uri = blobClient.Uri.AbsoluteUri
            };
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                blobInvidual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
            }
            BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
            if (blobProperties.Metadata.ContainsKey("title"))
            {
                blobInvidual.Title = blobProperties.Metadata["title"];
            }
            if (blobProperties.Metadata.ContainsKey("comment"))
            {
                blobInvidual.Title = blobProperties.Metadata["comment"];
            }
            blobList.Add(blobInvidual);
            
        }

        return blobList;

    }

    public async Task<bool> UploadBlob(string name, IFormFile file, string containerName,Blob blob)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        var httpHeaders = new BlobHttpHeaders()
        {
            ContentType = file.ContentType
        };
        IDictionary<string, string> metadata = new Dictionary<string, string>();
        metadata.Add("title",blob.Title);
        metadata["comment"] = blob.Comment;
        
        var result = await blobClient.UploadAsync(file.OpenReadStream(),httpHeaders,metadata);
       
        //metadata.Remove("title");

       //await blobClient.SetMetadataAsync(metadata);
        
        if (result != null)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteBlob(string name, string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

        var blobClient = blobContainerClient.GetBlobClient(name);

        return await blobClient.DeleteIfExistsAsync();
    }
}