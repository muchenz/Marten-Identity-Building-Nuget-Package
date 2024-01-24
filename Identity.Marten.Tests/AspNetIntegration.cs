namespace Identity.Marten.Tests;

public class AspNetIntegration : IntegrationTest
{
    public AspNetIntegration(IntegrationFicture integrationFicture) : base(integrationFicture)
    {
    }

    [Fact]
    public async Task Create_And_Read_User()
    {
        Console.WriteLine("ala makota");
        
        //await Task.Delay(1000);


        var resultsAdd = await Client.GetAsync("/add-user");


        Assert.Equal(System.Net.HttpStatusCode.OK, resultsAdd.StatusCode);

        var resultsGetName = await Client.GetAsync("/?name=ala");
        Assert.Equal(System.Net.HttpStatusCode.OK, resultsGetName.StatusCode);

    }
}