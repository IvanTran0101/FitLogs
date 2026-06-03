using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FitLogs.Localization;
using FitLogs.Permissions;
using FitLogs.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Localization.Resources.AbpUi;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace FitLogs.Blazor.Client.Navigation;

public class FitLogsMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public FitLogsMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }

        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<FitLogsResource>();
        
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        context.Menu.AddItem(new ApplicationMenuItem(
            FitLogsMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                FitLogsMenus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(FitLogsPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                FitLogsMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(FitLogsPermissions.Dashboard.Tenant)
        );

        //Saas
        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);
        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 2);

        //Administration->OpenId
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 4);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 6);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 7);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 8);
    }
    
    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        if (OperatingSystem.IsBrowser())
        {
            //Blazor wasm menu items

            var authServerUrl = _configuration["AuthServer:Authority"] ?? "";
            var accountResource = context.GetLocalizer<AccountResource>();

            context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 900,  target: "_blank").RequireAuthenticated());
            context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", icon: "fa fa-user-shield", order: 901,  target: "_blank").RequireAuthenticated());
            context.Menu.AddItem(new ApplicationMenuItem("Account.Sessions", accountResource["Sessions"], url: $"{authServerUrl.EnsureEndsWith('/')}Account/Sessions", icon: "fa fa-clock", order: 902, target: "_blank").RequireAuthenticated());
            
        }
        else
        {
            //Blazor server menu items

        }
        await Task.CompletedTask;
    }
}
