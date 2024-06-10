using Konteh.BackOfficeApi.Features.Questions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Konteh.BackOfficeApi.Controllers;

[Route("api/questions")]
[Authorize]
[ApiController]
public class QuestionsController : Controller
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllQuestions.Response>> GetAll([FromQuery] GetAllQuestions.Query query)
    {

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateOrUpdate(CreateOrUpdateQuestion.Command command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetById.Response), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetById.Response>> GetById(int id)
    {
        var response = await _mediator.Send(new GetById.Query { Id = id });
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteQuestion.Command { Id = id });
        return Ok();
    }
}