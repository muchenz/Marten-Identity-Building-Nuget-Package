using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Marten.Tests;

public class AddRoleToUserTest : IntegrationTest
{
    public AddRoleToUserTest(IntegrationFicture integrationFicture) : base(integrationFicture)
    {
    }
    [Fact]
    public async Task Can_Add_Role_To_User()
    {

        var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = Services.GetRequiredService<UserManager<IdentityUser>>();

        var roleName = Guid.NewGuid().ToString();

        var createRoleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

        Assert.True(createRoleResult.Succeeded);

        var userIdentity = new IdentityUser()
        {
            Email = $"{Guid.NewGuid()}@test.com",
            UserName = Guid.NewGuid().ToString(),

        };
        var createUserResult = await userManager.CreateAsync(userIdentity);
        Assert.True(createUserResult.Succeeded);

        var addToRoleResult = await userManager.AddToRoleAsync(userIdentity, roleName);

        Assert.True(addToRoleResult.Succeeded);

       var getRolesResult = await userManager.GetRolesAsync(userIdentity);

        var singleRole = Assert.Single(getRolesResult);

        Assert.Equal(roleName, singleRole);

    }
}
