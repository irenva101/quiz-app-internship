using Konteh.BackOfficeApi.Features.Questions;
using Konteh.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Konteh.BackOfficeApi.Tests;

public class CreateQuestionTests : IntegrationBase
{
    [Test]
    public async Task CreateQuestionTest()
    {
        var command = new CreateOrUpdateQuestion.Command
        {
            Category = Domain.Enum.Category.OOP,
            TypeButton = Domain.Enum.TypeButton.Radiobutton,
            Text = "Test question",
            Answers = [
                new(){
                    IsCorrect = false,
                    Text = "First answer"
                },
                new(){
                    IsCorrect = true,
                    Text = "Second answer"
                }
                ]
        };

        var response = await _httpClient.PostAsync("api/questions", JsonContent.Create(command));
        response.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KontehContext>();
        var questions = await dbContext.Questions.Include(x => x.Answers).ToListAsync();

        await Verify(questions)
            .IgnoreMembers("Id");
    }
}