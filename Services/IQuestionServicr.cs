using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services;

public interface IQuestionServicr
{
	Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request,CancellationToken cancellationToken);
}
