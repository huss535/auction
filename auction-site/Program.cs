

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AucDBContext>(options => options.UseSqlite(builder.Configuration["WebAPIConnection"]));
builder.Services.AddScoped<IAucRepo, AucRepo>();
builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, AucAuthHandler>("MyAuthentication", null);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin"));
    options.AddPolicy("UserOnly", policy => {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
            (c.Type == "UserName" )));
    });

    options.AddPolicy("UserAndAdmin", policy => {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
            (c.Type == "UserName" || c.Type == "admin")));
    });


});
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

