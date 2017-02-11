namespace QueryFirst
{
    public interface IQueryParamInfo
    {
        string CSType { get; set; }
        string VBType { get; set; }
        //bool ExplicitlyDeclared { get; set; }
        int Length { get; set; }
        int Precision { get; set; }
        int Scale { get; set; }
        string CSName { get; set; }
        string VBName { get; set; }
        string DbName { get; set; }
        string DbType { get; set; }
        //string SqlTypeAndLength { get; set}
    }
}