using Content.Client.Audio;
using Content.Shared.CCVar;
using Robust.Client.Audio;
using Content.Shared.Corvax.CCCVars;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared;
using Robust.Shared.Configuration;
using Range = Robust.Client.UserInterface.Controls.Range;
using System;

namespace Content.Client.Options.UI.Tabs
{
    [GenerateTypedNameReferences]
    public sealed partial class AudioTab : Control
    {
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        private readonly IAudioManager _audio;

        public AudioTab()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            _audio = IoCManager.Resolve<IAudioManager>();
            LobbyMusicCheckBox.Pressed = _cfg.GetCVar(CCVars.LobbyMusicEnabled);
            RestartSoundsCheckBox.Pressed = _cfg.GetCVar(CCVars.RestartSoundsEnabled);
            EventMusicCheckBox.Pressed = _cfg.GetCVar(CCVars.EventMusicEnabled);
            AdminSoundsCheckBox.Pressed = _cfg.GetCVar(CCVars.AdminSoundsEnabled);

            ApplyButton.OnPressed += OnApplyButtonPressed;
            ResetButton.OnPressed += OnResetButtonPressed;
            MasterVolumeSlider.OnValueChanged += OnMasterVolumeSliderChanged;
            MidiVolumeSlider.OnValueChanged += OnMidiVolumeSliderChanged;
            AmbientMusicVolumeSlider.OnValueChanged += OnAmbientMusicVolumeSliderChanged;
            AmbienceVolumeSlider.OnValueChanged += OnAmbienceVolumeSliderChanged;
            AmbienceSoundsSlider.OnValueChanged += OnAmbienceSoundsSliderChanged;
            LobbyVolumeSlider.OnValueChanged += OnLobbyVolumeSliderChanged;
            InterfaceVolumeSlider.OnValueChanged += OnInterfaceVolumeSliderChanged;
            TtsVolumeSlider.OnValueChanged += OnTtsVolumeSliderChanged; // Corvax-TTS
            LobbyMusicCheckBox.OnToggled += OnLobbyMusicCheckToggled;
            RestartSoundsCheckBox.OnToggled += OnRestartSoundsCheckToggled;
            EventMusicCheckBox.OnToggled += OnEventMusicCheckToggled;
            AdminSoundsCheckBox.OnToggled += OnAdminSoundsCheckToggled;

            AmbienceSoundsSlider.MinValue = _cfg.GetCVar(CCVars.MinMaxAmbientSourcesConfigured);
            AmbienceSoundsSlider.MaxValue = _cfg.GetCVar(CCVars.MaxMaxAmbientSourcesConfigured);

            Reset();
        }

        protected override void Dispose(bool disposing)
        {
            ApplyButton.OnPressed -= OnApplyButtonPressed;
            ResetButton.OnPressed -= OnResetButtonPressed;
            MasterVolumeSlider.OnValueChanged -= OnMasterVolumeSliderChanged;
            MidiVolumeSlider.OnValueChanged -= OnMidiVolumeSliderChanged;
            AmbientMusicVolumeSlider.OnValueChanged -= OnAmbientMusicVolumeSliderChanged;
            AmbienceVolumeSlider.OnValueChanged -= OnAmbienceVolumeSliderChanged;
            LobbyVolumeSlider.OnValueChanged -= OnLobbyVolumeSliderChanged;
            TtsVolumeSlider.OnValueChanged -= OnTtsVolumeSliderChanged; // Corvax-TTS
            InterfaceVolumeSlider.OnValueChanged -= OnInterfaceVolumeSliderChanged;
            base.Dispose(disposing);
        }

        private void OnLobbyVolumeSliderChanged(Range obj)
        {
            UpdateChanges();
        }

        private void OnInterfaceVolumeSliderChanged(Range obj)
        {
            UpdateChanges();
        }

        private void OnAmbientMusicVolumeSliderChanged(Range obj)
        {
            UpdateChanges();
        }

        private void OnAmbienceVolumeSliderChanged(Range obj)
        {
            UpdateChanges();
        }

        private void OnAmbienceSoundsSliderChanged(Range obj)
        {
            UpdateChanges();
        }

        private void OnMasterVolumeSliderChanged(Range range)
        {
            _audio.SetMasterGain(MasterVolumeSlider.Value / 100f * ContentAudioSystem.MasterVolumeMultiplier);
            UpdateChanges();
        }

        private void OnMidiVolumeSliderChanged(Range range)
        {
            UpdateChanges();
        }

        // Corvax-TTS-Start
        private void OnTtsVolumeSliderChanged(Range obj)
        {
            UpdateChanges();
        }
        // Corvax-TTS-End

        private void OnLobbyMusicCheckToggled(BaseButton.ButtonEventArgs args)
        {
            UpdateChanges();
        }
        private void OnRestartSoundsCheckToggled(BaseButton.ButtonEventArgs args)
        {
            UpdateChanges();
        }
        private void OnEventMusicCheckToggled(BaseButton.ButtonEventArgs args)
        {
            UpdateChanges();
        }

        private void OnAdminSoundsCheckToggled(BaseButton.ButtonEventArgs args)
        {
            UpdateChanges();
        }

        private void OnApplyButtonPressed(BaseButton.ButtonEventArgs args)
        {
            _cfg.SetCVar(CVars.AudioMasterVolume, MasterVolumeSlider.Value / 100f * ContentAudioSystem.MasterVolumeMultiplier);
            // Want the CVar updated values to have the multiplier applied
            // For the UI we just display 0-100 still elsewhere
            _cfg.SetCVar(CVars.MidiVolume, MidiVolumeSlider.Value / 100f * ContentAudioSystem.MidiVolumeMultiplier);
            _cfg.SetCVar(CCVars.AmbienceVolume, AmbienceVolumeSlider.Value / 100f * ContentAudioSystem.AmbienceMultiplier);
            _cfg.SetCVar(CCVars.AmbientMusicVolume, AmbientMusicVolumeSlider.Value / 100f * ContentAudioSystem.AmbientMusicMultiplier);
            _cfg.SetCVar(CCVars.LobbyMusicVolume, LobbyVolumeSlider.Value / 100f * ContentAudioSystem.LobbyMultiplier);
            _cfg.SetCVar(CCVars.InterfaceVolume, InterfaceVolumeSlider.Value / 100f * ContentAudioSystem.InterfaceMultiplier);
            _cfg.SetCVar(CCCVars.TTSVolume, LV100ToDB(TtsVolumeSlider.Value)); // Corvax-TTS
            _cfg.SetCVar(CCVars.MaxAmbientSources, (int)AmbienceSoundsSlider.Value);

            _cfg.SetCVar(CCVars.LobbyMusicEnabled, LobbyMusicCheckBox.Pressed);
            _cfg.SetCVar(CCVars.RestartSoundsEnabled, RestartSoundsCheckBox.Pressed);
            _cfg.SetCVar(CCVars.EventMusicEnabled, EventMusicCheckBox.Pressed);
            _cfg.SetCVar(CCVars.AdminSoundsEnabled, AdminSoundsCheckBox.Pressed);
            _cfg.SaveToFile();
            UpdateChanges();
        }

        private void OnResetButtonPressed(BaseButton.ButtonEventArgs args)
        {
            Reset();
        }

        private void Reset()
        {
            MasterVolumeSlider.Value = _cfg.GetCVar(CVars.AudioMasterVolume) * 100f / ContentAudioSystem.MasterVolumeMultiplier;
            MidiVolumeSlider.Value = _cfg.GetCVar(CVars.MidiVolume) * 100f / ContentAudioSystem.MidiVolumeMultiplier;
            AmbienceVolumeSlider.Value = _cfg.GetCVar(CCVars.AmbienceVolume) * 100f / ContentAudioSystem.AmbienceMultiplier;
            AmbientMusicVolumeSlider.Value = _cfg.GetCVar(CCVars.AmbientMusicVolume) * 100f / ContentAudioSystem.AmbientMusicMultiplier;
            LobbyVolumeSlider.Value = _cfg.GetCVar(CCVars.LobbyMusicVolume) * 100f / ContentAudioSystem.LobbyMultiplier;
            InterfaceVolumeSlider.Value = _cfg.GetCVar(CCVars.InterfaceVolume) * 100f / ContentAudioSystem.InterfaceMultiplier;
            TtsVolumeSlider.Value = DBToLV100(_cfg.GetCVar(CCCVars.TTSVolume)); // Corvax-TTS
            AmbienceSoundsSlider.Value = _cfg.GetCVar(CCVars.MaxAmbientSources);

            LobbyMusicCheckBox.Pressed = _cfg.GetCVar(CCVars.LobbyMusicEnabled);
            RestartSoundsCheckBox.Pressed = _cfg.GetCVar(CCVars.RestartSoundsEnabled);
            EventMusicCheckBox.Pressed = _cfg.GetCVar(CCVars.EventMusicEnabled);
            AdminSoundsCheckBox.Pressed = _cfg.GetCVar(CCVars.AdminSoundsEnabled);
            UpdateChanges();
        }

        private void UpdateChanges()
        {
            // y'all need jesus.
            var isMasterVolumeSame =
                Math.Abs(MasterVolumeSlider.Value - _cfg.GetCVar(CVars.AudioMasterVolume) * 100f / ContentAudioSystem.MasterVolumeMultiplier) < 0.01f;
            var isMidiVolumeSame =
                Math.Abs(MidiVolumeSlider.Value - _cfg.GetCVar(CVars.MidiVolume) * 100f / ContentAudioSystem.MidiVolumeMultiplier) < 0.01f;
            var isAmbientVolumeSame =
                Math.Abs(AmbienceVolumeSlider.Value - _cfg.GetCVar(CCVars.AmbienceVolume) * 100f / ContentAudioSystem.AmbienceMultiplier) < 0.01f;
            var isAmbientMusicVolumeSame =
                Math.Abs(AmbientMusicVolumeSlider.Value - _cfg.GetCVar(CCVars.AmbientMusicVolume) * 100f / ContentAudioSystem.AmbientMusicMultiplier) < 0.01f;
            var isLobbyVolumeSame =
                Math.Abs(LobbyVolumeSlider.Value - _cfg.GetCVar(CCVars.LobbyMusicVolume) * 100f / ContentAudioSystem.LobbyMultiplier) < 0.01f;
            var isInterfaceVolumeSame =
                Math.Abs(InterfaceVolumeSlider.Value - _cfg.GetCVar(CCVars.InterfaceVolume) * 100f / ContentAudioSystem.InterfaceMultiplier) < 0.01f;
            var isTtsVolumeSame =
                Math.Abs(TtsVolumeSlider.Value - DBToLV100(_cfg.GetCVar(CCCVars.TTSVolume))) < 0.01f; // Corvax-TTS
            var isAmbientSoundsSame = (int)AmbienceSoundsSlider.Value == _cfg.GetCVar(CCVars.MaxAmbientSources);
            var isLobbySame = LobbyMusicCheckBox.Pressed == _cfg.GetCVar(CCVars.LobbyMusicEnabled);
            var isRestartSoundsSame = RestartSoundsCheckBox.Pressed == _cfg.GetCVar(CCVars.RestartSoundsEnabled);
            var isEventSame = EventMusicCheckBox.Pressed == _cfg.GetCVar(CCVars.EventMusicEnabled);
            var isAdminSoundsSame = AdminSoundsCheckBox.Pressed == _cfg.GetCVar(CCVars.AdminSoundsEnabled);
            var isEverythingSame = isMasterVolumeSame && isMidiVolumeSame && isAmbientVolumeSame && isAmbientMusicVolumeSame && isAmbientSoundsSame && isLobbySame && isRestartSoundsSame && isEventSame
                                   && isAdminSoundsSame && isLobbyVolumeSame && isInterfaceVolumeSame;
            isEverythingSame = isEverythingSame && isTtsVolumeSame; // Corvax-TTS
            ApplyButton.Disabled = isEverythingSame;
            ResetButton.Disabled = isEverythingSame;
            MasterVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", MasterVolumeSlider.Value / 100));
            MidiVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", MidiVolumeSlider.Value / 100));
            AmbientMusicVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", AmbientMusicVolumeSlider.Value / 100));
            AmbienceVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", AmbienceVolumeSlider.Value / 100));
            LobbyVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", LobbyVolumeSlider.Value / 100));
            TtsVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", TtsVolumeSlider.Value / 100)); // Corvax-TTS
            InterfaceVolumeLabel.Text =
                Loc.GetString("ui-options-volume-percent", ("volume", InterfaceVolumeSlider.Value / 100));
            AmbienceSoundsLabel.Text = ((int)AmbienceSoundsSlider.Value).ToString();
        }
    }
}
