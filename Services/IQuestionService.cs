using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services;

public interface IQuestionService
{
	Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request,CancellationToken cancellationToken);
}
