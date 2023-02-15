using FreeSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Wombat.Infrastructure
{
    public abstract class IDBRepository<T> where T : class, new()
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db">注入数据库</param>
        public IDBRepository(IFreeSql db)
        {
            Db = db;
        }

        #endregion

        #region 私有成员

        protected virtual string _valueField { get; } = "Id";
        protected virtual string _textField { get => throw new Exception("请在子类重写"); }

        #endregion

        #region 外部属性

        /// <summary>
        /// 业务仓储接口(支持软删除),支持联表操作
        /// 注：若需要访问逻辑删除的数据,请使用IDbAccessor.FullRepository
        /// 注：仅支持单线程操作
        /// </summary>
        public IFreeSql Db { get; }

        #endregion

        #region 事物提交

        public (bool Success, Exception ex) RunTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            //using Object<DbConnection> conn = Db.Ado.MasterPool.Get();
            //using DbTransaction transaction = conn.Value.BeginTransaction();
            using (var uow = Db.CreateUnitOfWork())
            {
                try
                {
                    //指定事务级别
                    uow.IsolationLevel = isolationLevel;
                    //开启事务
                    var tran = uow.GetOrBeginTransaction();
                    //执行逻辑
                    action();

                    //提交事务
                    uow.Commit();

                    return (true, null);
                }
                catch (Exception ex)
                {
                    //回滚事务
                    uow.Rollback();
                    //抛出异常
                    throw ex;
                }

            }
        }
        public async Task<(bool Success, Exception ex)> RunTransactionAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using (var uow = Db.CreateUnitOfWork())
            {
                try
                {
                    //指定事务级别
                    uow.IsolationLevel = isolationLevel;
                    //开启事务
                    var tran = uow.GetOrBeginTransaction();
                    //执行逻辑
                    await action();

                    //提交事务
                    uow.Commit();

                    return (true, null);
                }
                catch (Exception ex)
                {
                    //回滚事务
                    uow.Rollback();
                    throw ex;
                }

            }



            //using (Object<DbConnection> conn = await Db.Ado.MasterPool.GetAsync())
            //{

            //    using DbTransaction transaction = await conn.Value.BeginTransactionAsync();
            //    try
            //    {
            //        await action();

            //        //提交事务
            //        transaction.Commit();

            //        return (true, null);
            //    }
            //    catch (Exception ex)
            //    {
            //        //回滚事务
            //        transaction.Rollback();
            //        //抛出异常
            //        return (false, ex);
            //    }

            //}
        }

        #endregion

        #region 增加数据

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public int Insert(T entity)
        {
            return Db.Insert(entity).ExecuteAffrows();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public async Task<int> InsertAsync(T entity)
        {
            return await Db.Insert(entity).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 添加多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public int Insert(List<T> entities)
        {
            return Db.Insert(entities).ExecuteAffrows();
        }

        /// <summary>
        /// 添加多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public async Task<int> InsertAsync(List<T> entities)
        {
            return await Db.Insert(entities).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 批量添加数据,速度快
        /// </summary>
        /// <param name="entities"></param>
        public void BulkInsert(List<T> entities)
        {
            Db.Insert(entities);
        }

        #endregion

        #region 删除数据

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public int DeleteAll()
        {
            var list = Db.Select<T>().ToList();
            return Db.Delete<T>(list).ExecuteAffrows();
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        public async Task<int> DeleteAllAsync()
        {
            var list = Db.Select<T>().ToList();

            return await Db.Delete<T>(list).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public int Delete(string key)
        {
            return Db.Delete<T>(key).ExecuteAffrows();
        }

        /// <summary>
        /// 删除指定主键数据
        /// </summary>
        /// <param name="key"></param>
        public async Task<int> DeleteAsync(string key)
        {
            return await Db.Delete<T>(key).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public int Delete(List<string> keys)
        {
            return Db.Delete<T>(keys).ExecuteAffrows();
        }

        /// <summary>
        /// 通过主键删除多条数据
        /// </summary>
        /// <param name="keys"></param>
        public async Task<int> DeleteAsync(List<string> keys)
        {
            return await Db.Delete<T>(keys).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public int Delete(T entity)
        {
            return Db.Delete<T>(entity).ExecuteAffrows();
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public async Task<int> DeleteAsync(T entity)
        {
            return await Db.Delete<T>(entity).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public int Delete(List<T> entities)
        {
            return Db.Delete<T>(entities).ExecuteAffrows();
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        public async Task<int> DeleteAsync(List<T> entities)
        {
            return await Db.Delete<T>(entities).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除指定条件数据
        /// </summary>
        /// <param name="condition">筛选条件</param>
        public int Delete(Expression<Func<T, bool>> condition)
        {
            return Db.Delete<T>().Where(condition).ExecuteAffrows();
        }

        /// <summary>
        /// 删除指定条件数据
        /// </summary>
        /// <param name="condition">筛选条件</param>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> condition)
        {
            return await Db.Delete<T>().Where(condition).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 使用SQL语句按照条件删除数据
        /// 用法:Delete_Sql"Base_User"(x=&gt;x.Id == "Admin")
        /// 注：生成的SQL类似于DELETE FROM [Base_User] WHERE [Name] = 'xxx' WHERE [Id] = 'Admin'
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>
        /// 影响条数
        /// </returns>
        public int DeleteSql(Expression<Func<T, bool>> where)
        {
            return Db.Select<T>().Where(where).ToDelete().ExecuteAffrows();
        }


        /// <summary>
        /// 使用SQL语句按照条件删除数据
        /// 用法:Delete_Sql"Base_User"(x=&gt;x.Id == "Admin")
        /// 注：生成的SQL类似于DELETE FROM [Base_User] WHERE [Name] = 'xxx' WHERE [Id] = 'Admin'
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>
        /// 影响条数
        /// </returns>
        public async Task<int> DeleteSqlAsync(Expression<Func<T, bool>> where)
        {
            return await Db.Select<T>().Where(where).ToDelete().ExecuteAffrowsAsync();
        }

        #endregion

        #region 更新数据

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public int Update(T entity)
        {
            return Db.Update<T>(entity).SetSource(entity).ExecuteAffrows();
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="entity">实体对象</param>
        public async Task<int> UpdateAsync(T entity)
        {
            return await Db.Update<T>(entity).SetSource(entity).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities">数据列表</param>
        public int Update(List<T> entities)
        {
            return Db.Update<T>(entities).SetSource(entities).ExecuteAffrows();
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities">数据列表</param>
        public async Task<int> UpdateAsync(List<T> entities)
        {
            return await Db.Update<T>(entities).SetSource(entities).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 指定条件更新
        /// </summary>
        /// <param name="whereExpre">筛选表达式</param>
        /// <param name="set">更改属性回调</param>
        public int Update(Expression<Func<T, bool>> whereExpre)
        {
            return Db.Select<T>().Where(whereExpre).ToUpdate().ExecuteAffrows();
        }

        /// <summary>
        /// 指定条件更新
        /// </summary>
        /// <param name="whereExpre">筛选表达式</param>
        /// <param name="set">更改属性回调</param>
        public async Task<int> UpdateAsync(Expression<Func<T, bool>> whereExpre)
        {
            return await Db.Select<T>().Where(whereExpre).ToUpdate().ExecuteAffrowsAsync();
        }

        ///// <summary>
        ///// 使用SQL语句按照条件更新
        ///// 用法:UpdateWhere_Sql"Base_User"(x=>x.Id == "Admin",("Name","小明"))
        ///// 注：生成的SQL类似于UPDATE [TABLE] SET [Name] = 'xxx' WHERE [Id] = 'Admin'
        ///// </summary>
        ///// <param name="where">筛选条件</param>
        ///// <param name="values">字段值设置</param>
        ///// <returns>影响条数</returns>
        //public int UpdateSql(Expression<Func<T, bool>> where, params (string field, object value)[] values)
        //{
        //    return Db.Select<T>().Where(where).WithSql(values).ToUpdate().ExecuteAffrows();
        //}

        ///// <summary>
        ///// 使用SQL语句按照条件更新
        ///// 用法:UpdateWhere_Sql"Base_User"(x=>x.Id == "Admin",("Name","小明"))
        ///// 注：生成的SQL类似于UPDATE [TABLE] SET [Name] = 'xxx' WHERE [Id] = 'Admin'
        ///// </summary>
        ///// <param name="where">筛选条件</param>
        ///// <param name="values">字段值设置</param>
        ///// <returns>影响条数</returns>
        //public async Task<int> UpdateSqlAsync(Expression<Func<T, bool>> where, params (string field, UpdateType updateType, object value)[] values)
        //{
        //    return await Db.UpdateSqlAsync(where, values);
        //}

        #endregion

        #region 查询数据

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public T GetEntity(params object[] keyValue)
        {
            return Db.Select<T>(keyValue).ToOne();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<T> GetEntityAsync(params object[] keyValue)
        {
            return await Db.Select<T>(keyValue).ToOneAsync();
        }

        /// <summary>
        /// 获取表的所有数据，当数据量很大时不要使用！
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            return Db.Select<T>().ToList();
        }

        /// <summary>
        /// 获取表的所有数据，当数据量很大时不要使用！
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync()
        {
            return await Db.Select<T>().ToListAsync();
        }

        /// <summary>
        /// 获取表的所有数据，当数据量很大时不要使用！
        /// </summary>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> whereExpre)
        {
            return Db.Select<T>().Where(whereExpre).ToList();
        }

        /// <summary>
        /// 获取表的所有数据，当数据量很大时不要使用！
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpre)
        {
            return await Db.Select<T>().Where(whereExpre).ToListAsync();
        }



        ///// <summary>
        ///// 获取实体对应的表，延迟加载，主要用于支持Linq查询操作
        ///// </summary>
        ///// <returns></returns>
        public virtual ISelect<T> GetISelect()
        {
            return Db.Select<T>();
        }

        ///// <summary>
        ///// 获取实体对应的表，延迟加载，主要用于支持Linq查询操作
        ///// </summary>
        ///// <returns></returns>
        public virtual IQueryable GetIQueryable()
        {
            return Db.Select<T>().ToList().AsQueryable();
        }




        /// <summary>
        /// 获取实体对应的表，延迟加载，主要用于支持Linq查询操作
        /// </summary>
        /// <returns></returns>
        public virtual T GetOne(Expression<Func<T, bool>> whereExpre)
        {
            return Db.Select<T>().Where(whereExpre).ToOne();
        }



        /// <summary>
        /// 获取实体对应的表，延迟加载，主要用于支持Linq查询操作
        /// </summary>
        /// <returns></returns>
        public async virtual Task<T> GetOneAsync(Expression<Func<T, bool>> whereExpre)
        {
            return await Db.Select<T>().Where(whereExpre).ToOneAsync();
        }


        #endregion

        #region 执行Sql语句

        #endregion



    }
}
