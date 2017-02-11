using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst.CodeProcessors
{
    public interface ICodeProcessor
    {
        string GetExtension();

        string MakeAddAParameter();

        string GetResultClassRegex();

        string GetNamespaceRegex();
    }
}
