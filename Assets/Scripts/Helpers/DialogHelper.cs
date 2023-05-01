using System.Threading.Tasks;
using System.Linq;

public static class DialogHelper
{
    public static async Task ShowDialog(DialogSO dialogSO, bool startConvo = true, bool endConvo = true)
    {
        Dialog dialog = Dialog.Instance;
        dialog.SetDialogWithSO(dialogSO);
        if (startConvo)
        {
            await dialog.FadeIn();
        }

        dialog.StartDialog();
        await dialog.WaitForDialogToFinish();

        if (endConvo)
        {
            await dialog.FadeOut();
        }
    }

    public static async Task ShowConversation(ConversationSO conversationSO)
    {
        foreach (var dialogue in conversationSO.Dialogues)
        {
            await ShowDialog(dialogue, conversationSO.Dialogues.First() == dialogue, conversationSO.Dialogues.Last() == dialogue);
        }
    }
}
