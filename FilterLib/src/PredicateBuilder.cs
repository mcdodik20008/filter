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
        if (filter.FieldName == null && filter.Childs is null)
        {
            return x => true;
        }

        var exp = PredicateBuilder.New<T>();
        if (filter.FieldName is not null)
        {
            var property = _propertyInfos.GetPropertyByName(filter.FieldName.GetFirstNameInPath());
            if (property is null)
            {
                throw new ValidationException("Not found property with name: " + filter.FieldName);
            }
            var function = filter.OperationWithValue switch
            {
                ValueOperationEnum.EQ => GetEquals(filter.FieldName, filter.Value!),
                _ => throw new ArgumentOutOfRangeException(filter.OperationWithValue.ToString(),
                    "Неподдерживаемая операция над фильтром")
            };
            exp.Start(function);
        }
        
        if (filter.Childs == null) return exp;
        
        foreach (var child in filter.Childs)
            exp = child.OperationWithParentFilter switch
            {
                FilterOperationEnum.AND => exp.And(GetPredicate(child)),
                FilterOperationEnum.OR => exp.Or(GetPredicate(child)),
                _ => throw new ArgumentOutOfRangeException(child.OperationWithParentFilter.ToString(), "Неподдерживаемая операция над фильтрами")
            };
        return exp;
    }

    /**
     *   Подумать в каких рамках хешировать значения
     */
    private Expression<Func<T, bool>> GetEquals(string propertyPath, string value)
    {
        return GetValueByPropertyPath(propertyPath) is null ? 
            x => true : // подходит все
            x => GetValueByPropertyPath(propertyPath).ToString().Equals(value); // фильтуем по нужному атрибуту
    }

    public Func<T, object?> GetValueByPropertyPath(string filterPath)
    {
        var property = _propertyInfos.GetPropertyByName(filterPath.GetFirstNameInPath());
        return filterPath.Split(".").Length switch
        {
            0 => null,
            1 => x => property?.GetValue(x), // фильтруем по первой вложенности
            // Фильтруем по любой вложенности, убирая первую
            _ => GetValueByPropertyPath(x => property?.GetValue(x), filterPath.RemoveFirstObjectInPath())
        };
    }
    
    private Func<T, object> GetValueByPropertyPath(Func<T, object?> oldFunc, string filterPath)
    {
        Func<T, object> func = x => oldFunc(x) // получяем объект любой вложенности
                               .GetType()
                               .GetProperties()
                               .GetPropertyByName(filterPath.GetFirstNameInPath())
                               .GetValue(oldFunc(x))
                               .ToConsole(); // получаем значение нужно проперти объекта
        return filterPath.Split(".").Length == 1
            ? func
            : // если крайняя вложенность, то хватит, вернем то, что есть
            GetValueByPropertyPath(func, filterPath.RemoveFirstObjectInPath()); // спустимся еще глубже
    }
}