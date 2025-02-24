using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly string _connectionString;

    public FileController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            // Read file data into byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "INSERT INTO files (file_name, file_data) VALUES (@name, @data) RETURNING id;";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", file.FileName);
            cmd.Parameters.AddWithValue("@data", fileBytes);

            var newId = await cmd.ExecuteScalarAsync();
            return Ok(new { FileId = newId, FileName = file.FileName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT file_name, file_data FROM files WHERE id = @id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string fileName = reader.GetString(0);
                byte[] fileData = (byte[])reader["file_data"];

                return File(fileData, "application/pdf", fileName);
            }
            return NotFound("File not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateFile(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            // Read file data into byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Check if file exists
            string checkQuery = "SELECT COUNT(*) FROM files WHERE id = @id";
            using var checkCmd = new NpgsqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@id", id);
            int fileExists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

            if (fileExists == 0)
                return NotFound("File not found.");

            // Update file data
            string updateQuery = "UPDATE files SET file_name = @name, file_data = @data WHERE id = @id";
            using var updateCmd = new NpgsqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@name", file.FileName);
            updateCmd.Parameters.AddWithValue("@data", fileBytes);
            updateCmd.Parameters.AddWithValue("@id", id);

            await updateCmd.ExecuteNonQueryAsync();
            return Ok(new { Message = "File updated successfully", FileId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Check if file exists
            string checkQuery = "SELECT COUNT(*) FROM files WHERE id = @id";
            using var checkCmd = new NpgsqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@id", id);
            int fileExists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

            if (fileExists == 0)
                return NotFound("File not found.");

            // Delete the file
            string deleteQuery = "DELETE FROM files WHERE id = @id";
            using var deleteCmd = new NpgsqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@id", id);
            await deleteCmd.ExecuteNonQueryAsync();

            return Ok(new { Message = "File deleted successfully", FileId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


}
