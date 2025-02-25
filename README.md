Install the Dopper Extention.
Install the NpgSql Extention.

_____________________________________________________________

Add the **CORS** Connection code in **Program.cs** File ..

**After AddSwaggerGen()**

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

....................
**After HTTP request pipeline**

// Enable CORS middleware
app.UseCors("AllowAll");

_____________________________________________________________

Connect to the **DataBase** in the **appsettings.json**

"AllowedHosts": "*",
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PersonDB;User Id=postgres;Password=123456789;"
}

_____________________________________________________________

