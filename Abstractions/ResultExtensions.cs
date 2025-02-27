namespace SurveyBasket.Abstractions;

public static class ResultExtensions
{
	public static ObjectResult ToProblem(this Result result, int statusCode)
	{
		if (result.IsSuccess)
			throw new InvalidOperationException("Can not create problem details for successful result");


        var problem=Results.Problem(statusCode: statusCode);

		var problemDetails= problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
		problemDetails!.Extensions= new Dictionary<string, object?>()
		{
			{"errors",new []{ result.Error} }
		};

		return new ObjectResult(problemDetails);
	}
}