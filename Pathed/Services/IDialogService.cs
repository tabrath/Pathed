namespace Pathed.Services
{
    public interface IDialogService
    {
        string ShowBrowseFolderDialog();
        void ShowErrorMessage(string message);
        void OpenFolder(string path);
        DialogResult ShowDialog(string caption, string message, DialogButton buttons);
    }

    public enum DialogButton
    {
        YesNo,
        YesNoCancel,
        Ok,
        OkCancel
    }

    public enum DialogResult
    {
        Yes,
        No,
        Ok,
        Cancel,
        None
    }
}
