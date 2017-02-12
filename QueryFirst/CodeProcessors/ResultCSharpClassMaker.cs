using System;

namespace QueryFirst.CodeProcessors
{
    public class ResultCSharpClassMaker : IResultClassMaker
    {
        public virtual string Usings() { return ""; }

        private string nl = Environment.NewLine;
        public virtual string StartClass(CodeGenerationContext ctx)
        {
            return string.Format("public partial class {0} {{" + nl, ctx.ResultClassName);
        }
        public virtual string MakeProperty(ResultFieldDetails fld)
        {
            return string.Format("public {0} {1}; //({2} {3})" + nl, fld.TypeCsShort, fld.CSColumnName, fld.TypeDb, fld.AllowDBNull ? "null" : "not null");
        }

        public virtual string CloseClass()
        {
            return "}" + nl;
        }
    }    
}
