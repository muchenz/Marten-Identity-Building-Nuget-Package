using Marten;
using Microsoft.AspNetCore.Identity;
using Identity.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten("host=127.0.0.1;port=5432;database=idenity_marten;user id=postgres;password=postgres;");

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


app.MapGet("/get-user", async (string? name, UserManager<IdentityUser> userManager) =>
{
    if (name is null) name = "ala";

    var result = await userManager.FindByNameAsync(name);

    if (result is null) return Results.BadRequest($"No user was find named {name}");

    return Results.Ok(result);
});


app.MapGet("/add-user", async (UserManager<IdentityUser> userManager) =>
{

    var result = await userManager.CreateAsync(new IdentityUser()
    {
        Email = "test@test.com",
        UserName = "ala"
    });

    return Results.Ok(result);
});


app.MapGet("/add-role", async (string? name, RoleManager<IdentityRole> roleManager) =>
{
    if (string.IsNullOrEmpty(name)) name = "Admin";

    var result = await roleManager.CreateAsync(new IdentityRole(name));

    return Results.Ok(result);
});

app.MapGet("/get-role", async (string? name, RoleManager<IdentityRole> roleManager) =>
{
    if (string.IsNullOrEmpty(name)) name="Admin"; // return Results.BadRequest("Need name of role");

    var result = await roleManager.FindByNameAsync(name);

    if (result is null) return Results.BadRequest($"No role named {name} was find");

    return Results.Ok(result);
});



//app.MapGet("/set-admin", async (RoleManager<IdentityRole> roleManager,
//    UserManager<IdentityUser> userManager) =>
//{


//    var result = await roleManager.CreateAsync(new IdentityRole("Admin"):

//    return Results.Ok(result);
//});

app.Run();


public partial class Program { }

