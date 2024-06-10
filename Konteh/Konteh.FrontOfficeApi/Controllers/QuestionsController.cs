using Konteh.FrontOfficeApi.Features.Questions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Konteh.FrontOfficeApi.Controllers;

[Route("api/questions/random")]
[ApiController]
public class QuestionsController : Controller
{
    private readonly IMediator _mediator;
    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<GenerateExam.Response>> GenerateExam(GenerateExam.Command command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
