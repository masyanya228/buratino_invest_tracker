using Buratino.Models.DomainService.DomainStructure;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using Buratino.DI;
using Buratino.Models.DomainService;
using Buratino.Models.Repositories.Implementations;
using Buratino.Models.Map.MapStructure;
using Buratino.Models.Map.Implementations;
using Buratino.Models.Repositories.Implementations.Postgres;
using Buratino.Entities;
using Buratino.Repositories.Implementations.Postgres;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddKeyedSingleton(typeof(IRepository<>), "IEntity", typeof(PGRepository<>));
        builder.Services.AddKeyedSingleton(typeof(IRepository<>), "PersistentEntity", typeof(PGPersistentRepository<>));

        builder.Services.AddKeyedSingleton(typeof(IDomainService<>), "IEntity", typeof(DefaultDomainService<>));
        builder.Services.AddKeyedSingleton(typeof(IDomainService<>), "PersistentEntity", typeof(PersistentDomainService<>));
        
        //for PG
        builder.Services.AddSingleton(typeof(IPGSessionFactory), typeof(PGSessionFactory));
        
        //for LiteDB
        builder.Services.AddSingleton(typeof(IMap<>), typeof(LiteDBMap<>));

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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

        app.Lifetime.ApplicationStarted.Register(OnStarted);
        app.Run();
    }

    public static void OnStarted()
    {
        var acc = Container.Resolve<IRepository<InvestSource>>("IEntity").GetAll().ToArray();
        var acc2 = Container.Resolve<IRepository<InvestSource>>("PersistentEntity").GetAll().ToArray();

        var ds1 = Container.ResolveDomainService<InvestSource>("IEntity").GetAll().ToArray();
        var ds2 = Container.ResolveDomainService<InvestSource>("PersistentEntity").GetAll().ToArray();

        var accounts = Container.ResolveDomainService<Account>();
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


        Container.ResolveDomainService<RoleAccountLink>().CascadeSave(new RoleAccountLink()
        {
            Account = accounts.GetAll().First(),
            Role = new Role()
            {
                Name = "Administrator"
            }
        });

        var list = Container.ResolveDomainService<RoleAccountLink>().GetAll();
    }
}