using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GadzzaaTB.Windows
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    [SuppressMessage("ReSharper", "RedundantCheckBeforeAssignment")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed partial class DebugOsu : INotifyPropertyChanged
    {
        private string _dl = "_dl";

        private string _mapArtistTitle = "_mapArtistTitle";

        private string _mapDiff = "_mapDiff";

        private string _mods = "_mods";

        private double _mStars = 5.00;

        public DebugOsu()
        {
            InitializeComponent();
            DataContext = this;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public double mStars
        {
            get => _mStars;
            set
            {
                if (_mStars != value) _mStars = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string mapArtistTitle
        {
            get => _mapArtistTitle;
            set
            {
                if (_mapArtistTitle != value) _mapArtistTitle = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string mapDiff
        {
            get => _mapDiff;
            set
            {
                if (_mapDiff != value) _mapDiff = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string mods
        {
            get => _mods;
            set
            {
                if (_mods != value) _mods = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string dl
        {
            get => _dl;
            set
            {
                if (_dl != value) _dl = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}