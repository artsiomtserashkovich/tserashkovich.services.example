using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using OperatingSystem = Microsoft.Azure.Management.AppService.Fluent.OperatingSystem;

public class Deploy : NukeBuild
{
    public static int Main() => Execute<Deploy>(x => x.DeployWebApi);

    AbsolutePath WebAppZipPath => RootDirectory / "buildOutput" / $"{Solution.Name}.zip";

    [Solution] Solution Solution;

    [Parameter("Tenant Id where app should be deployed.")]
    string TenantId => "82c51a82-548d-43ca-bcf9-bf4b7eb1d012";
    
    [Parameter]
    string PrincipalId;
    
    [Parameter]
    string PrincipalSecret;

    string ResourceGroupName => "exampleresourcegroup";

    string AppServicePlan => "tserexampleappplan";

    string WebAppName => "tserexampleapp";

    IAzure AzureClient;

    Target Initialize => _ => _
        .Executes(async () =>
        {
            var credentials = SdkContext
                .AzureCredentialsFactory
                .FromServicePrincipal(PrincipalId, PrincipalSecret, TenantId, AzureEnvironment.AzureGlobalCloud);
            
            AzureClient = await Azure.Authenticate(credentials)
                .WithDefaultSubscriptionAsync();
        });
    
    Target CreateWebApiService => _ => _
        .DependsOn(Initialize)
        .Executes(async () =>
        {
            await AzureClient
                .ResourceGroups
                .Define(ResourceGroupName)
                .WithRegion(Region.EuropeNorth)
                .CreateAsync();
        });
    
    Target DeployWebApi => _ => _
        .DependsOn(CreateWebApiService, Initialize)
        .Executes(async () =>
        {
            var appServicePlan = await AzureClient.AppServices.AppServicePlans
                .Define(AppServicePlan)
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(ResourceGroupName)
                .WithPricingTier(PricingTier.FreeF1)
                .WithOperatingSystem(OperatingSystem.Windows)
                .CreateAsync();

            var webApp = await AzureClient.AppServices.WebApps
                .Define(WebAppName)
                .WithExistingWindowsPlan(appServicePlan)
                .WithExistingResourceGroup(ResourceGroupName)
                .WithRuntimeStack(WebAppRuntimeStack.NET)
                .CreateAsync();
                
            await webApp.StopAsync();
            await webApp.DeployZip(WebAppZipPath);
            await webApp.StartAsync();
        });
}

public static class DeploymentExtensions
{
    public static async Task DeployZip(this IWebApp webApp, string path)
    {
        var profile = await webApp.GetPublishingProfileAsync();

        var username = profile.FtpUsername.Split('\\')[1];
        var password = profile.FtpPassword;
        var base64Auth = Convert.ToBase64String(Encoding.Default.GetBytes($"{username}:{password}"));
        
        var file = await File.ReadAllBytesAsync(path);

        var scmUrl = webApp.EnabledHostNames.SingleOrDefault(x => x.Contains("scm", StringComparison.InvariantCulture));
        var deployUrl = new Uri(new Uri($"https://{scmUrl}"), "api/zipdeploy");
        
        await using var stream = new MemoryStream(file);
        using var client = new HttpClient();
        
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64Auth);
        using var httpContent = new StreamContent(stream);
        
        var response = await client.PostAsync(deployUrl, httpContent);
        
        ControlFlow.Assert(response.IsSuccessStatusCode, "response.IsSuccessStatusCode");
    }
}