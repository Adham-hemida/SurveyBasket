namespace SurveyBasket.Mapping;

public class MappingConfiguarations : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		 config.NewConfig<Poll, PollResponse>() // Poll to pollresponse 
			.Map(dest => dest.Notes, src => src.Description);
	}
}
