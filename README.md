# <img src="src/MvvmMicro/icon.png" alt="logo" width="32" height="32" /> MvvmMicro
<a href="https://www.nuget.org/packages/MvvmMicro" target="_blank"><img alt="Nuget" src="https://img.shields.io/nuget/v/MvvmMicro" /></a>

A clean and lightweight MVVM framework for WPF, UWP and .NET Standard 2.0 inspired by MVVM Light Toolkit.

## Goals
- No third party dependencies, such as `CommonServiceLocator` or `System.Windows.Interactivity`.
- Avoid [feature kreep](https://en.wikipedia.org/wiki/Feature_creep) – only core MVVM types and services are included, such as `RelayCommand`, `ObservableObject` and a simple `IMessenger`.

## Supported platforms
- .NET Framework 4.5 & .NET 5.0 (WPF)
- Universal Windows Platform (UWP)
- .NET Standard 2.0

## Installation
The primary way to use MvvmMicro is by adding the [nuget](https://www.nuget.org/packages/MvvmMicro) package to your project:
```
Install-Package MvvmMicro
```

## Classes and interfaces
Here's an overview of classes and interfaces exposed by the library:
![Class diagram](src/MvvmMicro/Diagrams/ClassDiagram.png)
