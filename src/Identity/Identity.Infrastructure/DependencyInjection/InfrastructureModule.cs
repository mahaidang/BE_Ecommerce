using Autofac;
using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.DependencyInjection;

public class InfrastructureModule : Module
{
    private readonly IConfiguration _config;

    public InfrastructureModule(IConfiguration config)
    {
        _config = config;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // DbContext
        builder.Register(c =>
        {
            var optBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optBuilder.UseSqlServer(_config.GetConnectionString("Default"));
            return new IdentityDbContext(optBuilder.Options);
        })
        .AsSelf()
        .As<IIdentityDbContext>()
        .As<IUnitOfWork>()
        .InstancePerLifetimeScope();

        // Security services
        builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
        builder.RegisterType<PasswordHasher>().As<IPasswordHasher>().InstancePerLifetimeScope();
        builder.RegisterType<JwtTokenGenerator>().As<IJwtTokenGenerator>().InstancePerLifetimeScope();
        builder.RegisterType<CurrentUserService>().As<ICurrentUserService>().InstancePerLifetimeScope();

        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
    }
}
