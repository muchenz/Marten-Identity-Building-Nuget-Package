namespace Identity.Marten.Tests;

public class AspNetIntegration : IntegrationTest
{
    public AspNetIntegration(IntegrationFicture integrationFicture) : base(integrationFicture)
    {
    }

    [Fact]
    public async Task Create_And_Read_User()
    {
        Console.WriteLine("Create_And_Read_User");
        
        //await Task.Delay(1000);


        var resultsAdd = await Client.GetAsync("/add-user");


        Assert.Equal(System.Net.HttpStatusCode.OK, resultsAdd.StatusCode);

        var resultsGetName = await Client.GetAsync("/get-user?name=ala");
        Assert.Equal(System.Net.HttpStatusCode.OK, resultsGetName.StatusCode);

    }


    [Fact]
    public async Task Create_And_Read_Role()
    {
        Console.WriteLine("Create_And_Read_Role");

        //await Task.Delay(1000);


        var resultsAdd = await Client.GetAsync("/add-role?name=Admin");


        Assert.Equal(System.Net.HttpStatusCode.OK, resultsAdd.StatusCode);

        var resultsGetName = await Client.GetAsync("/get-role?name=Admin");
        Assert.Equal(System.Net.HttpStatusCode.OK, resultsGetName.StatusCode);

    }
}