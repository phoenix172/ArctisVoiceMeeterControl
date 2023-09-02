//using System.Threading;
//using System.Threading.Tasks;

//namespace ArctisVoiceMeeter;

//public enum VoiceMeeterVirtualInputStrip
//{
//    VoiceMeeterInput = 7,
//    VoiceMeeterAuxInput = 8,
//    VoiceMeeterVAIO3Input = 9
//}


//public class ArctisVoiceMeeterChannelBinding 
//{
//    private CancellationTokenSource _tokenSource;
//    private Task? bindingTask = null;

//    public uint ArctisGameVolume { get; set; }
//    public uint ArctisChatVolume { get; set; }
//    public float VoiceMeeterVolume { get; }
//    public float VoiceMeeterMinVolume { get; set; }
//    public float VoiceMeeterMaxVolume { get; set; }

//    public ArctisChannel BoundChannel { get; set; }
//    //public uint

//    public void Poll()
//    {
//        var pollTask = Task.Run(async () =>
//        {
//            while (true)
//            {
//                var status = _arctis.GetStatus();
//                CurrentChatVolume = status.ChatVolume;
//                CurrentGameVolume = status.GameVolume;

//                if (BindChatChannel)
//                {
//                    float scaledValue = GetScaledChannelVolume(ArctisChannel.Chat);
//                    _voiceMeeter.TrySetGain(StripIndex, scaledValue);
//                }

//                if (BindGameChannel)
//                {
//                    float scaledValue = GetScaledChannelVolume(ArctisChannel.Game);
//                    _voiceMeeter.TrySetGain(StripIndex, scaledValue);
//                }

//                await Task.Delay(1000 / 60);
//            }
//        });
//    }
//}