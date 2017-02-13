using System;
using System.Text;

namespace QueryFirst.CodeProcessors
{
    public class ResultVisualBasicClassMaker : IResultClassMaker
    {
        public virtual string Usings() { return ""; }

        private string nl = Environment.NewLine;
        public virtual string StartClass(CodeGenerationContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<DataContract>");
            sb.AppendLine(string.Format("public partial class {0} " + nl, ctx.ResultClassName));
            return sb.ToString();
        }
        public virtual string MakeProperty(ResultFieldDetails fld)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<DataMember>");
            sb.AppendLine(string.Format("Public Property {0} As {1} '({2} {3})" + nl, fld.CSColumnName, fld.TypeVbShort, fld.TypeDb, fld.AllowDBNull ? "null" : "not null"));
            return sb.ToString();
        }

        public virtual string CloseClass()
        {
            return "End Class" + nl;
        }
    }
}
