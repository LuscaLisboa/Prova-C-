using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using prova.data;
using prova.models;
using prova.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc"))
        };
    });

var app = builder.Build();


app.UseHttpsRedirection();

//-------------------Login--------------------------------------
app.MapPost("/login", async (HttpContext context) =>
{
    // Receber request
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var json = JsonDocument.Parse(body);
    var username = json.RootElement.GetProperty("username").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    // Será complementada com a sevice na próxima aula
    var token = "";
    if (senha == "comiteupai")
    {
        token = GenerateToken(email); // Método GenerateToken será reimplementado em uma classe especializada
    }

    // return token;
    await context.Response.WriteAsync(token);
});

// Rota segura: toda rota é tem corpo de código parecido
app.MapGet("/rotaSegura", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Rota segura acessada com sucesso.");
});

//-------------------Token Jwt--------------------------------------
string GenerateToken(string data)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var secretKey = Encoding.ASCII.GetBytes("macacomacacomacacomacacomacacomacacomacacomacaco"); // Esta chave será gravada em uma var de ambiente
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Expires = DateTime.UtcNow.AddHours(1), // Token expira em 1 hora 
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature
        )
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token); // Converte o token em string
}

//-------------------Endpoint serviço--------------------------------------
app.MapPost("/servicos", async (HttpContext context, ServicoService servicoService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var servico = JsonSerializer.Deserialize<Servico>(body);

    if (servico != null)
    {
        await servicoService.AddServicoAsync(servico);
        context.Response.StatusCode = 201; // Created
        await context.Response.WriteAsync("Serviço criado com sucesso.");
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("Dados do serviço inválidos.");
    }
});

app.MapPut("/servicos/{id}", async (HttpContext context, ServicoService servicoService, int id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var servico = JsonSerializer.Deserialize<Servico>(body);

    if (servico == null || servico.Id != id)
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("Dados do serviço inválidos.");
        return;
    }

    // Verificar se existe no banco de dados
    var existingServico = await servicoService.GetServicoByIdAsync(id);
    if (existingServico == null)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Serviço não encontrado.");
        return;
    }

    // Atualizar
    existingServico.Nome = servico.Nome;
    existingServico.Preco = servico.Preco;
    existingServico.Status = servico.Status;

    await servicoService.UpdateServicoAsync(existingServico);

    context.Response.StatusCode = 200; // OK
    await context.Response.WriteAsync("Serviço atualizado com sucesso.");
});

app.MapGet("/servicos/{id}", async (HttpContext context, ServicoService servicoService, int id) =>
{
    var servico = await servicoService.GetServicoByIdAsync(id);

    if (servico == null)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Serviço não encontrado.");
        return;
    }

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(JsonSerializer.Serialize(servico));
});

//-------------------Endpoint serviço--------------------------------------
app.MapPost("/contratos", async (HttpContext context, ContratoService contratoService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    // Deserializar o objeto
    var contrato = JsonSerializer.Deserialize<Contrato>(body);

    if (contrato != null)
    {
        await contratoService.AddContratoAsync(contrato);
        context.Response.StatusCode = 201; // Created
        await context.Response.WriteAsync("Contrato registrado com sucesso.");
    }
    else
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("Dados do contrato inválidos.");
    }
});

app.MapGet("/clientes/{clienteId}/servicos", async (HttpContext context, ContratoService contratoService, int clienteId) =>
{
    var servicos = await contratoService.GetServicosByClienteIdAsync(clienteId);

    if (servicos == null || servicos.Count == 0)
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Serviços não encontrados para este cliente.");
        return;
    }

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(JsonSerializer.Serialize(servicos));
});

app.Run();