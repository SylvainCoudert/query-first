﻿using System;
using System.Text;

namespace QueryFirst.CodeProcessors
{
    public class WrapperCSharpClassMaker : IWrapperClassMaker
    {
        public virtual string Usings(CodeGenerationContext ctx)
        {
            return @"using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;

";

        }
        public virtual string StartNamespace(CodeGenerationContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.Namespace))
                return "namespace " + ctx.Namespace + "{" + Environment.NewLine;
            else
                return "";
        }
        public virtual string StartClass(CodeGenerationContext ctx)
        {
            return "public partial class " + ctx.BaseName + " : I" + ctx.BaseName + "{" + Environment.NewLine;

        }
        public virtual string MakeExecuteWithoutConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            char[] spaceComma = new char[] { ',', ' ' };
            // Execute method, without connection
            code.AppendLine("public virtual List<" + ctx.ResultClassName + "> Execute(" + ctx.MethodSignature.Trim(spaceComma) + "){");
            code.AppendLine("using (IDbConnection conn = QfRuntimeConnection.GetConnection())");
            code.AppendLine("{");
            code.AppendLine("conn.Open();");
            code.AppendLine("return Execute(" + ctx.CallingArgs + ").ToList();");
            code.AppendLine("}");
            code.AppendLine("}");
            return code.ToString();
        }
        public virtual string MakeExecuteWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // Execute method with connection
            code.AppendLine("public virtual IEnumerable<" + ctx.ResultClassName + "> Execute(" + ctx.MethodSignature + "IDbConnection conn){");
            code.AppendLine("IDbCommand cmd = conn.CreateCommand();");
            code.AppendLine("cmd.CommandText = getCommandText();");
            foreach (var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.CSName + ", " + qp.Length + ");");
            }
            code.AppendLine("using (var reader = cmd.ExecuteReader())");
            code.AppendLine("{");
            code.AppendLine("while (reader.Read())");
            code.AppendLine("{");
            code.AppendLine("yield return Create(reader);");
            code.AppendLine("}");
            code.AppendLine("}");
            code.AppendLine("}"); //close Execute() method
            return code.ToString();
        }
        public virtual string MakeGetOneWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            // GetOne without connection
            code.AppendLine("public virtual " + ctx.ResultClassName + " GetOne(" + ctx.MethodSignature.Trim(spaceComma) + "){");
            code.AppendLine("using (IDbConnection conn = QfRuntimeConnection.GetConnection())");
            code.AppendLine("{");
            code.AppendLine("conn.Open();");
            code.AppendLine("return GetOne(" + ctx.CallingArgs + ");");
            code.AppendLine("}");
            code.AppendLine("}");
            return code.ToString();

        }
        public virtual string MakeGetOneWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // GetOne() with connection
            code.AppendLine("public virtual " + ctx.ResultClassName + " GetOne(" + ctx.MethodSignature + "IDbConnection conn)");
            code.AppendLine("{");
            code.AppendLine("var all = Execute(" + ctx.CallingArgs + ");");
            code.AppendLine("using (IEnumerator<" + ctx.ResultClassName + "> iter = all.GetEnumerator())");
            code.AppendLine("{");
            code.AppendLine("iter.MoveNext();");
            code.AppendLine("return iter.Current;");
            code.AppendLine("}");
            code.AppendLine("}"); // close GetOne() method
            return code.ToString();

        }
        public virtual string MakeExecuteScalarWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine("public virtual " + ctx.ResultFields[0].TypeCs + " ExecuteScalar(" + ctx.MethodSignature.Trim(spaceComma) + "){");
            code.AppendLine("using (IDbConnection conn = QfRuntimeConnection.GetConnection())");
            code.AppendLine("{");
            code.AppendLine("conn.Open();");
            code.AppendLine("return ExecuteScalar(" + ctx.CallingArgs + ");");
            code.AppendLine("}");
            code.AppendLine("}");
            return code.ToString();
        }
        public virtual string MakeExecuteScalarWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteScalar() with connection
            code.AppendLine("public virtual " + ctx.ResultFields[0].TypeCs + " ExecuteScalar(" + ctx.MethodSignature + "IDbConnection conn){");
            code.AppendLine("IDbCommand cmd = conn.CreateCommand();");
            code.AppendLine("cmd.CommandText = getCommandText();");
            foreach (var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.CSName + ", " + qp.Length + ");");
            }
            code.AppendLine("return (" + ctx.ResultFields[0].TypeCs + ")cmd.ExecuteScalar();");
            code.AppendLine("}");
            // close ExecuteScalar()
            return code.ToString();

        }
        public virtual string MakeExecuteNonQueryWithoutConn(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine("public virtual int ExecuteNonQuery(" + ctx.MethodSignature.Trim(spaceComma) + "){");
            code.AppendLine("using (IDbConnection conn = QfRuntimeConnection.GetConnection())");
            code.AppendLine("{");
            code.AppendLine("conn.Open();");
            code.AppendLine("return ExecuteNonQuery(" + ctx.CallingArgs + ");");
            code.AppendLine("}");
            code.AppendLine("}");
            return code.ToString();
        }
        public virtual string MakeExecuteNonQueryWithConn(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteScalar() with connection
            code.AppendLine("public virtual int ExecuteNonQuery(" + ctx.MethodSignature + "IDbConnection conn){");
            code.AppendLine("IDbCommand cmd = conn.CreateCommand();");
            code.AppendLine("cmd.CommandText = getCommandText();");
            foreach(var qp in ctx.Query.QueryParams)
            {
                code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.CSName + ", " + qp.Length + ");");
            }
            code.AppendLine("return cmd.ExecuteNonQuery();");
            code.AppendLine("}");
            // close ExecuteScalar()
            return code.ToString();

        }

        public virtual string MakeCreateMethod(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // Create() method
            code.AppendLine("public virtual " + ctx.ResultClassName + " Create(IDataRecord record)");
            code.AppendLine("{");
            code.AppendLine("var returnVal = CreatePoco(record);");
            int j = 0;
            foreach (var col in ctx.ResultFields)
            {
                code.AppendLine("if(record[" + j + "] != null && record[" + j + "] != DBNull.Value)");
                code.AppendLine("returnVal." + col.CSColumnName + " =  (" + col.TypeCsShort + ")record[" + j++ + "];");
            }
            // call OnLoad method in user's half of partial class
            code.AppendLine("returnVal.OnLoad();");
            code.AppendLine("return returnVal;");

            code.AppendLine("}"); // close method;

            return code.ToString();
        }
        public virtual string MakeGetCommandTextMethod(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            // private load command text
            code.AppendLine("private string getCommandText(){");
            code.AppendLine("Stream strm = typeof(" + ctx.ResultClassName + ").Assembly.GetManifestResourceStream(\"" + ctx.NameAndPathForManifestStream + "\");");
            code.AppendLine("string queryText = new StreamReader(strm).ReadToEnd();");
            code.AppendLine("#if DEBUG");
            code.AppendLine("//Comments inverted at runtime in debug, pre-build in release");
            code.AppendLine("queryText = queryText.Replace(\"-- designTime\", \"/*designTime\");");
            code.AppendLine("queryText = queryText.Replace(\"-- endDesignTime\", \"endDesignTime*/\");");
            // backwards compatible
            code.AppendLine("queryText = queryText.Replace(\"--designTime\", \"/*designTime\");");
            code.AppendLine("queryText = queryText.Replace(\"--endDesignTime\", \"endDesignTime*/\");");
            code.AppendLine("#endif");
            code.AppendLine("return queryText;");
            code.AppendLine("}"); // close method;
            return code.ToString();

        }
        public virtual string MakeOtherMethods(CodeGenerationContext ctx)
        {
            return "";
        }
        public virtual string CloseClass(CodeGenerationContext ctx)
        {
            return "}" + Environment.NewLine;
        }
        public virtual string CloseNamespace(CodeGenerationContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.Namespace))
                return "}" + Environment.NewLine;
            else
                return "";
        }

        public string MakeInterface(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            code.AppendLine("public interface I" + ctx.BaseName + "{" + Environment.NewLine);
            if (ctx.ResultFields != null && ctx.ResultFields.Count > 0)
            {
                code.AppendLine("List<" + ctx.ResultClassName + "> Execute(" + ctx.MethodSignature.Trim(spaceComma) + ");");
                code.AppendLine("IEnumerable<" + ctx.ResultClassName + "> Execute(" + ctx.MethodSignature + "IDbConnection conn);");
                code.AppendLine("" + ctx.ResultClassName + " GetOne(" + ctx.MethodSignature.Trim(spaceComma) + ");");
                code.AppendLine("" + ctx.ResultClassName + " GetOne(" + ctx.MethodSignature + "IDbConnection conn);");
                code.AppendLine("" + ctx.ResultFields[0].TypeCs + " ExecuteScalar(" + ctx.MethodSignature.Trim(spaceComma) + ");");
                code.AppendLine("" + ctx.ResultFields[0].TypeCs + " ExecuteScalar(" + ctx.MethodSignature + "IDbConnection conn);");
                code.AppendLine("" + ctx.ResultClassName + " Create(IDataRecord record);");
            }
            code.AppendLine("int ExecuteNonQuery(" + ctx.MethodSignature.Trim(spaceComma) + ");");
            code.AppendLine("int ExecuteNonQuery(" + ctx.MethodSignature + "IDbConnection conn);");
            code.AppendLine("}"); // close interface;

            return code.ToString();
        }

        public string SelfTestUsings(CodeGenerationContext ctx)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("using QueryFirst;");
            code.AppendLine("using Xunit;");
            return code.ToString();
        }

        public string MakeSelfTestMethod(CodeGenerationContext ctx)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();

            code.AppendLine("[Fact]");
            code.AppendLine("public void SelfTest()");
            code.AppendLine("{");
            code.AppendLine("var errors = new List<string>();");
            code.AppendLine("var queryText = getCommandText();");
            code.AppendLine("// we'll be getting a runtime version with the comments section closed. To run without parameters, open it.");
            code.AppendLine("queryText = queryText.Replace(\"/*designTime\", \"-- designTime\");");
            code.AppendLine("queryText = queryText.Replace(\"endDesignTime*/\", \"-- endDesignTime\");");
            code.AppendLine("var schema = new ADOHelper().GetFields(new QfRuntimeConnection(), queryText);");
            for (int i = 0; i < ctx.ResultFields.Count; i++)
            {
                var col = ctx.ResultFields[i];
                code.AppendLine("if (schema[" + i.ToString() + "].DataTypeName != \"" + col.TypeDb + "\")");
                code.AppendLine("errors.Add(string.Format(\"Col " + i.ToString() + " (ColName) DB datatype has changed! Was " + col.TypeDb + ". Now {1}\", schema[" + i.ToString() + "].DataTypeName));");
            }
            code.AppendLine("Assert.Empty(errors);");
            code.AppendLine("}");
            return code.ToString();
        }
    }
}
