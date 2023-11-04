namespace FilterLib;

public class Filter<T>
{
    public string? FieldName { get; set; }

    public string? Value { get; set; }
    
    public ValueOperationEnum? OperationWithValue { get; set; }
    
    public FilterOperationEnum? OperationWithParentFilter { get; set; }

    public List<Filter<T>>? Childs { get; set; }
}

