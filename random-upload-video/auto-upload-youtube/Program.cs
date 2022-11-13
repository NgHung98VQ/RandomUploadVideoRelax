using API.Services.Demo;
using API.Constant;
using BaseService.MemoryService;
using BaseService.Shared;

using System.Configuration;
using System.Collections.Specialized;
using auto_upload_youtube.Services.RandomUpload;
//using UploadJobInput = auto_upload_youtube.Services.Upload.UploadJobInput;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://*::6003", "http://0.0.0.0:6003");
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

    var randomUploadService = new JobQueueService<RandomUploadJob, RandomUploadJobInput, RandomUploadJobOutput>("Random Upload Service", 10, 100);
    ServiceManager.AddService(randomUploadService, true);

}