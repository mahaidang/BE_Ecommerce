using Autofac;
using FluentValidation;
using Identity.Application.Common.Behaviors;
using MediatR;
using System.Reflection;
using Module = Autofac.Module;

namespace Identity.Application.DependencyInjection;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var asm = Assembly.GetExecutingAssembly();

        // MediatR handlers
        builder.RegisterAssemblyTypes(asm)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // FluentValidation validators
        builder.RegisterAssemblyTypes(asm)
            .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Pipeline behaviors
        builder.RegisterGeneric(typeof(ValidationBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();
    }
}
