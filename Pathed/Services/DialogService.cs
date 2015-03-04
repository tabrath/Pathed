using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pathed.Services
{
    [Export(typeof(IDialogService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DialogService : IDialogService
    {
        [ImportingConstructor]
        public DialogService() { }

        public string ShowBrowseFolderDialog()
        {
            string folder = String.Empty;
            using (var dialog = new CommonOpenFileDialog() { EnsurePathExists = true, IsFolderPicker = true })
            {
                if (dialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
                {
                    folder = dialog.FileName;
                }
            }

            return folder;
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void OpenFolder(string path)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            Process.Start(path);
        }

        public DialogResult ShowDialog(string caption, string message, DialogButton button)
        {
            return Convert(MessageBox.Show(message, caption, Convert(button)));
        }

        private static MessageBoxButton Convert(DialogButton button)
        {
            switch (button)
            {
                case DialogButton.YesNo:
                    return MessageBoxButton.YesNo;

                case DialogButton.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;

                case DialogButton.Ok:
                    return MessageBoxButton.OK;

                case DialogButton.OkCancel:
                    return MessageBoxButton.OKCancel;

                default:
                    throw new ArgumentException("button");
            }
        }

        private static DialogResult Convert(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return DialogResult.Yes;

                case MessageBoxResult.OK:
                    return DialogResult.Ok;

                case MessageBoxResult.Cancel:
                    return DialogResult.Cancel;

                case MessageBoxResult.No:
                    return DialogResult.No;

                case MessageBoxResult.None:
                    return DialogResult.None;

                default:
                    throw new ArgumentException("result");
            }
        }
    }
}
