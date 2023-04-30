using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenPolicy",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("OpenPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//getall
app.MapGet("/items", (ToDoDbContext context) =>
{
    return context.Items.ToList();
});

//insert
app.MapPost("/items/", async (ToDoDbContext context, Item item) =>
{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

//update
app.MapPut("/items/{id}", async (ToDoDbContext context, Boolean isComplete, int id) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem is null) return Results.NotFound();

    existItem.IsComplete = isComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

//delete
app.MapDelete("/items/{id}", async (ToDoDbContext context, int id) =>
{
    var existItem = await context.Items.FindAsync(id);
    if (existItem is null) return Results.NotFound();

    context.Items.Remove(existItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

