using SurveyBasket.Contracts.Questions;
using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Mapping;

public class MappingConfiguarations : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		//if the attribute did not same as the property name, we can use Map method to map the property
		//config.NewConfig<Poll, PollResponse>() // Poll to pollresponse 
		//	.Map(dest => dest.Notes, src => src.Description);
		//config.NewConfig<CreatePollRequest, Poll>() // CreatePollRequest to Poll
		//	.Map(dest => dest.Description, src => src.Notes);


		config.NewConfig<QuestionRequest, Question>()
			.Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

		config.NewConfig<RegisterRequest, ApplicationUser>()
			.Map(dest => dest.UserName, src => src.Email);

		config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
			.Map(dest => dest, src => src.user)
			.Map(dest => dest.Roles, src => src.roles);

		config.NewConfig<CreateUserRequest, ApplicationUser>()
		.Map(dest => dest.UserName, src => src.Email)
		.Map(dest => dest.EmailConfirmed, src => true);

		config.NewConfig<UpdateUserRequest, ApplicationUser>()
		.Map(dest => dest.UserName, src => src.Email)
		.Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());
	}
}
