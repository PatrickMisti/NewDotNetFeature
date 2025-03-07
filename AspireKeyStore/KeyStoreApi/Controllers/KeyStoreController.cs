using KeyStoreApi.Models;
using KeyStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyStoreApi.Controllers;

[ApiController]
[Route("api/[controller]" )]
public class KeyStoreController : ControllerBase
{
    private IKeyStoreService _keyStoreService;

    public KeyStoreController(IKeyStoreService keyStoreService)
    {
        _keyStoreService = keyStoreService;
    }

    [HttpGet]
    [Route("all")]
    public async Task<IResult> GetAll()
    {
        return Results.Ok(await _keyStoreService.GetAsync());
    }


    [HttpGet]
    [Route("get/{id}")]
    public async Task<IResult> GetById(int id)
    {
        var result = await _keyStoreService.GetByIdAsync(id);

        if (result == null)
            return Results.NotFound();

        return Results.Ok(result);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IResult> Create(KeyEntry entry)
    {
        var result = await _keyStoreService.AddKeyAsync(entry);

        if (!result)
            return Results.BadRequest();

        return Results.Ok();
    }

    [HttpPut]
    [Route("update")]
    public async Task<IResult> Update(KeyEntry entry)
    {
        var result = await _keyStoreService.UpdateKeyAsync(entry);

        if (!result)
            return Results.BadRequest();

        return Results.Ok();
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IResult> DeleteById(int id)
    {
        var result = await _keyStoreService.DeleteKeyByIdAsync(id);

        if (!result)
            return Results.BadRequest();

        return Results.Ok();
    }
}