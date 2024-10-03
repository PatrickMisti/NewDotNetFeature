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

    [HttpPost]
    [Route("create")]
    public async Task<IResult> Create(KeyEntry entry)
    {
        var result = await _keyStoreService.AddKey(entry);

        if (!result)
            return Results.BadRequest();

        return Results.Ok();
    }
}