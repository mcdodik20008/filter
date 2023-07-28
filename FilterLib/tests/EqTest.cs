using Newtonsoft.Json;
using NUnit.Framework;

namespace FilterLib.tests;

[TestFixture]
public class EqTest
{
    private PredicateBuilder<ObjectForTest> _predicateBuilder = new();

    [Test]
    public void CheckOneEqualsByString()
    {
        var testFilter = new Filter<ObjectForTest>
        {
            FieldName = "StrValue",
            Value = "ggg",
            OperationWithValue = FilterOperationEnum.EQ
        };
        var eqString = JsonConvert.SerializeObject(testFilter);
        var filter = JsonConvert.DeserializeObject<Filter<ObjectForTest>>(eqString);

        Assert.IsNotNull(filter);

        var expression = _predicateBuilder.GetPredicate(filter).Compile();
        var count = TestValues.Where(expression).Count();

        Assert.AreEqual(1, count);
    }

    [Test]
    public void CheckTwoEqualsByString()
    {
        var testFilter = new Filter<ObjectForTest>
        {
            FieldName = "StrValue",
            Value = "fff",
            OperationWithValue = FilterOperationEnum.EQ
        };
        var eqString = JsonConvert.SerializeObject(testFilter);
        var filter = JsonConvert.DeserializeObject<Filter<ObjectForTest>>(eqString);

        Assert.IsNotNull(filter);

        var expression = _predicateBuilder.GetPredicate(filter).Compile();
        var count = TestValues.Where(expression).Count();

        Assert.AreEqual(2, count);
    }

    [Test]
    public void CheckNestingObject()
    {
        var testFilter = new Filter<ObjectForTest>
        {
            Childs = new List<Filter<ObjectForTest>>(1)
            {
                new()
                {
                    FieldName = "StrValue.InsideStrValue",
                    Value = "kkk",
                    OperationWithValue = FilterOperationEnum.EQ,
                    OperationWithParentFilter = FilterOperationEnum.AND
                }
            }
        };
        var eqString = JsonConvert.SerializeObject(testFilter);
        var filter = JsonConvert.DeserializeObject<Filter<ObjectForTest>>(eqString);
        
        Assert.IsNotNull(filter);

        var expression = _predicateBuilder.GetPredicate(filter).Compile();
        var count = TestValues.Where(expression).Count();

        Assert.AreEqual(1, count);
    }


    private readonly List<ObjectForTest> TestValues = new(3)
    {
        new()
        {
            StrValue = "ggg",
            IntValue = 4,
            BoolValue = true,
            Object = new InsideFilterObject()
            {
                InsideStrValue = "kkk",
                InsideIntValue = 6,
                InsideBoolValue = true
            }
        },
        new()
        {
            StrValue = "fff",
            IntValue = 4,
            BoolValue = false,
            Object = new InsideFilterObject()
            {
                InsideStrValue = "ttt",
                InsideIntValue = 7,
                InsideBoolValue = false,
            }
        },
        new()
        {
            StrValue = "fff",
            IntValue = 5,
            BoolValue = true,
            Object = new InsideFilterObject()
            {
                InsideStrValue = "ttt",
                InsideIntValue = 7,
                InsideBoolValue = true,
            }
        }
    };
}