using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Segurity;
using Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JwtIdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Serilog.ILogger _logger;
        private readonly IIdentityRepo _identity;
        public UserController(Serilog.ILogger logger, IIdentityRepo identity)
        {
            _logger = logger;
            _identity = identity;
        }

        // GET: api/<UserController>
        [Authorize] 
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var model = await _identity.GetUsersAsync();
                if (model.Count() > 0)
                {
                    return Ok(model);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, HttpContext.Request.Path);
                return BadRequest(ex);
            }
        }

        // GET api/<UserController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var model = await _identity.GetUserByIdAsync(id);
                if (model != null)
                {
                    return Ok(model);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, HttpContext.Request.Path);
                return BadRequest(ex);
            }
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Users model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                }

                var result = await _identity.RegisterAsync(model);
                if (result.Succeeded)
                {
                    return Ok("Registro guardado");
                }
                else
                {                    
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, HttpContext.Request.Path);
                return BadRequest(ex);
            }
        }

        // POST api/<UserController>
        [HttpGet("{email}/{password}")] 
        public async Task<IActionResult> Get(string email, string password) 
        {
            try
            {
                var user = await _identity.GetUserByEmailAsync(email);
                if (user != null)
                {
                    var result = await _identity.SignInJwtTokenAsync(user, password);
                    return Ok(result); 
                }
                else
                {
                    return NotFound("");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, HttpContext.Request.Path);
                return BadRequest(ex);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
