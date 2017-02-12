using System.Text;

namespace QueryFirst.CodeProcessors
{
    public class CodeProcessorCSharp : ICodeProcessor
    {
        public string GetExtension()
        {
            return "cs";
        }

        public string GetNamespaceRegex()
        {
            return "(?im)^namespace (\\S+)";
        }

        public string GetResultClassRegex()
        {
            return "(?im)partial class (\\S+)";
        }

        public virtual string MakeAddAParameter()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("private void AddAParameter(IDbCommand Cmd, string DbType, string DbName, object Value, int Length)\n{");
            code.AppendLine("var dbType = (SqlDbType)System.Enum.Parse(typeof(SqlDbType), DbType);");
            code.AppendLine("SqlParameter myParam;");
            code.AppendLine("if(Length != 0){");
            code.AppendLine("myParam = new SqlParameter(DbName, dbType, Length);");
            code.AppendLine("}else{");
            code.AppendLine("myParam = new SqlParameter(DbName, dbType);");
            code.AppendLine("}");
            code.AppendLine("myParam.Value = Value != null ? Value : DBNull.Value;");
            code.AppendLine("Cmd.Parameters.Add( myParam);");
            code.AppendLine("}");

            return code.ToString();

        }
    }
}
