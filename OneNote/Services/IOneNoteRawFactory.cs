namespace OneNoteApi.Services
{
    public interface IOneNoteRawFactory : IOneNoteService
    {
        IOneNoteRaw GetNew();
    }

    public class OneNoteRawFactory : IOneNoteRawFactory
    {
        public IOneNoteRaw GetNew()
        {
            return new OneNoteRaw();
        }
    }
}