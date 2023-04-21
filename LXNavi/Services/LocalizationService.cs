using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LXNavi;

public class LocalizationService : INotifyPropertyChanged
{
    public LocalizationService()
    {
        StringResource.Culture = CultureInfo.CurrentCulture;
    }

    public static LocalizationService Instance { get; } = new();

    public object this[string resourceKey]
        => StringResource.ResourceManager.GetObject(resourceKey, StringResource.Culture) ?? Array.Empty<byte>();

    public event PropertyChangedEventHandler PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        StringResource.Culture = culture;
        PropertyChanged?.Invoke(this, new(null));
    }
}