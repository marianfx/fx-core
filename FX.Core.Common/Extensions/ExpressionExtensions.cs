using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FX.Core.Common.Extensions
{
    public static class ExpressionsExtensions
    {
        /// <summary>
        /// Extension for IQueryable: filters query items for items that match the rule
        /// p => p."DynamicProperty" == "ConstantValue". 
        /// By default the property name is "Id"
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> PropertyEquals<TEntity, T>(this IQueryable<TEntity> source, T value, string propertyName = "Id")
        {
            var expressionInLambda = ExpressionPredicates.GetPredicatePropertyEquals<TEntity, T>(value, propertyName);
            var where = Expression.Call(typeof(Queryable), "Where", new[] { source.ElementType }, source.Expression, expressionInLambda);
            return source.Provider.CreateQuery<TEntity>(where);
        }


        /// <summary>
        /// Extension for IQueryable: OrderBy/OrderByDescending with dynamic property names (string)
        /// https://stackoverflow.com/questions/7265186/how-do-i-specify-the-linq-orderby-argument-dynamically
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderByProperty"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc = false)
        {
            var command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            if (property == null)
                throw new Exception("Invalid Order By property name '" + orderByProperty + "'.");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }



        /// <summary>
        /// Extension for IQueryable: filters items (property selected with the passed selector) with the rule:
        /// p.Property != null && value != null && p.Property.ToLower().Trim().Contains(description.ToLower().Trim())
        /// Useful for search (similar to like '%text%'
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ContainsLowered<TEntity, T>(this IQueryable<TEntity> source, Expression<Func<TEntity, T>> selector, string value)
        {
            var expressionInLambda = ExpressionPredicates.GetPredicateContainsLowered(selector, value);
            var where = Expression.Call(typeof(Queryable), "Where", new[] { source.ElementType }, source.Expression, expressionInLambda);
            return source.Provider.CreateQuery<TEntity>(where);
        }


        /// <summary>
        /// Extension for IQueryable: filters the source for 'valid' dates. The source has items with properties 'StartDate' and 'EndDate'.
        /// On the given object, applies a dynamic lambda operation that checks if it is valid. Uses the predicate method.
        /// x => x.StartDate le date AND (x.EndDate = null OR x.EndDate > date)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<T> FilterByValidDate<T>(this IQueryable<T> source, DateTime date)
        {
            return source.Where(ExpressionPredicates.GetPredicateFilterByValidDate<T>(date));
        }
    }
}
