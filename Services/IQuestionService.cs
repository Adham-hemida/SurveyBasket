using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services;

public interface IQuestionService
{
	Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,CancellationToken cancellationToken = default);
	Task<Result<QuestionResponse>> GetAsync(int pollId,int id,CancellationToken cancellationToken=default);
	Task<Result<QuestionResponse>> AddAsync(int pollId,QuestionRequest request,CancellationToken cancellationToken=default);
	Task<Result>ToggleSatausAsync(int pollId,int id ,CancellationToken cancellationToken=default);
	Task<Result>UpdateAsync(int pollId,int id,QuestionRequest request,CancellationToken cancellationToken=default);
}
