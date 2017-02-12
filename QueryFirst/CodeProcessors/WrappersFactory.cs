namespace QueryFirst.CodeProcessors
{
    public static class WrappersFactory
    {
        public const string prjKindCSharpProject = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string prjKindVBProject = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string prjKindVSAProject = "{13B7A3EE-4614-11D3-9BC7-00C04F79DE25}";

        public static ICodeProcessor GetProcessor(string projectKind)
        {
            switch (projectKind)
            {
                case prjKindCSharpProject:
                    return new CodeProcessorCSharp();
                case prjKindVBProject:
                    return new CodeProcessorVisualBasic();
                default:
                    throw new UnsupportedProjectTypeException();
            }
        }

        public static ISignatureMaker GetSignatureMaker(string projectKind)
        {
            switch (projectKind)
            {
                case prjKindCSharpProject:
                    return new SignatureCSharpMaker();
                case prjKindVBProject:
                    return new SignatureCSharpMaker();
                default:
                    throw new UnsupportedProjectTypeException();
            }
        }

        public static IWrapperClassMaker GetWrapperClassMaker(string projectKind)
        {
            switch (projectKind)
            {
                case prjKindCSharpProject:
                    return new WrapperCSharpClassMaker();
                case prjKindVBProject:
                    return new WrapperVisualBasicClassMaker();
                default:
                    throw new UnsupportedProjectTypeException();
            }
        }

        public static IResultClassMaker GetResultClassMaker(string projectKind)
        {
            switch (projectKind)
            {
                case prjKindCSharpProject:
                    return new ResultCSharpClassMaker();
                case prjKindVBProject:
                    return new ResultVisualBasicClassMaker();
                default:
                    throw new UnsupportedProjectTypeException();
            }
        }

    }
}
