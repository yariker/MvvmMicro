# <img src="src/MvvmMicro/icon.png" alt="logo" width="32" height="32" /> MvvmMicro
<a href="https://www.nuget.org/packages/MvvmMicro" target="_blank"><img alt="Nuget" src="https://img.shields.io/nuget/v/MvvmMicro" /></a>

A lightweight MVVM framework for .NET inspired by [MVVM Light Toolkit](https://github.com/lbugnion/mvvmlight).

## Goals

- No third party dependencies, such as `CommonServiceLocator` or `System.Windows.Interactivity`.
- No feature kreep – only core MVVM types and services are included, such as `RelayCommand`, `AsyncRelayCommand`,
`ObservableObject`, and a simple `IMessenger`.

## Supported platforms

- .NET Framework 4.6.2 & .NET 5.0 (WPF)
- Universal Windows Platform (UWP)
- .NET Standard 2.0 (Xamarin.Forms, Avalonia, MAUI, etc.)

## Installation

The primary way to use MvvmMicro is by adding the [nuget](https://www.nuget.org/packages/MvvmMicro) package to your project:
```
Install-Package MvvmMicro
```

## Overview

| Type | Description |
| ----- | ----------- |
| `ObservableObject` | The base class for objects that support property change notification. |
| `ViewModelBase` | The base class for view models with the `Messenger` and `IsInDesignMode` properties. |
| `Messenger` | A service for sending and receiving messages, typically between view models and views. |
| `RelayCommand`,<br/>`RelayCommand<T>` | An `ICommand` implementation based on a synchronous delegate for `Execute` and `CanExecute`. |
| `AsyncRelayCommand`,<br/>`AsyncRelayCommand<T>` | An `ICommand` implementation based on an asynchronous delegate for `Execute` with cancellation support. |

Also check out the [class diagram](src/MvvmMicro/Diagrams/ClassDiagram.png).

## License

Code licensed under the [MIT License](LICENSE).
