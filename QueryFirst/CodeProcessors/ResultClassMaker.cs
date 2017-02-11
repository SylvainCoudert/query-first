﻿using System;

namespace QueryFirst.CodeProcessors
{
    public interface IResultClassMaker
    {
        string Usings();
        string StartClass(CodeGenerationContext ctx);
        string MakeProperty(ResultFieldDetails fld);
        string CloseClass();
    }

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

    public class ResultVisualBasicClassMaker : IResultClassMaker
    {
        public virtual string Usings() { return ""; }

        private string nl = Environment.NewLine;
        public virtual string StartClass(CodeGenerationContext ctx)
        {
            return string.Format("public partial class {0} " + nl, ctx.ResultClassName);
        }
        public virtual string MakeProperty(ResultFieldDetails fld)
        {
            return string.Format("Public Property {0} As {1} '({2} {3})" + nl, fld.CSColumnName, fld.TypeCsShort, fld.TypeDb, fld.AllowDBNull ? "null" : "not null");
        }

        public virtual string CloseClass()
        {
            return "End Class" + nl;
        }
    }
}
