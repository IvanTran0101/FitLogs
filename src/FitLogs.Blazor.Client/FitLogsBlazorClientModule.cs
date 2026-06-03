using System;
using System.Net.Http;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FitLogs.Blazor.Client.Navigation;
using OpenIddict.Abstractions;
using Volo.Abp.AspNetCore.Components.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming.Bundling;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXLiteTheme;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;
using Volo.Abp.FeatureManagement.Blazor.WebAssembly;
using Volo.Abp.Account.Pro.Admin.Blazor.WebAssembly;
using Volo.Abp.Account.Pro.Public.Blazor.WebAssembly;
using Volo.Abp.Identity.Pro.Blazor.Server.WebAssembly;
using Volo.Abp.Identity.Pro.Blazor;
using Volo.Abp.AuditLogging.Blazor.WebAssembly;
using Volo.Abp.Gdpr.Blazor.Extensions;
using Volo.Abp.Gdpr.Blazor.WebAssembly;
using Volo.Abp.LanguageManagement.Blazor.WebAssembly;
using Volo.Abp.OpenIddict.Pro.Blazor.WebAssembly;
using Volo.Abp.TextTemplateManagement.Blazor.WebAssembly;
using Volo.Saas.Host.Blazor;
using Volo.Saas.Host.Blazor.WebAssembly;


namespace FitLogs.Blazor.Client;

[DependsOn(
    typeof(AbpSettingManagementBlazorWebAssemblyModule),
    typeof(AbpFeatureManagementBlazorWebAssemblyModule),
    typeof(AbpAccountAdminBlazorWebAssemblyModule),
    typeof(AbpAccountPublicBlazorWebAssemblyModule),
    typeof(AbpIdentityProBlazorWebAssemblyModule),
    typeof(SaasHostBlazorWebAssemblyModule),
    typeof(AbpOpenIddictProBlazorWebAssemblyModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyModule),
    typeof(AbpGdprBlazorWebAssemblyModule),
    typeof(TextTemplateManagementBlazorWebAssemblyModule),
    typeof(LanguageManagementBlazorWebAssemblyModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXLiteThemeModule),
    typeof(AbpAutofacWebAssemblyModule),
    typeof(FitLogsHttpApiClientModule)
)]
public class FitLogsBlazorClientModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpAspNetCoreComponentsWebOptions>(options =>
        {
            options.IsBlazorWebApp = true;
        });
    }
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
        var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

        ConfigureAuthentication(builder);
        ConfigureImpersonation(context);
        ConfigureHttpClient(context, environment);
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureMenu(context);
        ConfigureCookieConsent(context);
    }
    
    private void ConfigureCookieConsent(ServiceConfigurationContext context)
    {
        context.Services.AddAbpCookieConsent(options =>
        {
            options.IsEnabled = true;
            options.CookiePolicyUrl = "/CookiePolicy";
            options.PrivacyPolicyUrl = "/PrivacyPolicy";
        });
    }


    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(FitLogsBlazorClientModule).Assembly;
            options.AdditionalAssemblies.Add(typeof(FitLogsBlazorClientModule).Assembly);
        });
    }

    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new FitLogsMenuContributor(context.Services.GetConfiguration()));
        });
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBlazorise(options =>
            {
                // TODO (IMPORTANT): To use Blazorise, you need a license key. Get your license key directly from Blazorise, follow  the instructions at https://abp.io/faq#how-to-get-blazorise-license-key
                //options.ProductToken = "Your Product Token";
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazorWebAppServices();
    }
    
    private void ConfigureImpersonation(ServiceConfigurationContext context)
    {
        context.Services.Configure<SaasHostBlazorOptions>(options =>
        {
            options.EnableTenantImpersonation = true;
        });
        context.Services.Configure<AbpIdentityProBlazorOptions>(options =>
        {
            options.EnableUserImpersonation = true;
        });
    }

    private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
    {
        context.Services.AddTransient(sp => new HttpClient
        {
            BaseAddress = new Uri(environment.BaseAddress)
        });
    }
}
