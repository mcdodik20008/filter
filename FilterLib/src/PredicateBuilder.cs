using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using FilterLib.tests;
using LinqKit;

namespace FilterLib;

public class PredicateBuilder<T>
{
    private readonly PropertyInfo[] _propertyInfos;

    public PredicateBuilder()
    {
        _propertyInfos = typeof(T).GetProperties();
    }

    public Expression<Func<T, bool>> GetPredicate(Filter<T> filter)
    {
        if (filter.FieldName == null)
        {
            return x => true;
        }
        
        var exp = PredicateBuilder.New<T>();
        var property = _propertyInfos.GetPropertyByName(filter.FieldName!);
        if (property is null)
        {
            throw new ValidationException("Not found property with name: " + filter.FieldName);
        }


        var function = filter.OperationWithValue switch
        {
            FilterOperationEnum.EQ => GetEquals(property, filter.Value!),
            _ => throw new ArgumentOutOfRangeException(filter.OperationWithValue.ToString(),
                "Неподдерживаемая операция над фильтром")
        };
        exp.Start(function);
        if (filter.Childs == null) return exp;

        return filter.Childs.Aggregate(exp, (current, childFilter) => childFilter.OperationWithParentFilter switch
        {
            FilterOperationEnum.AND => current.And(GetPredicate(childFilter)),
            FilterOperationEnum.OR => current.Or(GetPredicate(childFilter)),
            _ => throw new ArgumentOutOfRangeException(childFilter.OperationWithParentFilter.ToString(), "Неподдерживаемая операция над фильтрами")
        });
    }

    private Expression<Func<T, bool>> GetEquals(PropertyInfo property, string value) =>
        x => property.GetValue(x)!.Equals(value);
    

    public Func<T, object?> GetValueByPropertyPath(string filterPath)
    {
        var property = _propertyInfos.GetPropertyByName(filterPath.GetFirstNameInPath());
        return filterPath.Split(".").Length == 1 ? 
            x => property?.GetValue(x) : 
            GetValueByPropertyPath(x => property?.GetValue(x), filterPath.RemoveFirstObjectInPath());
    }
    
    private Func<T, object> GetValueByPropertyPath(Func<T, object?> oldFunc, string filterPath)
    {
        
    }
}