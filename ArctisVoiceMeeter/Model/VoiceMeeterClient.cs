using System;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;

namespace ArctisVoiceMeeter.Model;

public class VoiceMeeterClient : IDisposable
{
    private readonly RemoteApiWrapper _api;

    public VoiceMeeterClient()
    {
        string dllPath = PathHelper.GetDllPath();
        _api = new RemoteApiWrapper(dllPath);
        _api.Login();
    }

    public bool TrySetGain(uint stripIndex, float dbValue)
    {
        int result = _api.SetParameter($"Strip[{stripIndex}].gain", dbValue);
        return result == 0;
    }

    public void Dispose()
    {
        _api.Logout();
        _api.Dispose();
    }
}