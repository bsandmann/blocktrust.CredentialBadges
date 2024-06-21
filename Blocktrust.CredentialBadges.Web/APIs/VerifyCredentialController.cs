using Microsoft.AspNetCore.Mvc;
using MediatR;
// using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;

namespace Blocktrust.CredentialBadges.Web.APIs;

[ApiController]
[Route("api/[controller]")]
public class VerifyCredentialController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyCredentialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCredentialById(Guid id)
    {
        //parse the id from string to Guid type 
        var cId = Guid.Parse(id.ToString());          
        
        // Create the request
        // var request = new VerifyOpenBadgeRequest(cId);

        // Send the request to the handler
        // var result = await _mediator.Send(request);

        // Check if the result was successful
        // if (result.IsFailed)
        // {
        //     return NotFound(result.Errors.First().Message);
        // }
        //
        // return Ok(result.Value);

        return Ok(cId);
    }
}