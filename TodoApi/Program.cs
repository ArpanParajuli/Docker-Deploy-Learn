using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Model;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// use to see client ip came to nginx and nginx forward those to this
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.MapGet("/" , async(HttpContext context) =>
{
    var ip = context.Connection.RemoteIpAddress?.ToString();
    var port = context.Connection.RemotePort;
    Results.Ok($"Hello ${ip} -> Port : ${port}");
});


app.MapGet("/todos", async (AppDbContext context) =>
{
    var todos = await context.Todos
        .AsNoTracking()
        .ToListAsync();

    return Results.Ok(todos);
});


app.MapGet("/todos/{id:int}", async (int id, AppDbContext context) =>
{
    var todo = await context.Todos.FindAsync(id);

    return todo is null
        ? Results.NotFound()
        : Results.Ok(todo);
});


app.MapPost("/todos", async (TodoDto dto, AppDbContext context) =>
{
    var todo = new Todo
    {
        Title = dto.Title,
        Description = dto.Description,
        IsDone = dto.IsDone
    };

    await context.Todos.AddAsync(todo);
    await context.SaveChangesAsync();

    return Results.Created($"/todos/{todo.Id}", todo);
});


app.MapPut("/todos/{id:int}", async (int id, TodoDto dto, AppDbContext context) =>
{
    var todo = await context.Todos.FindAsync(id);

    if (todo is null)
        return Results.NotFound();

    todo.Title = dto.Title;
    todo.Description = dto.Description;
    todo.IsDone = dto.IsDone;

    await context.SaveChangesAsync();

    return Results.Ok(todo);
});


app.MapDelete("/todos/{id:int}", async (int id, AppDbContext context) =>
{
    var todo = await context.Todos.FindAsync(id);

    if (todo is null)
        return Results.NotFound();

    context.Todos.Remove(todo);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();