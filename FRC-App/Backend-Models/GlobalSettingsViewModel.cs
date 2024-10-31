using System.ComponentModel;
using Microsoft.Maui.Graphics; // For Color
using Microsoft.Maui.Storage;  // For Preferences

namespace FRC_App
{
    public class GlobalFontSettingsViewModel : INotifyPropertyChanged
    {
        private double _fontSize;
        private Color _fontColor;
        private string _fontType;

        public event PropertyChangedEventHandler PropertyChanged;

        // Font Size Property
        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                    SaveFontSizePreference();
                }
            }
        }

        // Font Color Property
        public Color FontColor
        {
            get => _fontColor;
            set
            {
                if (_fontColor != value)
                {
                    _fontColor = value;
                    OnPropertyChanged(nameof(FontColor));
                    SaveFontColorPreference();
                }
            }
        }

        // Font Type Property
        public string FontType
        {
            get => _fontType;
            set
            {
                if (_fontType != value)
                {
                    _fontType = value;
                    OnPropertyChanged(nameof(FontType));
                    SaveFontTypePreference();
                }
            }
        }

        public GlobalFontSettingsViewModel()
        {
            // Load preferences or set default values
            _fontSize = Preferences.Get("UserFontSize", 18);
            _fontColor = Color.FromArgb(Preferences.Get("UserFontColor", "#000000")); // Default to black
            _fontType = Preferences.Get("UserFontType", "OpenSansRegular"); // Default to Arial or any other font family
        }

        // Save Preferences
        private void SaveFontSizePreference() => Preferences.Set("UserFontSize", _fontSize);
        private void SaveFontColorPreference() => Preferences.Set("UserFontColor", _fontColor.ToArgbHex());
        private void SaveFontTypePreference() => Preferences.Set("UserFontType", _fontType);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}