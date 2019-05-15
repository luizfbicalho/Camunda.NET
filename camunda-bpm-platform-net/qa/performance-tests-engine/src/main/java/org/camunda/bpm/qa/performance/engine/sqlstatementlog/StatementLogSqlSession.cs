using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.qa.performance.engine.sqlstatementlog
{

	using ResultHandler = org.apache.ibatis.session.ResultHandler;
	using RowBounds = org.apache.ibatis.session.RowBounds;
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using DelegatingSqlSession = org.camunda.bpm.qa.performance.engine.util.DelegatingSqlSession;
	using JsonUtil = org.camunda.bpm.qa.performance.engine.util.JsonUtil;
	using JsonGenerationException = org.codehaus.jackson.JsonGenerationException;
	using JsonMappingException = org.codehaus.jackson.map.JsonMappingException;
	using SerializationConfig = org.codehaus.jackson.map.SerializationConfig;
	using Feature = org.codehaus.jackson.map.SerializationConfig.Feature;
	using Inclusion = org.codehaus.jackson.map.annotate.JsonSerialize.Inclusion;

	/// <summary>
	/// <para>This SqlSession wraps an actual SqlSession and logs executed sql statements. (Calls to the
	/// delete*, update*, select*, insert* methods.)</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StatementLogSqlSession : DelegatingSqlSession
	{

	  protected internal static ThreadLocal<IList<SqlStatementLog>> threadStatementLog = new ThreadLocal<IList<SqlStatementLog>>();

	  public StatementLogSqlSession(SqlSession wrappedSession) : base(wrappedSession)
	  {
	  }

	  // statement interceptors ///////////////////////////////////

	  public override int delete(string statement)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.delete(statement);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.DELETE, null, statement, duration);
		return result;
	  }

	  public override int delete(string statement, object parameter)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.delete(statement, parameter);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.DELETE, parameter, statement, duration);
		return result;
	  }

	  public override int insert(string statement)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.insert(statement);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.INSERT, null, statement, duration);
		return result;
	  }

	  public override int insert(string statement, object paremeter)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.insert(statement, paremeter);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.INSERT, paremeter, statement, duration);
		return result;
	  }

	  public override int update(string statement)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.update(statement);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.UPDATE, null, statement, duration);
		return result;
	  }

	  public override int update(string statement, object parameter)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		int result = base.update(statement, parameter);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.UPDATE, parameter, statement, duration);
		return result;
	  }

	  public override void select(string statement, object parameter, ResultHandler handler)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		base.select(statement, parameter, handler);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT, parameter, statement, duration);
	  }

	  public override void select(string statement, object parameter, RowBounds rowBounds, ResultHandler handler)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		base.select(statement, parameter, rowBounds, handler);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT, parameter, statement, duration);
	  }

	  public override void select(string statement, ResultHandler handler)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		base.select(statement, handler);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT, null, statement, duration);
	  }

	  public override IList<E> selectList<E>(string statement)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IList<E> result = base.selectList(statement);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_LIST, null, statement, duration);

		return result;
	  }

	  public override IList<E> selectList<E>(string statement, object parameter)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IList<E> result = base.selectList(statement, parameter);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_LIST, parameter, statement, duration);

		return result;
	  }

	  public override IList<E> selectList<E>(string statement, object parameter, RowBounds rowBounds)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IList<E> result = base.selectList(statement, parameter, rowBounds);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_LIST, parameter, statement, duration);

		return result;
	  }

	  public override IDictionary<K, V> selectMap<K, V>(string statement, object parameter, string mapKey)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IDictionary<K, V> result = base.selectMap(statement, parameter, mapKey);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_MAP, parameter, statement, duration);

		return result;
	  }

	  public override IDictionary<K, V> selectMap<K, V>(string statement, object parameter, string mapKey, RowBounds rowBounds)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IDictionary<K, V> result = base.selectMap(statement, parameter, mapKey, rowBounds);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_MAP, parameter, statement, duration);

		return result;
	  }

	  public override IDictionary<K, V> selectMap<K, V>(string statement, string mapKey)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		IDictionary<K, V> result = base.selectMap(statement, mapKey);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_MAP, mapKey, statement, duration);

		return result;
	  }

	  public override T selectOne<T>(string statement)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		T result = base.selectOne(statement);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_MAP, null, statement, duration);

		return result;
	  }

	  public override T selectOne<T>(string statement, object parameter)
	  {
		long start = DateTimeHelper.CurrentUnixTimeMillis();

		T result = base.selectOne(statement, parameter);

		long duration = DateTimeHelper.CurrentUnixTimeMillis() - start;
		logStatement(SqlStatementType.SELECT_MAP, parameter, statement, duration);

		return result;
	  }

	  // logging ////////////////////////////////////////////////

	  protected internal virtual void logStatement(SqlStatementType type, object parameters, string statement, long duration)
	  {
		IList<SqlStatementLog> log = threadStatementLog.get();
		if (log != null)
		{
		  log.Add(new SqlStatementLog(type, parameters, statement, duration));
		}
	  }

	  /// <summary>
	  /// stops logging statement executed by the current thread and returns the list of logged statements. </summary>
	  /// <returns> the <seealso cref="List"/> of logged sql statements </returns>
	  public static IList<SqlStatementLog> stopLogging()
	  {
		IList<SqlStatementLog> log = threadStatementLog.get();
		threadStatementLog.remove();
		return log;
	  }

	  /// <summary>
	  /// starts logging any statements executed by the calling thread.
	  /// </summary>
	  public static void startLogging()
	  {
		threadStatementLog.set(new List<StatementLogSqlSession.SqlStatementLog>());
	  }

	  // log classes //////////////////////////////////////

	  public class SqlStatementLog
	  {

		protected internal SqlStatementType statementType;

		/// <summary>
		/// the statement (sql string) </summary>
		protected internal string statement;

		/// <summary>
		/// the duration the statement took to execute in Milliseconds </summary>
		protected internal long durationMs;

		protected internal string statementParameters;

		public SqlStatementLog(SqlStatementType type, object parameters, string statement, long duration)
		{
		  statementType = type;
		  this.statement = statement;
		  this.durationMs = duration;
		  try
		  {
			statementParameters = JsonUtil.Mapper.writeValueAsString(parameters).replaceAll("\"", "'");
		  }
		  catch (Exception)
		  {
	//        e.printStackTrace();
		  }
		}

		public virtual string Statement
		{
			get
			{
			  return statement;
			}
		}

		public virtual SqlStatementType StatementType
		{
			get
			{
			  return statementType;
			}
		}

		public virtual long DurationMs
		{
			get
			{
			  return durationMs;
			}
		}

		public virtual string StatementParameters
		{
			get
			{
			  return statementParameters;
			}
		}

	  }

	  public enum SqlStatementType
	  {
		SELECT,
		SELECT_ONE,
		SELECT_LIST,
		SELECT_MAP,
		INSERT,
		UPDATE,
		DELETE
	  }

	}

}