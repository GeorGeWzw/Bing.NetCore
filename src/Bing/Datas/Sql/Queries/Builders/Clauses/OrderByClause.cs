﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bing.Datas.Sql.Queries.Builders.Abstractions;
using Bing.Datas.Sql.Queries.Builders.Core;
using Bing.Utils.Extensions;

namespace Bing.Datas.Sql.Queries.Builders.Clauses
{
    /// <summary>
    /// 排序子句
    /// </summary>
    public class OrderByClause:IOrderByClause
    {
        /// <summary>
        /// 排序项列表
        /// </summary>
        private readonly List<OrderByItem> _items;

        /// <summary>
        /// Sql方言
        /// </summary>
        private readonly IDialect _dialect;

        /// <summary>
        /// 实体解析器
        /// </summary>
        private readonly IEntityResolver _resolver;

        /// <summary>
        /// 实体别名注册器
        /// </summary>
        private readonly IEntityAliasRegister _register;

        /// <summary>
        /// 初始化一个<see cref="OrderByClause"/>类型的实例
        /// </summary>
        /// <param name="dialect">Sql方言</param>
        /// <param name="resolver">实体解析器</param>
        /// <param name="register">实体别名注册器</param>
        public OrderByClause(IDialect dialect, IEntityResolver resolver, IEntityAliasRegister register)
        {
            _dialect = dialect;
            _resolver = resolver;
            _register = register;
            _items=new List<OrderByItem>();
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="order">排序列表</param>
        public void OrderBy(string order)
        {
            if (string.IsNullOrWhiteSpace(order))
            {
                return;
            }
            _items.AddRange(order.Split(',').Select(column => new OrderByItem(column)));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="column">排序列</param>
        /// <param name="desc">是否倒序</param>
        public void OrderBy<TEntity>(Expression<Func<TEntity, object>> column, bool desc = false)
        {
            if (column == null)
            {
                return;
            }
            _items.Add(new OrderByItem(_resolver.GetColumn(column), desc, typeof(TEntity)));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="order">排序列表</param>
        public void AppendOrderBy(string order)
        {
            _items.Add(new OrderByItem(order, raw: true));
        }

        /// <summary>
        /// 获取Sql
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            if (_items.Count == 0)
            {
                return null;
            }

            return $"Order By {_items.Select(t => t.ToSql(_dialect, _register)).Join()}";
        }
    }
}
