using Konteh.FrontOfficeApi.Features.Candidates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Konteh.FrontOfficeApi.Controllers;

[ApiController]
[Route("api/candidates")]
public class CandidateController : Controller
{
    private readonly IMediator _mediator;
    public CandidateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RegisterCandidate(RegisterCandidate.Command command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
