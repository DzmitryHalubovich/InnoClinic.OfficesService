namespace Offices.Services.Abstractions;

public interface IRedisCahceService
{
    public T GetCachedData<T>(string key);

    public void SetCachedData<T>(string key, T data, TimeSpan cacheDuration);
}
