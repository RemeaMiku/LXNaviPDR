using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LXNavi;

public partial class AppShellViewModel : ObservableObject
{
    #region Public Properties

    public LocalizationService LocalizationService => LocalizationService.Instance;

    #endregion Public Properties

}
