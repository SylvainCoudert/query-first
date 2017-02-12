using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst.CodeProcessors
{
    class SignatureCSharpMaker : ISignatureMaker
    {
        public void MakeMethodAndCallingSignatures(List<QueryParamInfo> ParamNamesAndTypes, out string MethodSignature, out string CallingSignature)
        {
            StringBuilder sig = new StringBuilder();
            StringBuilder call = new StringBuilder();
            foreach (var qp in ParamNamesAndTypes)
            {
                sig.Append(qp.CSType + ' ' + qp.CSName + ", ");
                call.Append(qp.CSName + ", ");
            }
            //signature trailing comma trimmed in place if needed. 
            call.Append("conn"); // calling args always used to call overload with connection
            MethodSignature = sig.ToString();
            CallingSignature = call.ToString();
        }
        
        public string MakeSignature(List<IQueryParamInfo> ParamNamesAndTypes)
        {
            StringBuilder sig = new StringBuilder();
            int i = 0;
            foreach (var qp in ParamNamesAndTypes)
            {
                sig.Append(qp.CSType + ' ' + qp.CSName + ", ");
                i++;
            }

            return sig.ToString();
        }

        public string MakeCallingArgs(List<IQueryParamInfo> ParamNamesAndTypes)
        {
            StringBuilder sig = new StringBuilder();
            StringBuilder call = new StringBuilder();
            foreach (var qp in ParamNamesAndTypes)
            {
                sig.Append(qp.CSType + ' ' + qp.CSName + ", ");
                call.Append(qp.CSName + ", ");
            }
            //signature trailing comma trimmed in place if needed. 
            call.Append("conn"); // calling args always used to call overload with connection
            return call.ToString();
        }
    }
}
