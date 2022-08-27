// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using Xunit;

namespace MvvmMicro.Test;

public class ObservableObjectTest : ObservableObject
{
    [Fact]
    public void OnPropertyChanged_RaisesPropertyChanged()
    {
        const string propertyName = "TestProperty";
        Assert.PropertyChanged(this, propertyName, () => OnPropertyChanged(propertyName));
    }

    [Fact]
    public void Set_VerifiesArguments()
    {
        string testField = null;
        Assert.Throws<ArgumentNullException>("propertyName", () => Set(ref testField, null, null));
    }

    [Fact]
    public void Set_RaisesPropertyChanged()
    {
        const string propertyName = "TestProperty";
        const string testValue = "TestValue";
        string testField = null;
        Assert.PropertyChanged(this, propertyName, () => Set(ref testField, testValue, propertyName));
    }

    [Fact]
    public void Set_ChangesFieldValue()
    {
        const string propertyName = "TestProperty";
        const string testValue = "TestValue";
        string testField = null;

        Assert.True(Set(ref testField, testValue, propertyName));
        Assert.Equal(testValue, testField);

        Assert.False(Set(ref testField, testValue, propertyName));
        Assert.Equal(testValue, testField);
    }
}
