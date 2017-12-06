using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleSystem.Common.DialogInterface
{
    public interface IDialogProvider
    {
        bool AskQuestion(string title, string text);
        bool? AskCancellableQuestion(string title, string text);

        void ShowAlert(string title, string text);
        bool ShowCancellableAlert(string title, string text);

        void ShowInformation(string title, string text);
        bool ShowCancellableInformation(string title, string text);

        void ShowError(string title, string text);
        bool ShowCancellableError(string title, string text);
    }
}
