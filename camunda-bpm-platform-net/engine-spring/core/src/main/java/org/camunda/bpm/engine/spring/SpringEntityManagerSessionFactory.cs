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
namespace org.camunda.bpm.engine.spring
{

	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;
	using EntityManagerSession = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSession;
	using EntityManagerSessionImpl = org.camunda.bpm.engine.impl.variable.serializer.jpa.EntityManagerSessionImpl;
	using EntityManagerFactoryUtils = org.springframework.orm.jpa.EntityManagerFactoryUtils;


	/// <summary>
	/// Session Factory for <seealso cref="EntityManagerSession"/>.
	/// 
	/// Must be used when the <seealso cref="EntityManagerFactory"/> is managed by Spring.
	/// This implementation will retrieve the <seealso cref="EntityManager"/> bound to the
	/// thread by Spring in case a transaction already started.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class SpringEntityManagerSessionFactory : SessionFactory
	{

	  protected internal EntityManagerFactory entityManagerFactory;
	  protected internal bool handleTransactions;
	  protected internal bool closeEntityManager;

	  public SpringEntityManagerSessionFactory(object entityManagerFactory, bool handleTransactions, bool closeEntityManager)
	  {
		this.entityManagerFactory = (EntityManagerFactory) entityManagerFactory;
		this.handleTransactions = handleTransactions;
		this.closeEntityManager = closeEntityManager;
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(EntityManagerFactory);
		  }
	  }

	  public virtual Session openSession()
	  {
		EntityManager entityManager = EntityManagerFactoryUtils.getTransactionalEntityManager(entityManagerFactory);
		if (entityManager == null)
		{
		  return new EntityManagerSessionImpl(entityManagerFactory, handleTransactions, closeEntityManager);
		}
		return new EntityManagerSessionImpl(entityManagerFactory, entityManager, false, false);
	  }

	}

}