namespace BackToTheDawnTrainer;

public interface ILogHandlerProvider
{
	LogHandler? Handler { get; set; }
}
