public class PhoneNotesPage : PhonePage
{
    public override void Open()
    {
        base.Open();

        foreach (NoteRow row in GetComponentsInChildren<NoteRow>(true))
        {
            row.Refresh();
        }
    }
}
