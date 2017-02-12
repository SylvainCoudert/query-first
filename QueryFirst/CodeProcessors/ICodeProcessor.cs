namespace QueryFirst.CodeProcessors
{
    public interface ICodeProcessor
    {
        string GetExtension();

        string MakeAddAParameter();

        string GetResultClassRegex();

        string GetNamespaceRegex();
    }
}
