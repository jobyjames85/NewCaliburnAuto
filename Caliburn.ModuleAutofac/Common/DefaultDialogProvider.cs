using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModuleSystem.Common.DialogInterface;
using ModuleSystem.ModuleAutofac.Configurations;

namespace ModuleSystem.ModuleAutofac.Common
{
    [SingleInstance]
    public class DefaultDialogProvider : IDialogProvider
    {
        public DefaultDialogProvider()
        {

        }

        public bool AskQuestion(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public bool? AskCancellableQuestion(string title, string text)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                return true;
            else if (result == MessageBoxResult.No)
                return false;
            else
                return null;
        }

        public void ShowAlert(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowCancellableAlert(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
        }

        public void ShowInformation(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowCancellableInformation(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK;
        }

        public void ShowError(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowCancellableError(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK;
        }
    }
}
