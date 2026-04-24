using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace ADay15.NET.Domain.Extensions
{

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var visitor = new ParameterRebinder(right.Parameters[0], left.Parameters[0]);
            var body = Expression.AndAlso(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var visitor = new ParameterRebinder(right.Parameters[0], left.Parameters[0]);
            var body = Expression.OrElse(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;
            public ParameterRebinder(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
