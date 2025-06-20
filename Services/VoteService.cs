using SurveyBasket.Contracts.Votes;
using SurveyBasket.UnitOfWorks;

namespace SurveyBasket.Services;

public class VoteService(ApplicationDbContext context,IUnitOfWork unitOfWork) : IVoteService
{
	private readonly ApplicationDbContext _context = context;
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken)
	{
		//var hasVoted = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

		var hasVoted = await _unitOfWork.Repository<Vote>()
			.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken: cancellationToken);

		if (hasVoted)
			return Result.Failure(VoteErrors.DuplicatedVote);
		//var pollIsExist = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished
		//&& x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
		//&& x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

		var pollIsExist = await _unitOfWork.Repository<Poll>()
			.AnyAsync(x => x.Id == pollId && x.IsPublished
				&& x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
				&& x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellationToken);

		if (!pollIsExist)
			return Result.Failure(PollErrors.PollNotFound);

		//var questionIds = await _context.Questions
		//	.Where(x => x.PollId == pollId)
		//	.Select(x => x.Id)
		//	.ToListAsync(cancellationToken);

		var questionIds = await _unitOfWork.Repository<Question>()
			.FindAllProjectedWithSelect(x => x.Id, x => x.PollId == pollId);


		if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(questionIds))
			return Result.Failure(VoteErrors.InvalidQuestions);

		var vote = new Vote
		{
			PollId = pollId,
			UserId = userId,
			//VoteAnswers = request.Answers.Adapt<ICollection<VoteAnswer>>()
			VoteAnswers = request.Answers.Select(x => new VoteAnswer
			{
				QuestionId = x.QuestionId,
				AnswerId = x.AnswerId
			}).ToList()
		};
		//await _context.AddAsync(vote, cancellationToken);
		//await _context.SaveChangesAsync(cancellationToken);
		await _unitOfWork.Repository<Vote>().CreateAsync(vote, cancellationToken: cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
		return Result.Success();
	}
}
