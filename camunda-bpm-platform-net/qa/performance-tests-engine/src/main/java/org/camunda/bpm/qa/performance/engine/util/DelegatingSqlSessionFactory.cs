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

	using Configuration = org.apache.ibatis.session.Configuration;
	using ExecutorType = org.apache.ibatis.session.ExecutorType;
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using TransactionIsolationLevel = org.apache.ibatis.session.TransactionIsolationLevel;

	/// <summary>
	/// <para>Implements the <seealso cref="SqlSessionFactory"/> delegating
	/// to a wrapped <seealso cref="SqlSessionFactory"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DelegatingSqlSessionFactory : SqlSessionFactory
	{

	  protected internal SqlSessionFactory wrappedSessionFactory;

	  public DelegatingSqlSessionFactory(SqlSessionFactory wrappSqlSessionFactory)
	  {
		wrappedSessionFactory = wrappSqlSessionFactory;
	  }

	  public virtual SqlSession openSession()
	  {
		return wrappedSessionFactory.openSession();
	  }

	  public virtual SqlSession openSession(bool autoCommit)
	  {
		return wrappedSessionFactory.openSession(autoCommit);
	  }

	  public virtual SqlSession openSession(Connection connection)
	  {
		return wrappedSessionFactory.openSession(connection);
	  }

	  public virtual SqlSession openSession(TransactionIsolationLevel level)
	  {
		return wrappedSessionFactory.openSession(level);
	  }

	  public virtual SqlSession openSession(ExecutorType execType)
	  {
		return wrappedSessionFactory.openSession(execType);
	  }

	  public virtual SqlSession openSession(ExecutorType execType, bool autoCommit)
	  {
		return wrappedSessionFactory.openSession(execType, autoCommit);
	  }

	  public virtual SqlSession openSession(ExecutorType execType, TransactionIsolationLevel level)
	  {
		return wrappedSessionFactory.openSession(execType, level);
	  }

	  public virtual SqlSession openSession(ExecutorType execType, Connection connection)
	  {
		return wrappedSessionFactory.openSession(execType, connection);
	  }

	  public virtual Configuration Configuration
	  {
		  get
		  {
			return wrappedSessionFactory.Configuration;
		  }
	  }

	}

}