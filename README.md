Install the Dopper Extention.
Install the NpgSql Extention.

Add the **CORS** Connection code in **Program.cs** File ..

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

....................

// Enable CORS middleware
app.UseCors("AllowAll");

