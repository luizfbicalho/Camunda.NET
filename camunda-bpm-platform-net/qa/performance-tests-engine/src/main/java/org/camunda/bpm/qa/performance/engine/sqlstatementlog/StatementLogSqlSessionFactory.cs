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
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using SqlSessionFactory = org.apache.ibatis.session.SqlSessionFactory;
	using DelegatingSqlSessionFactory = org.camunda.bpm.qa.performance.engine.util.DelegatingSqlSessionFactory;

	/// <summary>
	/// <seealso cref="DelegatingSqlSessionFactory"/> wrapping the created sessions using a {@link StatementLogSqlSession.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StatementLogSqlSessionFactory : DelegatingSqlSessionFactory
	{

	  public StatementLogSqlSessionFactory(SqlSessionFactory wrappSqlSessionFactory) : base(wrappSqlSessionFactory)
	  {
	  }

	  public override SqlSession openSession()
	  {
		return new StatementLogSqlSession(base.openSession());
	  }

	}

}