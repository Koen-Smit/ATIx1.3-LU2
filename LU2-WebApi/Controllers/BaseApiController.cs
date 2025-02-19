﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public abstract class BaseApiController<T> : ControllerBase where T : class
{
    private readonly IRepository<T> _repository;

    protected BaseApiController(IRepository<T> repository)
    {
        _repository = repository;
    }

    // GET: /{Entity} - Alleen gebruikers met de "entity:read" claim
    [Authorize(Policy = "CanReadEntity")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAll()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching {typeof(T).Name}: {ex.Message}");
        }
    }

    // GET: /{Entity}/{id} - Alleen gebruikers met de "entity:read" claim
    [Authorize(Policy = "CanReadEntity")]
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetById(int id)
    {
        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound($"{typeof(T).Name} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching {typeof(T).Name}: {ex.Message}");
        }
    }

    // POST: /{Entity} - Alleen gebruikers met "entity:write" claim of role "Admin"
    [Authorize(Policy = "CanWriteEntity")]
    [HttpPost]
    public async Task<ActionResult<T>> Create([FromBody] T entity)
    {
        try
        {
            if (entity == null)
                return new BadRequestObjectResult("Entity cannot be null");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEntity = await _repository.AddAsync(entity);
            if (createdEntity == null)
                return StatusCode(500, $"{typeof(T).Name} creation failed.");

            var idProperty = createdEntity.GetType().GetProperty("Id");
            var idValue = idProperty?.GetValue(createdEntity);
            return CreatedAtAction(nameof(GetById), new { id = idValue }, createdEntity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching {typeof(T).Name}: {ex.Message}");
        }
    }

    // PUT: /{Entity}/{id} - Alleen gebruikers met "entity:write" claim of role "Admin"
    [Authorize(Policy = "CanWriteEntity")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] T updatedEntity)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedEntity == null)
                return BadRequest($"{typeof(T).Name} cannot be null.");

            var success = await _repository.UpdateAsync(id, updatedEntity);
            if (!success)
                return NotFound($"{typeof(T).Name} not found or update failed.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching {typeof(T).Name}: {ex.Message}");
        }
    }

    // DELETE: /{Entity}/{id} - Alleen gebruikers met "entity:delete" claim of role "Admin"
    [Authorize(Policy = "CanDeleteEntity")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
                return NotFound($"{typeof(T).Name} not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching {typeof(T).Name}: {ex.Message}");
        }
    }
}
