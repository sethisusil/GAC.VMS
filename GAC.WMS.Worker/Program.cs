using GAC.WMS.Worker.BackgroundWorker;
using GAC.WMS.Worker.HttpHelpers;
using GAC.WMS.Worker.Models;
using GAC.WMS.Worker.XmlHelpers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<XmlPollingHostedService>();
builder.Services.AddScoped<IXmlFileProcessor, XmlFileProcessor>();
var apiConfig = builder.Configuration.GetSection("GACWMSApi").Get<GacWmsApiConfig>();
builder.Services.Configure<GacWmsApiConfig>(builder.Configuration.GetSection("GACWMSApi"));
builder.Services.Configure<XmlFileConfig>(builder.Configuration.GetSection("XmlFileConfig"));
builder.Services.AddHttpClient<IGACWMSApiClient, GACWMSApiClient>(client =>
{
    client.BaseAddress = new Uri(apiConfig!.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

var host = builder.Build();
host.Run();
