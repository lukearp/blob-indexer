# What does this do?
This is a simple Web App that connects to an Azure Blob Storage container and indexes the contents. For files it links directly to the Blob in the Azure Storage account.  To make the files accessible, the Blob Container needs to be set to Public with Anonymous Access. 

# What does it require?
Settings the Storage App settings Variables in the appsettings.json file.

```json
{
  "storage": {
    "connectionString": "MY_STORAGE_CONNECTION_STRING",
    "containerName": "MY_CONTAINER_NAME"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```