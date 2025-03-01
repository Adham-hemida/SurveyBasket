namespace SurveyBasket.Abstractions;

public class Result
{
	public Result(bool isSuccess, Error error)
	{
		if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
			throw new InvalidOperationException();

		IsSuccess = isSuccess;
		Error = error;

	}
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public Error Error { get; } = default!;

	// this two method that as did not return any response
	public static Result Success() => new(true, Error.None);
	public static Result Failure(Error error) => new(false, error);

	// this two method that as if I need to return response
	public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
	public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}


public class Result<TValue> : Result
{
	private readonly TValue? _value;
	public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
	{
		_value = value;
	}
	//I need to access the value as it is private
	public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Faliure result Can not have value");
}