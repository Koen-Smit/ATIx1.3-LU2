﻿using LU2_WebApi.Models;
using LU2_WebApi.Repositorys;
using Microsoft.AspNetCore.Mvc;

namespace LU2_WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironment2DRepository _repository;

        public Environment2DController(IEnvironment2DRepository repository)
        {
            _repository = repository;
        }

        // GET: /Environment2D
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetAll()
        {
            var environments = await _repository.GetAllAsync();
            return Ok(environments);
        }

        // GET: /Environment2D/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Environment2D>> GetById(int id)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                return NotFound("Environment not found.");

            return Ok(environment);
        }

        // POST: /Environment2D
        [HttpPost]
        public async Task<ActionResult<Environment2D>> Create([FromBody] Environment2D environment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEnvironment = await _repository.AddAsync(environment);
            return CreatedAtAction(nameof(GetById), new { id = createdEnvironment.Id }, createdEnvironment);
        }

        // PUT: /Environment2D/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Environment2D updatedEnvironment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _repository.UpdateAsync(id, updatedEnvironment);
            if (!success)
                return NotFound("Environment not found.");

            return NoContent();
        }

        // DELETE: /Environment2D/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
                return NotFound("Environment not found.");

            return NoContent();
        }
    }
}
