using Chat.Authorization.Extensions;
using Chat.Infrastructure;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScalarDocumentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


app.Run();
