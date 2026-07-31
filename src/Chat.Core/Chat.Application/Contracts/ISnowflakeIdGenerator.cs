namespace Chat.Application.Contracts;

public interface ISnowflakeIdGenerator
{
    long NextId();
}
