using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using VibrometerHostApp.ViewModels;

namespace VibrometerHostApp
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if(data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data) => data is ViewModelBase;
    }
}