using System;

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
namespace org.camunda.bpm.engine.impl.db.sql
{
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;

	/// <summary>
	/// Provides the <seealso cref="DbSqlSession"/> as <seealso cref="PersistenceSession"/>.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class DbSqlPersistenceProviderFactory : SessionFactory
	{

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(PersistenceSession);
		  }
	  }

	  public virtual Session openSession()
	  {
		return Context.CommandContext.DbSqlSession;
	  }

	}

}