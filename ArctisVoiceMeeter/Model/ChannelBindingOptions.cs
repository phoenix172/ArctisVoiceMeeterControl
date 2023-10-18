using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ChannelBindingOptions : ObservableObject
{
    [ObservableProperty] private string _bindingName;
    [ObservableProperty] private float _voiceMeeterMinVolume;
    [ObservableProperty] private float _voiceMeeterMaxVolume;
    [ObservableProperty] private uint _boundStrip;

    private List<HeadsetChannelBinding> _boundHeadsets;

    [JsonConstructor]
    private ChannelBindingOptions()
        :this(string.Empty)
    {
    }

    public ChannelBindingOptions(string bindingName)
    {
        _bindingName = bindingName;
    }

    public List<HeadsetChannelBinding> BoundHeadsets
    {
        get => _boundHeadsets;
        set
        {
            _boundHeadsets?.ForEach(x => x.PropertyChanged -= OnBoundHeadsetPropertyChanged);
            SetProperty(ref _boundHeadsets, value);
            _boundHeadsets?.ForEach(x => x.PropertyChanged += OnBoundHeadsetPropertyChanged);
        }
    }

    public void CopyFrom(ChannelBindingOptions options)
    {
        BindingName = options.BindingName;
        VoiceMeeterMinVolume = options.VoiceMeeterMinVolume;
        VoiceMeeterMaxVolume = options.VoiceMeeterMaxVolume;
        BoundStrip = options.BoundStrip;

        BoundHeadsets = options.BoundHeadsets.ToList();
    }

    private void OnBoundHeadsetPropertyChanged(object? o, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        OnPropertyChanged(nameof(BoundHeadsets));
    }
}