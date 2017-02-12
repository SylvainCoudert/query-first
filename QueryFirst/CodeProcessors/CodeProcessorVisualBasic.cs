using System.Text;

namespace QueryFirst.CodeProcessors
{
    public class CodeProcessorVisualBasic : ICodeProcessor
    {
        public string GetExtension()
        {
            return "vb";
        }

        public string GetNamespaceRegex()
        {
            return "(?im)^Namespace (\\S+)";
        }

        public string GetResultClassRegex()
        {
            return "(?im)Partial Public class (\\S+)";
        }

        public virtual string MakeAddAParameter()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("Private Sub AddAParameter(Cmd as IDbCommand, DbType as String, DbName as String , Value as Object, Length as Integer)\n");
            code.AppendLine("Dim MySqldbType = DirectCast(System.Enum.Parse(GetType(SqlDbType), DbType),SqlDbType)");
            code.AppendLine("Dim myParam As SqlParameter");
            code.AppendLine("If Length <> 0 Then");
            code.AppendLine("myParam = new SqlParameter(DbName, MySqldbType, Length)");
            code.AppendLine("Else");
            code.AppendLine("myParam = new SqlParameter(DbName, MySqldbType)");
            code.AppendLine("End If");
            code.AppendLine("myParam.Value = IIf(Value IsNot Nothing, Value, DBNull.Value)");
            code.AppendLine("Cmd.Parameters.Add( myParam)");
            code.AppendLine("End Sub");

            return code.ToString();

        }
    }
}
