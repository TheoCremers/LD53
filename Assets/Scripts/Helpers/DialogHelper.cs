using System.Threading.Tasks;
using System.Linq;

public static class DialogHelper
{
    public static async Task<int> ShowDialog(DialogSO dialogSO, bool startConvo = true, bool endConvo = true)
    {
        Dialog dialog = Dialog.Instance;
        dialog.SetDialogWithSO(dialogSO);
        if (startConvo)
        {
            await dialog.FadeIn();
        }

        dialog.StartDialog();
        int result = await dialog.WaitForDialogToFinish();

        if (endConvo)
        {
            await dialog.FadeOut();
        }

        return result;
    }

    public static async Task<int> ShowConversation(ConversationSO conversationSO)
    {
        int result = 0;        
        foreach (var dialogue in conversationSO.Dialogues)
        {
            result = await ShowDialog(dialogue, conversationSO.Dialogues.First() == dialogue, conversationSO.Dialogues.Last() == dialogue);
        }
        return result;
    }
}
