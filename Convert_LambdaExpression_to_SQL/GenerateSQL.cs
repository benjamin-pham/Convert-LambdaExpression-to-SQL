using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convert_LambdaExpression_to_SQL
{
    public class GenerateSQL<TEntity> where TEntity : class
    {
        //private string constQuery = @$"SELECT * FROM {typeof(TEntity).Name}";
        private string query { get; set; }
        private string select { get; set; }
        private string where { get; set; }
        private List<string> listSelect { get; set; }
        private List<string> listWhere { get; set; }
        private string whereClause { get; set; }
        //private string selectStatement { get; set; }
        public GenerateSQL()
        {
            this.listSelect = new List<string>();
            this.listWhere = new List<string>();
        }
        //public GenerateSQL(List<string> listSelect)
        //{
        //    this.listSelect = listSelect;
        //}
        private IEnumerable<string> GetColumns()
        {
            return typeof(TEntity)
                    .GetProperties()
                    .Where(e => e.Name != "id" && e.GetCustomAttribute<NotMappedAttribute>() == null)
                    .Select(e => e.Name);
        }
        private string GetComparisonlOperation(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    return null;
            }
        }
        private string GenerateSelectStatement(Expression<Func<TEntity, TEntity>> select)
        {
            var list = new List<string>();
            var temp = ((MemberInitExpression)select.Body).Bindings.ToList().Select(e => e.Member.Name);
            list.AddRange(temp);
            return string.Join(", ", list);
        }
        private List<string> GenerateSelect(Expression<Func<TEntity, TEntity>> select)
        {
            var list = new List<string>();
            var temp = ((MemberInitExpression)select.Body).Bindings.ToList().Select(e => e.Member.Name);
            list.AddRange(temp);
            return list.Distinct().ToList();
        }
        private void GengerateWhereClause(BinaryExpression expression)
        {
            if (expression != null)
            {
                
                if (!string.IsNullOrEmpty(GetComparisonlOperation(expression.NodeType)))
                {
                    if (expression.Left.NodeType is ExpressionType.MemberAccess && expression.Right.NodeType is ExpressionType.Constant)
                    {
                        this.whereClause = this.whereClause + ((MemberExpression)expression.Left).Member.Name
                            + GetComparisonlOperation(expression.NodeType)
                            + "'" + Expression.Lambda(expression.Right).Compile().DynamicInvoke() + "'";
                    }
                    else if (expression.Left.NodeType is ExpressionType.Constant && expression.Right.NodeType is ExpressionType.MemberAccess)
                    {
                        this.whereClause = this.whereClause + "'" + Expression.Lambda(expression.Left).Compile().DynamicInvoke() + "'"
                            + GetComparisonlOperation(expression.NodeType)
                            + ((MemberExpression)expression.Right).Member.Name;

                    }
                    else
                    {
                        throw new Exception("fail");
                    }
                    
                }
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.whereClause += "(";
                }
                if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.AndAlso)
                {
                    GengerateWhereClause((BinaryExpression)expression.Left);
                }
                
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.whereClause += " or ";
                }
                
                if (expression.NodeType == ExpressionType.AndAlso)
                {

                    this.whereClause += " and ";
                }
                
                if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.AndAlso)
                {                    
                    GengerateWhereClause((BinaryExpression)expression.Right);                    
                }
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.whereClause += ")";
                }
            }
        }
        private void GengerateWhere(BinaryExpression expression)
        {
            if (expression != null)
            {
                if (!string.IsNullOrEmpty(GetComparisonlOperation(expression.NodeType)))
                {
                    if (expression.Left.NodeType is ExpressionType.MemberAccess && expression.Right.NodeType is ExpressionType.Constant)
                    {
                        this.listWhere.Add(((MemberExpression)expression.Left).Member.Name
                            + GetComparisonlOperation(expression.NodeType)
                            + "'" + Expression.Lambda(expression.Right).Compile().DynamicInvoke() + "'");
                    }
                    else if (expression.Left.NodeType is ExpressionType.Constant && expression.Right.NodeType is ExpressionType.MemberAccess)
                    {
                        this.listWhere.Add("'" + Expression.Lambda(expression.Left).Compile().DynamicInvoke() + "'"
                            + GetComparisonlOperation(expression.NodeType)
                            + ((MemberExpression)expression.Right).Member.Name);
                    }
                    else
                    {
                        throw new Exception("fail");
                    }
                }
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.listWhere.Add("(");
                }
                if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.AndAlso)
                {
                    GengerateWhere((BinaryExpression)expression.Left);
                }
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.listWhere.Add("or");
                }
                if (expression.NodeType == ExpressionType.AndAlso)
                {
                    this.listWhere.Add("and");
                }
                if (expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.AndAlso)
                {
                    GengerateWhere((BinaryExpression)expression.Right);
                }
                if (expression.NodeType == ExpressionType.OrElse)
                {
                    this.listWhere.Add(")");
                }
            }
        }
        public string GenerateQuery(Expression<Func<TEntity, bool>> expression)
        {
            GengerateWhereClause((BinaryExpression)expression.Body);
            var temp = this.whereClause;
            this.whereClause = null;
            return $"SELECT * FROM {typeof(TEntity).Name} WHERE {temp}";
        }
        public string GenerateQuery(Expression<Func<TEntity, bool>>[] expressions)
        {
            var listWhereClause = new List<string>();
            foreach (var expression in expressions)
            {
                GengerateWhereClause((BinaryExpression)expression.Body);
                if (this.whereClause != null)
                {
                    listWhereClause.Add(this.whereClause);
                    this.whereClause = null;
                }
            }
            var where = string.Join(" and ", listWhereClause);
            this.whereClause = null;
            return $"SELECT * FROM {typeof(TEntity).Name} WHERE {where}";
        }
        public string GenerateQuery(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TEntity>> select)
        {
            var selectStatement = GenerateSelectStatement(select);
            GengerateWhereClause((BinaryExpression)expression.Body);
            var where = this.whereClause;
            this.whereClause = null;
            return $"SELECT {selectStatement} FROM {typeof(TEntity).Name} WHERE {where}";
        }
        public string GenerateQuery(Expression<Func<TEntity, bool>>[] expressions, Expression<Func<TEntity, TEntity>> select)
        {
            var selectStatement = GenerateSelectStatement(select);
            var listWhereClause = new List<string>();
            foreach (var expression in expressions)
            {
                GengerateWhereClause((BinaryExpression)expression.Body);
                if (this.whereClause != null)
                {
                    listWhereClause.Add(this.whereClause);
                    this.whereClause = null;
                }
            }
            this.whereClause = null;
            var where = string.Join(" and ", listWhereClause);
            return $"SELECT {selectStatement} FROM {typeof(TEntity).Name} WHERE {where}";
        }
        public GenerateSQL<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            if (this.listWhere.Count > 0)
            {
                this.listWhere.Add("and");
            }
            GengerateWhere((BinaryExpression)expression.Body);
            return this;
        }
        public GenerateSQL<TEntity> Where(Expression<Func<TEntity, bool>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                if (this.listWhere.Count > 0)
                {
                    this.listWhere.Add("and");
                }
                GengerateWhere((BinaryExpression)expression.Body);
                
            }
            return this;
        }
        public GenerateSQL<TEntity> Select(Expression<Func<TEntity, TEntity>> select)
        {
            var list = GenerateSelect(select);
            this.listSelect.AddRange(list);
            this.listSelect = this.listSelect.Distinct().ToList();
            return this;
        }

        public string ToQuery()
        {
            this.query = @$"SELECT [[selectStatement]] FROM {typeof(TEntity).Name} [[WHERE]] [[whereClause]]";
            this.select = string.Join(", ", this.listSelect);
            this.where = string.Join(" ", this.listWhere);
            if (!string.IsNullOrEmpty(this.select))
            {
                this.query = this.query.Replace("[[selectStatement]]", this.select);
            }
            else
            {
                this.query = this.query.Replace("[[selectStatement]]", "*");
            }
            if (!string.IsNullOrEmpty(this.where))
            {
                this.query = this.query.Replace("[[WHERE]]", "WHERE");
                this.query = this.query.Replace("[[whereClause]]", this.where);
            }
            else
            {
                this.query = this.query.Replace("[[WHERE]]", string.Empty);
                this.query = this.query.Replace("[[whereClause]]", string.Empty);
            }
            var temp = this.query.Trim();
            this.select = null;
            this.where = null;
            this.listSelect.Clear();
            this.listWhere.Clear();
            this.query = null;
            return temp;
        }
    }
}
