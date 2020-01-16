using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace FX.Core.Common.Extensions
{
    public static class ExpressionPredicates
    {
        /// <summary>
        /// Builds an OR predicate (expression) between two exiting expressions.
        /// Result: (expression1) OR (expression2) [boolean expression]
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetPredicateOr<TEntity, T>(Expression<Func<TEntity, bool>> e1,
            Expression<Func<TEntity, bool>> e2)
        {
            return PredicateBuilder.New<TEntity>().Or(e1).Or(e2);
        }

        /// <summary>
        /// Builds an predicate (expression) that searches for a property name on the TEntity object, and returns true if the object has a property with that name and a given value.
        /// Result: p."DynamicPropertyName" == "Constant value" [boolean expression]
        /// Can be passed to .Where, FirstOrDefault etc (predicates, return bool)
        /// Fully compatible with EF / Data providers / IQueriable. Useful when property names are dynamic
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetPredicatePropertyEquals<TEntity, T>(T value, string propertyName = "Id")
        {
            var type = typeof(TEntity);

            // set constants
            var constant = Expression.Constant(value);
            var parameter = Expression.Parameter(type, "p");
            var valueGetter = Expression.Property(parameter, propertyName);

            // build expression => first check for property existance
            var propType = type.GetProperty(propertyName)?.PropertyType;
            if (propType == null)
                throw new Exception($"Property {propertyName} not found on object {typeof(TEntity).Name}.");

            // check if conversion needed
            Expression exprToUse;
            if (propType != typeof(T))
                exprToUse = Expression.Convert(constant, propType);
            else
                exprToUse = constant;

            var fullExpression = Expression.Equal(valueGetter, exprToUse);
            return Expression.Lambda<Func<TEntity, bool>>(fullExpression, parameter);
        }

        /// <summary>
        /// Builds an predicate (expression) that can give access to a property member (generates (p => p.Member), the member name being given as a string parameter.
        /// Result: p => p."DynamicMemberValue" [selector expression]
        /// Can be passed to anything LINQ-like (Where, OrderBy, GroupBy etc) that builds a member access dynamically
        /// (eg p => p.Id), wiith "Id" given as string param
        /// Fully compatible with EF / Data providers / IQueriable. Useful when property names are dynamic
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, T>> GetPredicateMemberAccess<TEntity, T>(string propertyName = "Id")
        {
            var type = typeof(TEntity);
            var property = type.GetProperty(propertyName);
            if (property == null)
                throw new Exception($"Property {propertyName} not found on object {typeof(TEntity).Name}.");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            return Expression.Lambda<Func<TEntity, T>>(propertyAccess, parameter);
        }


        /// <summary>
        /// Builds an predicate (expression) that returns true if a member (accessed with selector p => p.Member) contains a value (string), lowered and trimmed.
        /// Result: p => p."DynamicMember".ToLower().Trim() == "Constant value".ToLower().Trim() [boolean expression]
        /// 
        /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/how-to-use-expression-trees-to-build-dynamic-queries
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<TEntity, bool>> GetPredicateContainsLowered<TEntity, T>(Expression<Func<TEntity, T>> selector, string value)
        {
            // get prerequisites
            var valueLowered = value?.ToLower().Trim();
            var lowerMethod = typeof(string).GetMethods().First(m => m.Name == "ToLower" && m.GetParameters().Length == 0);
            var trimMethod = typeof(string).GetMethods().First(m => m.Name == "Trim" && m.GetParameters().Length == 0);
            var containsMethod = typeof(string).GetMethods().First(m => m.Name == "Contains" && m.GetParameters().Length == 1);

            // set constants
            var constant = Expression.Constant(valueLowered);
            var nullConst = Expression.Constant(null);
            var memberExpression = (MemberExpression)selector.Body;
            var parameterExpression = Expression.Parameter(typeof(TEntity), "p");
            var valueGetter = Expression.Property(parameterExpression,
                typeof(TEntity).GetProperty(memberExpression.Member.Name));

            // build expression x.Description != null && description != null
            var exprNotNull1 = Expression.NotEqual(constant, nullConst);
            var exprNotNull2 = Expression.NotEqual(valueGetter, nullConst);
            var exprNotNull = Expression.AndAlso(exprNotNull1, exprNotNull2);

            // build expression x.Description.ToLower().Trim().Contains(valueLowered)
            var expressionLower = Expression.Call(valueGetter, lowerMethod);
            var expressionTrim = Expression.Call(expressionLower, trimMethod);
            var expressionContains = Expression.Call(expressionTrim, containsMethod, constant);

            // build final expression
            var expressionFull = Expression.AndAlso(exprNotNull, expressionContains);
            return Expression.Lambda<Func<TEntity, bool>>(expressionFull, parameterExpression);
        }

        /// <summary>
        /// Builds an predicate (expression) that checks if the given StartDate and EndDate properties of an object are valid at a given date.
        /// /// Result: p => p."DynamicStartDate" <= date && (p."DynamicEndDate" == null || p."DynamicEndDate" > date) [boolean expression]
        /// The output is a predicate. This predicate will filter only elements that are 'Valid' at the given date.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetPredicateFilterByValidDate<T>(DateTime? date, string startDatePropName = "StartDate", string endDatePropName = "EndDate")
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var member1 = Expression.Property(parameter, startDatePropName);
            var member2 = Expression.Property(parameter, endDatePropName);
            var dateConst = Expression.Constant(date, typeof(DateTime?));
            var dateNotNull = Expression.Constant(date, typeof(DateTime));
            var nullConst = Expression.Constant(null);
            var startDateLower = Expression.GreaterThanOrEqual(dateNotNull, member1);
            var equalNull = Expression.Equal(member2, nullConst);
            var endDateGreater = Expression.GreaterThan(member2, dateConst);
            var innerRight = Expression.OrElse(equalNull, endDateGreater);
            var fullOp = Expression.AndAlso(startDateLower, innerRight);
            return Expression.Lambda<Func<T, bool>>(fullOp, parameter);
        }
    }
}
