using Microsoft.Maui.Controls.Maps;
#pragma warning disable CS8632
namespace LXNavi;

public class CustomPin : Pin
{
    #region Public Fields

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CustomPin));

    #endregion Public Fields

    #region Public Properties

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    #endregion Public Properties
}
