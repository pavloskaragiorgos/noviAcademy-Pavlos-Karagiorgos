namespace WorldRank.Application.Interfaces;

public interface ICache
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan ttl);
    void Remove(string key);
}
