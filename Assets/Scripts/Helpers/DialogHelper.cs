using System.Threading.Tasks;

public static class DialogHelper
{
    public static async Task ShowDialog(DialogSO dialogSO)
    {
        Dialog dialog = Dialog.Instance;
        dialog.SetDialogWithSO(dialogSO);
        await dialog.FadeIn();
        dialog.StartDialog();
        await dialog.WaitForDialogToFinish();
        await dialog.FadeOut();
    }
}
