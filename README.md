# <img src="src/MvvmMicro/icon.png" alt="logo" width="32" height="32" /> MvvmMicro
<a href="https://www.nuget.org/packages/MvvmMicro" target="_blank"><img alt="Nuget" src="https://img.shields.io/nuget/v/MvvmMicro" /></a>
<a href="https://www.nuget.org/packages/MvvmMicro/absoluteLatest" target="_blank"><img alt="Nuget (Prerelease)" src="https://img.shields.io/nuget/vpre/MvvmMicro" /></a>

A clean and lightweight MVVM framework for WPF, UWP and Xamarin.Forms inspired my MVVM Light Toolkit.

## Goals
- No third party dependencies, such as `CommonServiceLocator` and `System.Windows.Interactivity`.
- Avoid [feature kreep](https://en.wikipedia.org/wiki/Feature_creep) – only core MVVM types and services are included, such as `RelayCommand`, `ObservableObject` and a simple `IMessenger`.
- Maintain clean, high quality code base.

## Supported platforms
- .NET Framework 4.5 (WPF)
- .NET Standard 2.0 (UWP, Xamarin.Forms)

## Downloads
The primary way to use MvvmMicro is by adding the [nuget](https://www.nuget.org/packages/MvvmMicro) package to your project.
