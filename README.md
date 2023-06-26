# LXNaviPDR
## 项目简介
这是2022-2023学年度第二学期武汉大学测绘学院导航工程专业选修课程《位置服务与实践》（顺带一提，这课是真的水，陈*老师有几次自己都不来，就纯让学生自己写代码，基本什么都不教，所以我干脆自学微软新出的框架了。正好也不想学Java，又要换IDE，配置麻烦死了，C# .NET+Visual Studio多牛，啥都能做，不比学一堆大杂烩好？）的.NET MAUI项目，包含名为[LXNavi.Core](https://github.com/RemeaMiku/LXNaviPDR/tree/master/LXNavi.Core)库和[LXNavi](https://github.com/RemeaMiku/LXNaviPDR/tree/master/LXNavi)的MAUI程序，实现了实时的行人航迹推算（PDR）算法和数据、轨迹可视化，均依赖于另一个自编工具库 [NaviSharp](https://github.com/RemeaMiku/NaviSharp)。其中MAUI应用使用了MVVM社区工具包、MAUI社区工具包、微软调试日志扩展、微软MAUI地图控件包、Sycnfusion MAUI等Nuget包。

在写这个项目的时候，MAUI正式版还没出多久，网上的教程非常少，尤其是简中网。好在微软的官方文档写的不错，很多直接复制就能用。再配上Youtube上[James Montemagno的MAUI入门教学](https://www.youtube.com/watch?v=DuNLR_NJv8U)，总算是顺利完成了。这是我第一次MVVM架构和IoC设计实践（如果你不知道什么是MVVM和IoC，请分别查看[模型-视图-视图模型](https://learn.microsoft.com/zh-cn/dotnet/architecture/maui/mvvm)和[体系结构原则-依赖关系反转](https://learn.microsoft.com/zh-cn/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion)），也是第一次用XAML标记来写UI布局和样式，之前只用过Winform拖拽控件，因此花了相当长的时间。

MAUI全称是Muilti-platform APP UI，正如其名，这是.NET的一个跨平台框架，是在Xamarin的基础上再进行了一次封装，使得一份代码可以在不进行任何修改的情况下，同时在Windows、MacOS、iOS、Android和Tizen（哎就是不支持Linux）上运行（详细情况可以点击这里[MAUI官网](https://dotnet.microsoft.com/zh-cn/apps/maui)），并且还是原生控件渲染。不过正因为是一份的公共代码（不同于Xamarin的多项目，MAUI只需要一个项目就能到处跑），MAUI上能访问的平台API应该是各个平台的交集，也就是说，如果有个平台不支持某个功能，那么MAUI里就不能访问（或者可以写平台特定代码，不过这样好像就失去MAUI的意义了）。

![maui-overview.png](https://learn.microsoft.com/zh-cn/dotnet/maui/media/what-is-maui/maui-overview.png)

仅对于这个项目而言，目前只完成了Android端的运行测试，Windows上不知为何有些地方渲染有些问题，可能是MAUI的BUG。至于iOS还需要一些额外的配置，也还没有实机测试条件（太麻烦了，没时间做），所以实际上只能在Android上完整运行。
## LXNavi.Core
包含PDR的观测值数据结构 ` DeadReckoningMeasurement ` 和简单的PDR算法类 `DeadReckoning ` ，供MAUI应用调用。
## LXNavi
此项目不实现任何导航算法，只对LXNavi.Core中的方法进行进一步封装以方便调用。

CustomMap包含用于自定义地图控件包中的图钉 `Pin` 的样式的代码文件。主要是不知道为啥Google Map自带的位置显示只能用它自带的定位的位置，不能自己赋值传给地图，只好曲线救国用自定义图钉来显示了。这里面的代码基本是复制的[Customize map pins in .NET MAUI](https://vladislavantonyuk.github.io/articles/Customize-map-pins-in-.NET-MAUI/)里面的。

Models对应于MVVM中的模型层。` Sensor ` 就是传感器类，MAUI中获取传感器数据都是基本通过静态方法获取的，为方便管理，还是自己写了一个类；` SensorData ` 用以将各个传感器数据的集成在一起，并统一时间戳（也不知道为啥MAUI没有提供获取传感器数据时间戳的接口，这次程序中的时间戳都是在触发事件时手动获取的），方便PDR算法处理。

Platforms是MAUI自带的文件夹，用于管理各平台的特定文件，如资源文件、配置文件等。

Resources是MAUI自带的文件夹，包含各种公共资源文件，如应用图标、字体、启动屏幕和XAML样式等。其中的Localization是我自己加的，用于实现MAUI应用的多语言（简体中文/English/日本語）的资源文件，各个StringResource文件的后缀中的国家语言代码及对应了各个语言，参考[国家、地区和语言代码](https://support.microsoft.com/zh-cn/topic/%E5%9B%BD%E5%AE%B6-%E5%9C%B0%E5%8C%BA%E5%92%8C%E8%AF%AD%E8%A8%80%E4%BB%A3%E7%A0%81-add36afe-804a-44f1-ae68-cfb9c9b72f8b)。

Services包含MAUI应用提供的所需服务。`DeadReckoningService` 提供PDR解算服务，对LXNavi.Core中的方法进行进一步封装；`LocalizationService` 提供运行时本地化服务；`SensorsService` 提供PDR所需的传感器及其数据服务。

ViewModels对应于MVVM中的视图模型层。

Views对应于MVVM中的视图层，存放页面的XAML标记文件和其代码隐藏。共有4个页面：
- 主页面 `MainPage`：监测加速度计、陀螺仪和磁强计
- 传感器详情页 `SensorDetailPage`：绘制传感器三轴实时折线图
- 轨迹展示页 `TrackDisplayPage`：在谷歌地图底图上绘制实时行人轨迹。可以设置对轨迹进行一些设置
- 选项页 `OptionsPage`：进行本地化语言设置

`Microsoft.Maui.Controls.Application` 类是 Microsoft.Maui.Controls 应用程序的核心。 它设置应用程序的根页，跨字典中 `Properties` 应用程序的调用保留基元类型数据，并提供事件来响应模式视图的推送和弹出。 Visual Studio 在新的 Microsoft.Maui.Controls 解决方案的相应项目中为开发人员创建此类。当开发人员创建新的 Microsoft.Maui.Controls 解决方案时，Visual Studio for Mac和 Visual Studio 都为应用程序创建 XAML 和代码隐藏文件。

`App` 即当前MAUI应用类，继承于 `Application`。在App.xaml中可注册用到的资源字典（Resource Dictionary），以便于在其他XAML标记文件中引用，达到样式与结构分离的效果。

`Microsoft.Maui.Controls.Shell` 类继承于 `Microsoft.Maui.Controls.Page`，因此本质就是一个页面，承载整个应用，给应用提供一个外壳以实现大多数应用程序需要的基本 UI 功能，让开发者能够专注于处理应用程序的核心工作负载。例如页面间导航等。这一点不同于WPF。

`AppShell`即当前的MAUI应用外壳类，继承于`Shell`。可在AppShell.xaml中定义页面导航方式以及布局样式。本次实习用的是比较常见的飞出式菜单(Flyout Menu)。
## 界面与功能展示
### 传感器监测
![传感器监测.gif](GIFs/传感器监测.gif)
- 实时显示加速度计、陀螺仪和磁强计三轴输出
- 实时绘制传感器三轴输出随时间变化曲线
- 传感器数据记录和分享
### 实时轨迹绘制
![实时轨迹绘制.gif](GIFs/实时轨迹绘制.gif)
- 实时位置显示
- 轨迹颜色设置
- 行人步长调节
- 视图居中开关