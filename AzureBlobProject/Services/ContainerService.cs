using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services;

public class ContainerService:IContainerService
{
    private readonly BlobServiceClient _blobClient;

    public ContainerService(BlobServiceClient blobClient)
    {
        _blobClient = blobClient;
    }
    public async Task<List<string>> GetALlContainerAndBlobs()
    {
        List<string> containerAndBlobNames = new();
        containerAndBlobNames.Add("Account Name :" + _blobClient.AccountName);
        containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");

        await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
        {
            containerAndBlobNames.Add("--"+blobContainerItem.Name);
            BlobContainerClient _blobContainer = _blobClient.GetBlobContainerClient(blobContainerItem.Name);
            
           await  foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
           {
               //get MetaData
               var blobClient = _blobContainer.GetBlobClient(blobItem.Name);
               BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
               var blobToAdd = blobItem.Name;
               if (blobProperties.Metadata.ContainsKey("title"))
               {
                   blobToAdd += "(" + blobProperties.Metadata["title"] + ")";
               }
               
               
                containerAndBlobNames.Add("--------"+blobToAdd);
            }
           containerAndBlobNames.Add("--------------------------------------------------------------------------------------------------------------");
        }

        return containerAndBlobNames;

    }

    public async Task<List<string>> GetAllContainer()
    {
        List<string> containerName = new();
        await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
        {
            containerName.Add(blobContainerItem.Name);
        }

        return containerName;
    }

    public async Task CreateContainer(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
    }

    public async Task DeleteContainer(string containerName)
    {
        BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        await blobContainerClient.DeleteIfExistsAsync();
    }
}