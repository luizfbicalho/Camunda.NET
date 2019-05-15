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
namespace org.camunda.bpm.qa.performance.engine.util
{
	using Cursor = org.apache.ibatis.cursor.Cursor;
	using BatchResult = org.apache.ibatis.executor.BatchResult;
	using Configuration = org.apache.ibatis.session.Configuration;
	using ResultHandler = org.apache.ibatis.session.ResultHandler;
	using RowBounds = org.apache.ibatis.session.RowBounds;
	using SqlSession = org.apache.ibatis.session.SqlSession;


	/// <summary>
	/// <para>Implementation of <seealso cref="SqlSession"/> delegating to a wrapped session</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class DelegatingSqlSession : SqlSession
	{

	  protected internal SqlSession wrappedSession;

	  public DelegatingSqlSession(SqlSession wrappedSession)
	  {
		this.wrappedSession = wrappedSession;
	  }

	  public virtual T selectOne<T>(string statement)
	  {
		return wrappedSession.selectOne(statement);
	  }

	  public virtual T selectOne<T>(string statement, object parameter)
	  {
		return wrappedSession.selectOne(statement, parameter);
	  }

	  public virtual IList<E> selectList<E>(string statement)
	  {
		return wrappedSession.selectList(statement);
	  }

	  public virtual IList<E> selectList<E>(string statement, object parameter)
	  {
		return wrappedSession.selectList(statement, parameter);
	  }

	  public virtual IList<E> selectList<E>(string statement, object parameter, RowBounds rowBounds)
	  {
		return wrappedSession.selectList(statement, parameter, rowBounds);
	  }

	  public virtual IDictionary<K, V> selectMap<K, V>(string statement, string mapKey)
	  {
		return wrappedSession.selectMap(statement, mapKey);
	  }

	  public virtual IDictionary<K, V> selectMap<K, V>(string statement, object parameter, string mapKey)
	  {
		return wrappedSession.selectMap(statement, parameter, mapKey);
	  }

	  public virtual IDictionary<K, V> selectMap<K, V>(string statement, object parameter, string mapKey, RowBounds rowBounds)
	  {
		return wrappedSession.selectMap(statement, parameter, mapKey, rowBounds);
	  }

	  public override Cursor<T> selectCursor<T>(string s)
	  {
		return wrappedSession.selectCursor(s);
	  }

	  public override Cursor<T> selectCursor<T>(string s, object o)
	  {
		return wrappedSession.selectCursor(s, o);
	  }

	  public override Cursor<T> selectCursor<T>(string s, object o, RowBounds rowBounds)
	  {
		return wrappedSession.selectCursor(s, o, rowBounds);
	  }

	  public virtual void select(string statement, object parameter, ResultHandler handler)
	  {
		wrappedSession.select(statement, parameter, handler);
	  }

	  public virtual void select(string statement, ResultHandler handler)
	  {
		wrappedSession.select(statement, handler);
	  }

	  public virtual void select(string statement, object parameter, RowBounds rowBounds, ResultHandler handler)
	  {
		wrappedSession.select(statement, parameter, rowBounds, handler);
	  }

	  public virtual int insert(string statement)
	  {
		return wrappedSession.insert(statement);
	  }

	  public virtual int insert(string statement, object parameter)
	  {
		return wrappedSession.insert(statement, parameter);
	  }

	  public virtual int update(string statement)
	  {
		return wrappedSession.update(statement);
	  }

	  public virtual int update(string statement, object parameter)
	  {
		return wrappedSession.update(statement, parameter);
	  }

	  public virtual int delete(string statement)
	  {
		return wrappedSession.delete(statement);
	  }

	  public virtual int delete(string statement, object parameter)
	  {
		return wrappedSession.delete(statement, parameter);
	  }

	  public virtual void commit()
	  {
		wrappedSession.commit();
	  }

	  public virtual void commit(bool force)
	  {
		wrappedSession.commit(force);
	  }

	  public virtual void rollback()
	  {
		wrappedSession.rollback();
	  }

	  public virtual void rollback(bool force)
	  {
		wrappedSession.rollback(force);
	  }

	  public virtual IList<BatchResult> flushStatements()
	  {
		return wrappedSession.flushStatements();
	  }

	  public virtual void close()
	  {
		wrappedSession.close();
	  }

	  public virtual void clearCache()
	  {
		wrappedSession.clearCache();
	  }

	  public virtual Configuration Configuration
	  {
		  get
		  {
			return wrappedSession.Configuration;
		  }
	  }

	  public virtual T getMapper<T>(Type<T> type)
	  {
		return wrappedSession.getMapper(type);
	  }

	  public virtual Connection Connection
	  {
		  get
		  {
			return wrappedSession.Connection;
		  }
	  }


	}

}