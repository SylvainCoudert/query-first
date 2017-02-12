﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using EnvDTE;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using QueryFirst.TypeMappings;
using QueryFirst.CodeProcessors;

namespace QueryFirst
{
    public class CodeGenerationContext
    {
        #region Code Processors
        private ICodeProcessor _codeProcessor;
        public ICodeProcessor CodeProcessor
        {
            get
            {
                if(_codeProcessor == null)
                {
                    _codeProcessor = WrappersFactory.GetProcessor(config.Project.Kind);
                }
                return _codeProcessor;
            }
        }

        protected IProvider provider;
        public IProvider Provider { get { return provider; } }

        protected ISignatureMaker _signatureMaker;
        public ISignatureMaker SignatureMaker
        {
            get
            {
                if (_signatureMaker == null)
                {
                    _signatureMaker = WrappersFactory.GetSignatureMaker(config.Project.Kind);
                }
                return _signatureMaker;
            }
        }
        #endregion
        protected TinyIoCContainer tiny;
        private PutCodeHere _putCodeHere;
        public PutCodeHere PutCodeHere { get { return _putCodeHere; } }
        protected DTE dte;
        public DTE Dte { get { return dte; } }
        protected Document queryDoc;
        public Document QueryDoc { get { return queryDoc; } }
        protected Query query;
        public Query Query { get { return query; } }
        protected string baseName;
        private ConfigurationAccessor config;
        public ConfigurationAccessor ProjectConfig
        {
            get
            {
                if (config == null)
                {
                    try
                    {
                        config = new ConfigurationAccessor(dte, null);
                    }
                    catch (Exception ex)
                    {
                        // will throw if there's no configuration file
                        return null;
                    }
                }
                return config;
            }
        }
        /// <summary>
        /// The name of the query file, without extension. Used to infer the filenames of code classes, and to generate the wrapper class name.
        /// </summary>
        public string BaseName
        {
            get
            {
                if (baseName == null)
                    baseName = Path.GetFileNameWithoutExtension((string)queryDoc.ProjectItem.Properties.Item("FullPath").Value);
                return baseName;
            }
        }

        /// <summary>
        /// Construct TheInterface Name from the baseName
        /// </summary>
        public string InterfaceName
        {
            get
            {
                return "I" + baseName;
            }
        }

        /// <summary>
        /// The directory containing the 3 files for this query, with trailing slash
        /// </summary>
        public string CurrDir { get { return Path.GetDirectoryName((string)queryDoc.ProjectItem.Properties.Item("FullPath").Value) + "\\"; } }
        /// <summary>
        /// The query filename, extension and path relative to approot. Used for generating the call to GetManifestStream()
        /// </summary>
        public string PathFromAppRoot
        {
            get
            {
                string fullNameAndPath = (string)queryDoc.ProjectItem.Properties.Item("FullPath").Value;
                return fullNameAndPath.Substring(queryDoc.ProjectItem.ContainingProject.Properties.Item("FullPath").Value.ToString().Length);
            }
        }
        /// <summary>
        /// DefaultNamespace.Path.From.Approot.QueryFileName.sql
        /// </summary>
        public string NameAndPathForManifestStream
        {
            get
            {
                EnvDTE.Project vsProject = queryDoc.ProjectItem.ContainingProject;
                return vsProject.Properties.Item("DefaultNamespace").Value.ToString() + '.' + PathFromAppRoot.Replace('\\', '.');
            }
        }
        /// <summary>
        /// Path and filename of the generated code file.
        /// </summary>
        public string GeneratedClassFullFilename
        {
            get
            {
                return CurrDir + BaseName + ".gen." + CodeProcessor.GetExtension();
            }
        }
        protected string userPartialClass;
        protected string resultClassName;
        /// <summary>
        /// Result class name, read from the user's half of the partial class, written to the generated half.
        /// </summary>
        public virtual string ResultClassName
        {
            get
            {
                if (string.IsNullOrEmpty(userPartialClass))
                    userPartialClass = File.ReadAllText(CurrDir + BaseName + "Results." + CodeProcessor.GetExtension());
                if (resultClassName == null)
                    resultClassName = Regex.Match(userPartialClass, CodeProcessor.GetResultClassRegex()).Groups[1].Value;
                return resultClassName;

            }
        }
        /// <summary>
        /// The query namespace, read from the user's half of the result class, used for the generated code file.
        /// </summary>
        public virtual string Namespace
        {
            get
            {
                if (string.IsNullOrEmpty(userPartialClass))
                    userPartialClass = File.ReadAllText(CurrDir + BaseName + "Results." + CodeProcessor.GetExtension());
                return Regex.Match(userPartialClass, CodeProcessor.GetNamespaceRegex()).Groups[1].Value;

            }
        }
        protected DesignTimeConnectionString _dtcs;
        public DesignTimeConnectionString DesignTimeConnectionString
        {
            get
            {
                return _dtcs ?? (_dtcs = new DesignTimeConnectionString(this));
            }
        }

        protected string methodSignature;
        /// <summary>
        /// Parameter types and names, extracted from sql, with trailing comma.
        /// </summary>
        public virtual string MethodSignature
        {
            get
            {
                if (string.IsNullOrEmpty(methodSignature))
                {

                    methodSignature = SignatureMaker.MakeSignature(Query.QueryParams);
                }
                return methodSignature;
            }
        }
        //taken out of constructor, we don't need this anymore????
        //                ((ISignatureMaker)TinyIoCContainer.Current.Resolve(typeof(ISignatureMaker)))
        //.MakeMethodAndCallingSignatures(ctx.Query.QueryParams, out methodSignature, out callingArgs);
        protected string callingArgs;
        /// <summary>
        /// Parameter names, if any, with trailing "conn". String used by connectionless methods to call their connectionful overloads.
        /// </summary>
        public string CallingArgs
        {
            get
            {
                if (string.IsNullOrEmpty(callingArgs))
                {
                    callingArgs = SignatureMaker.MakeSignature(Query.QueryParams);
                }
                return callingArgs;
            }
        }
        protected List<ResultFieldDetails> resultFields;
        /// <summary>
        /// The schema table returned from the dummy run of the query.
        /// </summary>
        public List<ResultFieldDetails> ResultFields
        {
            get { return resultFields; }
            set { resultFields = value; }
        }

        protected bool queryHasRun;
        public bool QueryHasRun
        {
            get { return queryHasRun; }
            set { queryHasRun = value; }
        }
        protected AdoSchemaFetcher hlpr;
        /// <summary>
        /// The class that runs the query and returns the schema table
        /// </summary>
        public AdoSchemaFetcher Hlpr { get { return hlpr; } }
        protected ProjectItem resultsClass;

        // constructor
        public CodeGenerationContext(Document queryDoc)
        {
            tiny = TinyIoCContainer.Current;
            queryHasRun = false;
            this.queryDoc = queryDoc;
            dte = queryDoc.DTE;
            query = new Query(this);
            provider = tiny.Resolve<IProvider>(DesignTimeConnectionString.v.ProviderName);
            provider.Initialize(DesignTimeConnectionString.v);
            // resolving the target project item for code generation. We know the file name, we loop through child items of the query til we find it.
            _putCodeHere = new PutCodeHere(Conductor.GetItemByFilename(queryDoc.ProjectItem.ProjectItems, GeneratedClassFullFilename));


            string currDir = Path.GetDirectoryName(queryDoc.FullName);
            //WriteToOutput("\nprocessing " + queryDoc.FullName );
            // class name and namespace read from user's half of partial class.
            //QfClassName = Regex.Match(query, "(?im)^--QfClassName\\s*=\\s*(\\S+)").Groups[1].Value;
            //QfNamespace = Regex.Match(query, "(?im)^--QfNamespace\\s*=\\s*(\\S+)").Groups[1].Value;
            // doc.fullname started being lowercase ??
            //namespaceAndClassNames = GetNamespaceAndClassNames(resultsClass);

            hlpr = new AdoSchemaFetcher();
        }

    }
}
