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
namespace org.camunda.bpm.engine.impl.db.entitymanager
{
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DbEntityManagerFactory : SessionFactory
	{

	  protected internal IdGenerator idGenerator;

	  public DbEntityManagerFactory(IdGenerator idGenerator)
	  {
		this.idGenerator = idGenerator;
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(DbEntityManager);
		  }
	  }

	  public virtual DbEntityManager openSession()
	  {
		PersistenceSession persistenceSession = Context.CommandContext.getSession(typeof(PersistenceSession));
		return new DbEntityManager(idGenerator, persistenceSession);
	  }

	}

}