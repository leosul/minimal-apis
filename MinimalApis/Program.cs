using Microsoft.EntityFrameworkCore;
using MinimalApis.Data;
using MinimalApis.Models;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MinimalApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

MapActions(app);

app.Run();

void MapActions(WebApplication app)
{
    app.MapGet("/clientes", async (
    MinimalApiDbContext context) =>
    await context.Clientes.ToListAsync())
    .WithName("GetClientes")
    .WithTags("Clientes");

    app.MapGet("/clientes/{id}", async (
        Guid id,
        MinimalApiDbContext context) =>

        await context.Clientes.FindAsync(id)
            is Cliente clientes
            ? Results.Ok(clientes)
            : Results.NotFound())

        .Produces<Cliente>(StatusCodes.Status200OK)
        .Produces<Cliente>(StatusCodes.Status404NotFound)
        .WithName("GetClientesById")
        .WithTags("Clientes");

    app.MapPost("/clientes", async (
        MinimalApiDbContext context,
        Cliente cliente) =>
    {
        if (!MiniValidator.TryValidate(cliente, out var errors))
            return Results.ValidationProblem(errors);

        context.Clientes.Add(cliente);
        var result = await context.SaveChangesAsync();

        return result > 0 ?
        Results.CreatedAtRoute("GetClientesById", new { id = cliente.Id }, cliente) :
        Results.BadRequest("Erro ao salvar o cliente");
    }).ProducesValidationProblem()
        .Produces<Cliente>(StatusCodes.Status201Created)
        .Produces<Cliente>(StatusCodes.Status400BadRequest)
        .WithName("PostCliente")
        .WithTags("Clientes");

    app.MapPut("/clientes/{id}", async (
            Guid id,
            MinimalApiDbContext context,
            Cliente cliente) =>
    {
        var clienteDb = await context.Clientes.AsNoTracking<Cliente>().FirstOrDefaultAsync(s => s.Id == id);
        if (clienteDb == null) return Results.NotFound();

        if (!MiniValidator.TryValidate(clienteDb, out var errors))
            return Results.ValidationProblem(errors);

        context.Clientes.Update(cliente);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.CreatedAtRoute("GetClientesById", new { id = cliente.Id }, cliente)
                : Results.BadRequest("Erro ao salvar o cliente");

    }).ProducesValidationProblem()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("PutCliente")
        .WithTags("Clientes");

    app.MapDelete("/cliente/{id}", async (
            Guid id,
            MinimalApiDbContext context) =>
    {
        var cliente = await context.Clientes.FindAsync(id);
        if (cliente == null) return Results.NotFound();

        context.Clientes.Remove(cliente);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("Erro ao salvar o cliente");

    }).Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("DeleteCliente")
        .WithTags("Clientes");
}