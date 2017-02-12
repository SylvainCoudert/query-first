using System;

namespace QueryFirst.CodeProcessors
{
    public interface IResultClassMaker
    {
        string Usings();
        string StartClass(CodeGenerationContext ctx);
        string MakeProperty(ResultFieldDetails fld);
        string CloseClass();
    }
}
