using EnvDTE;

namespace QueryFirst.CodeProcessors
{
    public interface ICodeProcessor
    {
        string GetExtension();

        string MakeAddAParameter();

        string GetResultClassRegex();

        string GetNamespaceRegex();

        string NameAndPathForManifestStream(Project vsProject, Document queryDoc);
    }
}
