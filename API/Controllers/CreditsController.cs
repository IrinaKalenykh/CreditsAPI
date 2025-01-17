using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CreditsController : ControllerBase
{
    private readonly ICreditsService _creditsService;

    public CreditsController(ICreditsService creditsService)
    {
        _creditsService = creditsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCredits()
    {
        return HandleResult(await _creditsService.GetCreditListAsync());
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatictics()
    {
        return HandleResult(await _creditsService.GetCreditStatisticsAsync());
    }

    private ActionResult HandleResult<T>(T result, string message = null)
    {
        try 
        {
            if (result == null)
                return message != null ? NotFound(new { Message = message }) : NotFound();
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "An error occurred while processing the request.",
                Details = ex.Message
            });
        }
    }
}