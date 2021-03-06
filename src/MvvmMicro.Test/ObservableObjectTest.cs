using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MvvmMicro.Test
{
    public class ObservableObjectTest : ObservableObject
    {
        [Fact]
        public void OnPropertyChanged_Should_Raise_PropertyChanged()
        {
            const string propertyName = "TestProperty";
            Assert.PropertyChanged(this, propertyName, () => OnPropertyChanged(propertyName));
        }

        [Fact]
        public void Set_Should_Verify_Arguments()
        {
            string testField = null;
            Assert.Throws<ArgumentNullException>("propertyName", () => Set(ref testField, null, null));
        }

        [Fact]
        public void Set_Should_Raise_PropertyChanged()
        {
            const string propertyName = "TestProperty";
            const string testValue = "TestValue";
            string testField = null;
            Assert.PropertyChanged(this, propertyName, () => Set(ref testField, testValue, propertyName));
        }

        [Fact]
        public void Set_Should_Change_Field_Value()
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
}
