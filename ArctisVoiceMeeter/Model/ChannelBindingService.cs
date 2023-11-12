using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Awesome.Net.WritableOptions;

namespace ArctisVoiceMeeter.Model
{
    public class ChannelBindingService : IDisposable
    {
        public event NotifyCollectionChangedEventHandler BindingsChanged
        {
            add => Bindings.CollectionChanged += value;
            remove => Bindings.CollectionChanged -= value;
        }

        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _presets;
        private readonly HeadsetPoller _headsetPoller;
        private readonly VoiceMeeterClient _voiceMeeterClient;

        public ChannelBindingService(HeadsetPoller headsetPoller, VoiceMeeterClient voiceMeeterClient,
            IWritableOptions<ArctisVoiceMeeterPresets> presets)
        {
            _presets = presets;
            _headsetPoller = headsetPoller;
            _voiceMeeterClient = voiceMeeterClient;

            _headsetPoller.Bind();

            Bindings = CreateBindingsCollection();
            BindingsChanged += (_, _) => UpdateChannelBindings();
            //TODO: _headsetPoller.HeadsetCountChanged += (_, _) => UpdateChannelBindings()

            UpdateChannelBindings();
        }

        private void UpdateChannelBindings()
        {
            var headsetBindings = CalculateHeadsetBindings().ToArray();
            HeadsetBindings = new ObservableCollection<HeadsetChannelBinding>(headsetBindings);

            var headsetsByChannelName = headsetBindings.ToLookup(x => x.ChannelBindingName);

            foreach (var channelBinding in Bindings)
            {
                channelBinding.BoundHeadsets = headsetsByChannelName[channelBinding.BindingName].ToArray();
            }
        }

        public ObservableCollection<ChannelBinding> Bindings { get; }
        public ObservableCollection<HeadsetChannelBinding> HeadsetBindings { get; private set; }
        
        public ChannelBinding AddNewBinding()
        {
            string bindingName = "Preset " + (Bindings.Count + 1);
            while (BindingExists(bindingName))
                bindingName += "_";

            return AddBinding(new ChannelBindingOptions(bindingName));
        }

        public ChannelBinding AddBinding(ChannelBindingOptions options)
        {
            if (_presets.Value.Any(x => x.BindingName == options.BindingName))
                throw new ArgumentException("A Channel Binding with this name already exists.", nameof(options.BindingName));

            _presets.Update(x => x.Add(options));

            var binding = CreateBinding(options);
            Bindings.Add(binding);
            return binding;
        }

        public void RenameBinding(string oldName, string newName)
        {
            _presets.Update(presets =>
            {
                var binding = presets.FirstOrDefault(x => x.BindingName == oldName);
                binding.BindingName = newName;
            });
        }

        public void ChangeBinding(ChannelBindingOptions binding)
        {
            _presets.Update(presets =>
            {
                var previous = presets.FirstOrDefault(x => x.BindingName == binding.BindingName);
                if (previous == null)
                    throw new ArgumentException("An existing Channel Binding with this name was not found.",
                        nameof(binding.BindingName));
                previous.CopyFrom(binding);
            });
        }

        public ChannelBinding? GetBinding(string name) => Bindings.FirstOrDefault(x => x.BindingName == name);

        public bool BindingExists(string name) => GetBinding(name) != null;

        public bool RemoveBinding(string name)
        {
            bool result = false;

            _presets.Update(presets =>
            {
                var bindingOption = presets.FirstOrDefault(x => x.BindingName == name);
                if (bindingOption == null) return;
                presets.Remove(bindingOption);
                result = true;
            });

            if (!result) return false;

            var binding = GetBinding(name);
            if (binding != null)
                Bindings.Remove(binding);

            return true;
        }

        public IEnumerable<HeadsetChannelBinding> GetHeadsetBindings(int? headsetIndex)
        {
            var bindings = HeadsetBindings.Where(x => x.Index == headsetIndex);

            return bindings;
        }

        private IEnumerable<HeadsetChannelBinding> CalculateHeadsetBindings()
        {
            var bindingsLookup = Bindings
                .SelectMany(x => x.BoundHeadsets)
                .ToDictionary(x => (x.Index, x.ChannelBindingName), x=>(x.BoundChannel, x.IsEnabled));

            var headsetIndices = Enumerable.Range(0, _headsetPoller.GetStatus().Length);

            var bindings =
                from index in headsetIndices
                from binding in Bindings
                select CreateHeadsetChannelBinding(index, binding, bindingsLookup);
            return bindings;
        }

        private HeadsetChannelBinding CreateHeadsetChannelBinding(
            int index, 
            ChannelBinding binding, 
            Dictionary<(int Index, string ChannelBindingName), (ArctisChannel BoundChannel, bool IsEnabled)> bindingsLookup)
        {
            bindingsLookup.TryGetValue((index, binding.BindingName), out var channel);
            HeadsetChannelBinding result = new HeadsetChannelBinding(index, channel.BoundChannel, binding.BindingName)
            {
                IsEnabled = channel.IsEnabled
            };
            result.PropertyChanged += OnHeadsetChannelBindingPropertyChanged;
            return result;


            void OnHeadsetChannelBindingPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (sender == null) throw new ArgumentNullException(nameof(sender));

                var source = (HeadsetChannelBinding)sender;
                var headset = binding.BoundHeadsets.First(x => x.Index == index);

                headset.IsEnabled = source.IsEnabled;
                headset.BoundChannel = source.BoundChannel;
            }
        }

        public void Dispose()
        {
            _headsetPoller.Unbind();

            foreach (var binding in Bindings)
            {
                binding.OptionsChanged -= BindingPropertyChanged;
                binding.BindingNameChanged -= BindingNameChanged;
                binding.Dispose();
            }

            Bindings.Clear();
        }

        private ChannelBinding CreateBinding(ChannelBindingOptions options)
        {
            var binding = new ChannelBinding(_headsetPoller, _voiceMeeterClient, options);
            binding.OptionsChanged += BindingPropertyChanged;
            binding.BindingNameChanged += BindingNameChanged;
            return binding;
        }

        private ObservableCollection<ChannelBinding> CreateBindingsCollection()
        {
            ArctisVoiceMeeterPresets options = _presets.Value;
            var bindings = options.Select(CreateBinding);

            return new ObservableCollection<ChannelBinding>(bindings);
        }

        private void BindingPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var options = sender as ChannelBindingOptions ?? throw new ArgumentException("ChannelBindingOptions type expected", nameof(sender));
            ChangeBinding(options);
        }

        private void BindingNameChanged(object? sender, PropertyValueChangedEventArgs e)
        {
            RenameBinding(e.OldValue.ToString(), e.NewValue.ToString());
        }
    }
}
