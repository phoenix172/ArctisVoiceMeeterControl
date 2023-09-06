namespace ArctisVoiceMeeter.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public interface IWritableOptions<out T> : IOptionsMonitor<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
}

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly IOptionsMonitor<T> _options;
    private readonly string _section;
    private readonly string _file;

    public WritableOptions(
        IOptionsMonitor<T> options,
        string section,
        string file)
    {
        _options = options;
        _section = section;
        _file = file;
    }

    public T Get(string name) => _options.Get(name);

    public IDisposable OnChange(Action<T, string> listener) => _options.OnChange(listener);

    public T CurrentValue => _options.CurrentValue;

    public void Update(Action<T> applyChanges)
    {
        var physicalPath = _file;

        var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
        var sectionObject = jObject.TryGetValue(_section, out JToken section) ?
            JsonConvert.DeserializeObject<T>(section.ToString()) : (CurrentValue ?? new T());

        applyChanges(sectionObject);

        jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
        File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented));
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureWritable<T>(
        this IServiceCollection services,
        IConfigurationSection section,
        string file = "appsettings.json") where T : class, new()
    {
        services.Configure<T>(section);
        services.AddTransient<IWritableOptions<T>>(provider =>
        {
            var options = provider.GetService<IOptionsMonitor<T>>();
            return new WritableOptions<T>(options, section.Key, file);
        });
        return services;
    }
}