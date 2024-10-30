using Buratino.Models.DomainService.DomainStructure;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using Buratino.DI;
using Buratino.Models.DomainService;
using Buratino.Entities;
using Buratino.Repositories.Implementations.Postgres;
using Buratino.API;
using Buratino.Maps.Implementations;
using Buratino.Maps.MapStructure;
using Buratino.Repositories;
using Buratino.Repositories.Implementations;
using Buratino.Repositories.RepositoryStructure;
using Buratino.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddSingleton(typeof(IRepository<>), typeof(PGRepository<>));

        builder.Services.AddSingleton(typeof(IDomainService<>), typeof(DefaultDomainService<>));
        
        //for PG
        builder.Services.AddSingleton(typeof(IPGSessionFactory), typeof(PGSessionFactory));
        
        //for LiteDB
        builder.Services.AddSingleton(typeof(IMap<>), typeof(LiteDBMap<>));

        builder.Services.AddSingleton(typeof(InvestCalcService));
        builder.Services.AddSingleton(typeof(InvestIncomeService));
        builder.Services.AddSingleton(typeof(TInvestService));

        builder.Services.AddSingleton(typeof(IDomainService<InvestSource>), typeof(InvestSourceService));
        builder.Services.AddSingleton(typeof(IDomainService<InvestCharge>), typeof(InvestChargeService));
        builder.Services.AddSingleton(typeof(IDomainService<InvestPoint>), typeof(InvestPointService));
        
        builder.Services.AddSingleton(typeof(IRepository<InvestCharge>), typeof(InvestChargeRepository));
        builder.Services.AddSingleton(typeof(IRepository<InvestSource>), typeof(InvestSourceRepository));


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = PathString.FromUriComponent("/Security/LoginPage");
                options.ReturnUrlParameter = "url";
            });
        builder.Services.AddControllers(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        });

        var app = builder.Build();

        Container.Configure(app.Services);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(x =>
        {
            x.MapDefaultControllerRoute();
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                           Path.Combine(builder.Environment.ContentRootPath, "Images")),
            RequestPath = "/Images"
        });

        new MigrateData();

        app.Lifetime.ApplicationStarted.Register(OnStarted);
        app.Run();
    }

    public static void OnStarted()
    {
        new TGAPI().Start("7230300926:AAHenYludkc2ga1kuqhPZHGClQRlqlLXxjU");

        var ds2 = Container.GetDomainService<InvestSource>().GetAll().ToArray();

        var accounts = Container.GetDomainService<Account>();
        if (!accounts.GetAll().Any(x => x.Email == "admin"))
        {
            Account entity = new Account()
            {
                Email = "admin",
                Name = "Admin Admin Admin",
            };
            entity.SetPass("admin");

            accounts.Save(entity);
        }

        //Переопределение сравнения сущностей в управляемом коде
        var exist = accounts.GetAll().First();
        var newacc = new Account()
        {
            Id = exist.Id
        };
        if (exist == newacc)
        {

        }


        Container.GetDomainService<RoleAccountLink>().CascadeSave(new RoleAccountLink()
        {
            Account = accounts.GetAll().First(),
            Role = new Role()
            {
                Name = "Administrator"
            }
        });

        var list = Container.GetDomainService<RoleAccountLink>().GetAll();
    }
}