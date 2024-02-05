using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Marten;

public static class IdentityBuilderExtensions
{
    public static IdentityBuilder AddMartenStore(this IdentityBuilder identityBuilder)
    {
        var userStoreType = typeof(MartenUserStore<,,>).MakeGenericType(
            identityBuilder.UserType,
            identityBuilder.RoleType,
            identityBuilder.UserType.GenericTypeArguments.Length == 1
            ? identityBuilder.UserType.GenericTypeArguments[0]
            : identityBuilder.UserType.BaseType?.GenericTypeArguments[0]
            ?? throw new ArgumentException("bad user type; cudn't find key")
            );

        // var roleStoreType = typeof(Role)

        identityBuilder.Services.AddScoped(typeof(IUserStore<>).MakeGenericType(identityBuilder.UserType), userStoreType);


        if (identityBuilder.RoleType is not null)
        {
            identityBuilder.Services.AddScoped(typeof(IRoleStore<>).MakeGenericType(identityBuilder.RoleType),
                typeof(MartenRoleStore<>).MakeGenericType(identityBuilder.RoleType));
        }
        return identityBuilder;

    }
}
