using GadzzaaTB.Classes;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GadzzaaTB.Windows;

[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
[SuppressMessage("ReSharper", "RedundantCheckBeforeAssignment")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed partial class DebugOsu : INotifyPropertyChanged
{
    private string _dl;

    private string _mapInfo;

    private int _mods;
    private string _modsText;

    private double _mStars;


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
    public string mapInfo
    {
        get => _mapInfo;
        set
        {
            if (_mapInfo != value) _mapInfo = value;
            OnPropertyChanged();
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global

    public string modsText
    {
        get => _modsText;
        set
        {
            if (_modsText != value) _modsText = value;
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