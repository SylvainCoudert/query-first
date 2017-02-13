using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst.CodeProcessors
{
    public class WrapperVisualBasicClassMaker : IWrapperClassMaker
    {
        public virtual string Usings(CodeGenerationContext ctx)
        {
            return @"Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Linq
Imports System.Runtime.Serialization

";

        }
        public virtual string StartNamespace(CodeGenerationContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.Namespace))
                return "Namespace " + ctx.Namespace + Environment.NewLine;
            else
                return "";
        }
        public virtual string StartClass(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("Public Partial Class " + ctx.BaseName);
            code.AppendLine("Implements " + ctx.InterfaceName + Environment.NewLine);

            return code.ToString();
        }
        public virtual string MakeExecuteWithoutConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            char[] spaceComma = new char[] { ',', ' ' };
            // Execute method, without connection
            code.AppendLine("Public Overridable Function Execute(" + ctx.MethodSignature.Trim(spaceComma) + ") As List(Of " + ctx.ResultClassName + ") Implements " + ctx.InterfaceName + ".Execute");
            code.AppendLine("using conn as IDbConnection = QfRuntimeConnection.GetConnection()");
            code.AppendLine("conn.Open()");
            code.AppendLine("return Execute(" + ctx.CallingArgs + ").ToList()");
            code.AppendLine("End Using");
            code.AppendLine("End Function");
            return code.ToString();
        }
        public virtual string MakeExecuteWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // Execute method with connection
            code.AppendLine("Public Iterator Overridable Function Execute(" + ctx.MethodSignature + "conn As IDbConnection) As IEnumerable(Of " + ctx.ResultClassName + ") Implements " + ctx.InterfaceName + ".Execute");
            code.AppendLine("Dim cmd As IDbCommand = conn.CreateCommand()");
            code.AppendLine("cmd.CommandText = getCommandText()");
            foreach (var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.VBName + ", " + qp.Length + ")");
            }
            code.AppendLine("using reader = cmd.ExecuteReader()");
            code.AppendLine("While reader.Read()");
            code.AppendLine("Yield Create(reader)");
            code.AppendLine("End While");
            code.AppendLine("End Using");
            code.AppendLine("End Function"); //close Execute() method
            return code.ToString();
        }
        public virtual string MakeGetOneWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            // GetOne without connection
            code.AppendLine("Public Overridable Function GetOne(" + ctx.MethodSignature.Trim(spaceComma) + ") As " + ctx.ResultClassName + " Implements " + ctx.InterfaceName + ".GetOne");
            code.AppendLine("using conn As IDbConnection = QfRuntimeConnection.GetConnection()");
            code.AppendLine("conn.Open()");
            code.AppendLine("return GetOne(" + ctx.CallingArgs + ")");
            code.AppendLine("End Using");
            code.AppendLine("End Function");
            return code.ToString();

        }
        public virtual string MakeGetOneWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // GetOne() with connection
            code.AppendLine("Public Overridable Function GetOne(" + ctx.MethodSignature + "conn As IDbConnection) As " + ctx.ResultClassName + " Implements " + ctx.InterfaceName + ".GetOne");
            code.AppendLine("Dim all = Execute(" + ctx.CallingArgs + ")");
            code.AppendLine("using iter As IEnumerator(Of " + ctx.ResultClassName + ")  = all.GetEnumerator()");
            code.AppendLine("iter.MoveNext()");
            code.AppendLine("return iter.Current");
            code.AppendLine("End Using");
            code.AppendLine("End Function"); // close GetOne() method
            return code.ToString();

        }
        public virtual string MakeExecuteScalarWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine("Public Overridable Function ExecuteScalar(" + ctx.MethodSignature.Trim(spaceComma) + ") As " + ctx.ResultFields[0].TypeVb + " Implements " + ctx.InterfaceName + ".ExecuteScalar");
            code.AppendLine("using conn As IDbConnection= QfRuntimeConnection.GetConnection()");
            code.AppendLine("conn.Open()");
            code.AppendLine("return ExecuteScalar(" + ctx.CallingArgs + ")");
            code.AppendLine("End Using");
            code.AppendLine("End Function");
            return code.ToString();
        }
        public virtual string MakeExecuteScalarWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteScalar() with connection
            code.AppendLine("Public Overridable Function ExecuteScalar(" + ctx.MethodSignature + "conn As IDbConnection) As " + ctx.ResultFields[0].TypeVb + " Implements " + ctx.InterfaceName + ".ExecuteScalar");
            code.AppendLine("Dim cmd As IDbCommand = conn.CreateCommand()");
            code.AppendLine("cmd.CommandText = getCommandText()");
            foreach (var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.VBName + ", " + qp.Length + ")");
            }
            code.AppendLine("return DirectCast(cmd.ExecuteScalar()," + ctx.ResultFields[0].TypeVbShort + ")");
            code.AppendLine("End Function");
            return code.ToString();

        }
        public virtual string MakeExecuteNonQueryWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine("Public Overridable Function ExecuteNonQuery(" + ctx.MethodSignature.Trim(spaceComma) + ") As Integer Implements " + ctx.InterfaceName + ".ExecuteNonQuery");
            code.AppendLine("using conn As IDbConnection = QfRuntimeConnection.GetConnection()");
            code.AppendLine("conn.Open()");
            code.AppendLine("return ExecuteNonQuery(" + ctx.CallingArgs + ")");
            code.AppendLine("End Using");
            code.AppendLine("End Function");
            return code.ToString();
        }
        public virtual string MakeExecuteNonQueryWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteScalar() with connection
            code.AppendLine("Public Overridable Function ExecuteNonQuery(" + ctx.MethodSignature + "conn As IDbConnection) As Integer Implements " + ctx.InterfaceName + ".ExecuteNonQuery");
            code.AppendLine("Dim cmd As IDbCommand = conn.CreateCommand()");
            code.AppendLine("cmd.CommandText = getCommandText()");
            foreach (var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.VBName + ", " + qp.Length + ")");
            }
            code.AppendLine("return cmd.ExecuteNonQuery()");
            code.AppendLine("End Function");
            // close ExecuteScalar()
            return code.ToString();

        }

        public virtual string MakeCreateMethod(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // Create() method
            code.AppendLine("Public Overridable Function Create(record As IDataRecord) As " + ctx.ResultClassName + " Implements " + ctx.InterfaceName + ".Create");
            code.AppendLine("Dim returnVal = new " + ctx.ResultClassName + "()");
            int j = 0;
            foreach (var col in ctx.ResultFields)
            {
                code.AppendLine("If record(" + j + ") IsNot Nothing And Not record.IsDBNull(" + j + ") Then");
                code.AppendLine("returnVal." + col.CSColumnName + " =  DirectCast(record(" + j++ + "), " + col.TypeVbShort + ")");
                code.AppendLine("End If");
            }
            // call OnLoad method in user's half of partial class
            code.AppendLine("returnVal.OnLoad()");
            code.AppendLine("return returnVal");

            code.AppendLine("End Function"); // close method;

            return code.ToString();
        }
        public virtual string MakeGetCommandTextMethod(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // private load command text
            code.AppendLine("Private Function getCommandText() As String");
            code.AppendLine("Dim strm As Stream = GetType(" + ctx.ResultClassName + ").Assembly.GetManifestResourceStream(\"" + ctx.NameAndPathForManifestStream + "\")");
            code.AppendLine("Dim queryText As String = new StreamReader(strm).ReadToEnd()");
            code.AppendLine("#If DEBUG Then ");
            code.AppendLine("'Comments inverted at runtime in debug, pre-build in release");
            code.AppendLine("queryText = queryText.Replace(\"-- designTime\", \"/*designTime\")");
            code.AppendLine("queryText = queryText.Replace(\"-- endDesignTime\", \"endDesignTime*/\")");
            // backwards compatible
            code.AppendLine("queryText = queryText.Replace(\"--designTime\", \"/*designTime\")");
            code.AppendLine("queryText = queryText.Replace(\"--endDesignTime\", \"endDesignTime*/\")");
            code.AppendLine("#End If");
            code.AppendLine("return queryText");
            code.AppendLine("End Function"); // close method;
            return code.ToString();

        }
        public virtual string MakeOtherMethods(CodeGenerationContext ctx)
        {
            return "";
        }
        public virtual string CloseClass(CodeGenerationContext ctx)
        {
            return "End Class" + Environment.NewLine;
        }
        public virtual string CloseNamespace(CodeGenerationContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.Namespace))
                return "End Namespace" + Environment.NewLine;
            else
                return "";
        }

        public string MakeInterface(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            code.AppendLine("Public Interface " + ctx.InterfaceName + Environment.NewLine);
            if (ctx.ResultFields != null && ctx.ResultFields.Count > 0)
            {
                code.AppendLine("Function Execute(" + ctx.MethodSignature.Trim(spaceComma) + ") As List(Of " + ctx.ResultClassName + ")");
                code.AppendLine("Function Execute(" + ctx.MethodSignature + "conn As IDbConnection) As IEnumerable(Of " + ctx.ResultClassName + ")");
                code.AppendLine("Function GetOne(" + ctx.MethodSignature.Trim(spaceComma) + ") As " + ctx.ResultClassName);
                code.AppendLine("Function GetOne(" + ctx.MethodSignature + "conn As IDbConnection) As " + ctx.ResultClassName);
                code.AppendLine("Function ExecuteScalar(" + ctx.MethodSignature.Trim(spaceComma) + ") As " + ctx.ResultFields[0].TypeVb);
                code.AppendLine("Function ExecuteScalar(" + ctx.MethodSignature + "conn As IDbConnection) As " + ctx.ResultFields[0].TypeVb);
                code.AppendLine("Function Create(record As IDataRecord) As " + ctx.ResultClassName);
            }
            code.AppendLine("Function ExecuteNonQuery(" + ctx.MethodSignature.Trim(spaceComma) + ") As Integer");
            code.AppendLine("Function ExecuteNonQuery(" + ctx.MethodSignature + "conn As IDbConnection) As Integer");
            code.AppendLine("End Interface"); // close interface;

            return code.ToString();
        }

        public string SelfTestUsings(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("Imports QueryFirst");
            code.AppendLine("Imports Xunit");
            return code.ToString();
        }

        public string MakeSelfTestMethod(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();

            code.AppendLine("<Fact>");
            code.AppendLine("Public Function SelfTest()");
            code.AppendLine("Dim errors = new List(Of string)()");
            code.AppendLine("Dim queryText = getCommandText()");
            code.AppendLine("' we'll be getting a runtime version with the comments section closed. To run without parameters, open it.");
            code.AppendLine("queryText = queryText.Replace(\"/*designTime\", \"-- designTime\")");
            code.AppendLine("queryText = queryText.Replace(\"endDesignTime*/\", \"-- endDesignTime\")");
            code.AppendLine("Dim schema = new ADOHelper().GetFields(new QfRuntimeConnection(), queryText)");
            for (int i = 0; i < ctx.ResultFields.Count; i++)
            {
                var col = ctx.ResultFields[i];
                code.AppendLine("If schema(" + i.ToString() + ").DataTypeName <> \"" + col.TypeDb + "\" Then");
                code.AppendLine("errors.Add(string.Format(\"Col " + i.ToString() + " (ColName) DB datatype has changed! Was " + col.TypeDb + ". Now {1}\", schema(" + i.ToString() + ").DataTypeName))");
                code.AppendLine("End If");
            }
            code.AppendLine("Assert.Empty(errors);");
            code.AppendLine("End Function");
            return code.ToString();
        }
    }
}
