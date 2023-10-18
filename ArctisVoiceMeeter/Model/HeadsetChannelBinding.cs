using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

//public class HeadsetChannelBindingToNullableBooleanConverter : MarkupExtension, IValueConverter
//{
//    // Enabled & Game => true
//    // Enabled & Chat => false
//    // Disabled => null

//    public override object ProvideValue(IServiceProvider serviceProvider)
//    {
//        return this;
//    }

//    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        if (value is not HeadsetChannelBinding binding || targetType != typeof(bool?)) return null;

//        (bool IsEnabled, ArctisChannel BoundChannel) switchKey = (binding.IsEnabled, binding.BoundChannel);
//        bool? result = switchKey switch
//        {
//            (IsEnabled: true, BoundChannel: ArctisChannel.Game) => true,
//            (IsEnabled: true, BoundChannel: ArctisChannel.Chat) => false,
//            _ => null
//        };
//        return result;
//    }

//    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        if (parameter is not int index) return null;

//        var result = new HeadsetChannelBinding(index, ArctisChannel.Game, isEnabled: false);
        
//        if (value == null)
//            return result;

//        if (value is not bool boolValue || targetType != typeof(HeadsetChannelBinding)) 
//            return result;
        
//        result = BoolToHeadsetChannelBinding(boolValue, index);

//        return result;
//    }

//    private static HeadsetChannelBinding BoolToHeadsetChannelBinding(bool boolValue, int index)
//    {
//        HeadsetChannelBinding result;
//        result = boolValue switch
//        {
//            true => new HeadsetChannelBinding(index, ArctisChannel.Game, isEnabled: true),
//            false => new HeadsetChannelBinding(index, ArctisChannel.Chat, isEnabled: true)
//        };
//        return result;
//    }
//}

public partial class HeadsetChannelBinding : ObservableObject
{
    public HeadsetChannelBinding(int index, ArctisChannel boundChannel, ChannelBinding channelBinding,
        bool isEnabled = true)
    {
        ChannelBinding = channelBinding;
        Index = index;
        BoundChannel = boundChannel;
        IsEnabled = isEnabled;
    }

    public ChannelBinding ChannelBinding { get; }

    [ObservableProperty] private int _index;
    [ObservableProperty] private ArctisChannel _boundChannel;
    [ObservableProperty] private bool _isEnabled;


    //public bool IsEnabled
    //{
    //    get => _isEnabled;
    //    set
    //    {
    //        if (value)
    //            Enable();
    //        else
    //            Disable();
    //        SetProperty(ref _isEnabled, value);
    //    }
    //}

    //public void Enable()
    //{

    //}

    //public void Disable()
    //{

    //}
}