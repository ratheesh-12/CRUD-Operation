using backend.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public DetailsController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }
        [HttpGet]
        [Route("insert")]
        public IActionResult Get()
        {
            try
            {
                using (var conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    string cmd = "SELECT * FROM person";
                    var res = conn.Query<Person>(cmd);
                    return Ok (res);
                }
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok();
        }
        [HttpPost]
        [Route("details")]
        public IActionResult Post([FromBody]Person person)
        {
            try
            {
                using (var conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    string cmd = "INSERT INTO public.person(name, email, message)VALUES (@name, @email, @message);";
                    using (var command = new NpgsqlCommand(cmd, conn)) {
                        command.Parameters.AddWithValue("@name", person.Name);
                        command.Parameters.AddWithValue("@email", person.Email);
                        command.Parameters.AddWithValue("@message", person.Message);
                        command.ExecuteNonQuery();
                    }
                    

                    return Ok("Succes");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(person.Name);
        }
        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult Put(int id, [FromBody]Person person)
        {
            try
            {
                using (var conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    string cmd = "UPDATE public.person SET name=@name, email=@email, message=@message WHERE id=@id;";
                    using (var command = new NpgsqlCommand(cmd, conn))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", person.Name);
                        command.Parameters.AddWithValue("@email", person.Email);
                        command.Parameters.AddWithValue("@message", person.Message);
                        command.ExecuteNonQuery();
                    }
                    return Ok("Succes");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("details/{id}")]
        public IActionResult Post(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    string cmd = "DELETE FROM public.person WHERE id=@id;";
                    using(var command = new NpgsqlCommand(cmd,conn))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                    return Ok("Succes");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
