using API.Services.Demo;
using auto_upload_youtube.Services.Upload;
using BaseService.MemoryService;
using BaseService.Shared;

using System.Configuration;
using System.Collections.Specialized;
using auto_upload_youtube.Services.Download;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://*::6002", "http://0.0.0.0:6002");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ManageServices();


app.Run();

static void ManageServices()
{
    //var countingService = new JobQueueService<CountingJob, CountingJobInput, CountingJobOutput>("counting number", 10, 100);
    //ServiceManager.AddService(countingService, true);

    var uploadService = new JobQueueService<UploadJob, UploadJobInput, UploadJobOutput>("Upload Service", 1, 100);
    ServiceManager.AddService(uploadService, true);

    var downloadService = new JobQueueService<DownloadJob, DownloadJobInput, DownloadJobOutput>("Download Service", 5, 100);
    ServiceManager.AddService(downloadService, true);

    //var input = new UploadJobInput();
    //input.ProfilePath = @"D:\profile\630988a4d02964299ea7410f";
    //var yt = new YoutubeUpload();
    //var driver = yt.InitChromeDriver(input);
    //driver.Url = "https://studio.youtube.com";
    //driver.Navigate();
    //Thread.Sleep(15000);
}