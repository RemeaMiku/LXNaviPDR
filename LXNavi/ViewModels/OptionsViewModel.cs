using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LXNavi;

public class OptionsViewModel : ObservableObject
{
    #region Public Properties

    public LocalizationService LocalizationService => LocalizationService.Instance;
    public string LanguageCode
    {
        get => StringResource.Culture.TwoLetterISOLanguageName;
        set => LocalizationService.SetCulture(new(value));
    }

    #endregion Public Properties
}
