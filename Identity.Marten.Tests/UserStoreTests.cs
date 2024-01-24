using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Marten.Tests;
public class UserStoreTests : IntegrationTest
{
    public UserStoreTests(IntegrationFicture integrationFicture) : base(integrationFicture)
    {
    }

    

    [Fact]
    public async void Crud_Operations_insert()
    {

        var userId = Guid.NewGuid().ToString();

        var userStore = Services.GetRequiredService<IUserStore<IdentityUser>>();

        var identity = new IdentityUser()
        {
            Id = userId,
            UserName = "Ala"

        };

        var identityResult = await userStore.CreateAsync(identity, CancellationToken.None);

        var userAle = await userStore.FindByIdAsync(userId, CancellationToken.None);

        Assert.NotNull(userAle);
        Assert.Equal("Ala", userAle.UserName);

       
    }





    [Fact]
    public async void Crud_Operations_update()
    {

        var userId = Guid.NewGuid().ToString();

        await Instert(userId, "Ala");


        var userStore = Services.GetRequiredService<IUserStore<IdentityUser>>();

        var identity = new IdentityUser()
        {
            Id = userId,
            UserName = "Ela"

        };
       
        var updareResult = await userStore.UpdateAsync(identity, CancellationToken.None);
        Assert.True(updareResult.Succeeded);

        var userEla = await userStore.FindByIdAsync(userId, CancellationToken.None);

        Assert.NotNull(userEla);
        Assert.Equal("Ela", userEla.UserName);
    }
    [Fact]
    public async void Crud_Operations_delete()
    {
        //await Task.Delay(1000);


        var userId = Guid.NewGuid().ToString();

        await Instert(userId, "Ala");

        var userStore = Services.GetRequiredService<IUserStore<IdentityUser>>();

        var identity = new IdentityUser()
        {
            Id = userId,

        };
        var deleteUserEla = await userStore.DeleteAsync(identity, CancellationToken.None);
        Assert.True(deleteUserEla.Succeeded);

        var userDeleted = await userStore.FindByIdAsync(userId, CancellationToken.None);

        Assert.Null(userDeleted);
    }


    async Task Instert(string guid, string name)
    {

        using (var scope = Services.CreateScope())
        {
            var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<IdentityUser>>();

            var identity = new IdentityUser()
            {
                Id = guid,
                UserName = name

            };

            var identityResult = await userStore.CreateAsync(identity, CancellationToken.None);
        }
    }
}