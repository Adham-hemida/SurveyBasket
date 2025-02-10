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
	}
}
