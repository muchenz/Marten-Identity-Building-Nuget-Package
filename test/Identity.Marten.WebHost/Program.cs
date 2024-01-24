using Marten;
using Microsoft.AspNetCore.Identity;
using Identity.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten("host=127.0.0.1;port=5432;database=idenity_marten;user id=postgres;password=password;");

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddMartenStore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", async (string name, UserManager<IdentityUser> userManager) =>
{
    var result = await userManager.FindByNameAsync(name);
    return result;
});


app.MapGet("/add-user", async (UserManager<IdentityUser>  userManager) =>
{

    var result = await userManager.CreateAsync(new IdentityUser()
    {
        Email = "test@test.com",
        UserName = "ala"
    });

    return Results.Ok(result);
});




app.Run();


public partial class Program { }

