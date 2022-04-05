using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace blob_indexer.Pages;

public class dataTree
{
    public string filename {get;set;}
    public string dir {get;set;}
    public double size {get;set;}
}
public class IndexModel : PageModel
{
    private string connectionString;
    private string containerName;
    public List<BlobItem> blobs;
    private BlobContainerClient blobClient;
    public string bloburl;
    public List<dataTree> data;
    private int count;
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _config;
    public string currentDirectory;
    public List<string> RootDirs;
    [BindProperty]
    public string previousPath {get;set;}
    private double? nullableDouble {get;set;}

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _config = config;
        _logger = logger;
        connectionString = config["Storage:connectionString"];
        containerName = config["Storage:containerName"];
        blobClient = new BlobContainerClient(connectionString,containerName);
        blobs = new List<BlobItem>();
        bloburl = blobClient.Uri.AbsoluteUri;
        data = new List<dataTree>();
        currentDirectory = "";
        RootDirs = new List<string>();
        count = 0;
        previousPath = "";
    }

    public void OnGet()
    {
        blobs = blobClient.GetBlobs().OrderByDescending(x => x.Properties.CreatedOn).ToList<BlobItem>();
        foreach(BlobItem blob in blobs)
        {
            data.Add(new dataTree() { filename = blob.Name.Split("/").Last(), dir = String.Join("/",blob.Name.Split("/").SkipLast(1))});
        }
        RootDirs = data.Where(x => x.dir != "").Select(y => y.dir.Split("/")[0]).ToList<string>();
    }

    public void OnGetDir(string directory)
    {
        previousPath = directory.Split("/").Count() == 0 ? "" : String.Join("/",directory.Split("/").SkipLast(1));
        currentDirectory = directory;
        count = currentDirectory.Split("/").Count() == 0 ? 1 : currentDirectory.Split("/").Count();
        blobs = blobClient.GetBlobs(BlobTraits.None, BlobStates.None, currentDirectory).OrderByDescending(x => x.Properties.CreatedOn).ToList<BlobItem>();
        foreach(BlobItem blob in blobs)
        {
            nullableDouble = blob.Properties.ContentLength;
            data.Add(new dataTree() { filename = blob.Name.Split("/").Last(), dir = String.Join("/",blob.Name.Split("/").SkipLast(1)), size = nullableDouble.Value / 1000.0});
        }
        RootDirs = data.Where(x => x.dir.StartsWith(currentDirectory)).Select(y => y.dir != currentDirectory && y.dir.Contains(currentDirectory+"/") ? y.dir.Split("/")[count] : currentDirectory).ToList<string>();
        RootDirs.RemoveAll(x => x == currentDirectory);
        for(int i = 0; i < RootDirs.Count; i++)
        {
            RootDirs[i] = currentDirectory + "/" + RootDirs[i];
        }
    }
}
