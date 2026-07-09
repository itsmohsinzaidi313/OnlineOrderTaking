var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();
app.MapGet("/UploadMenu/{id}", (int id) =>
{
    return Results.Ok();

});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.Run();

