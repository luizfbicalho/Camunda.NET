﻿using System;

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
namespace org.camunda.bpm.engine.impl.variable.serializer.jpa
{
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using SessionFactory = org.camunda.bpm.engine.impl.interceptor.SessionFactory;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class EntityManagerSessionFactory : SessionFactory
	{

	  protected internal EntityManagerFactory entityManagerFactory;
	  protected internal bool handleTransactions;
	  protected internal bool closeEntityManager;

	  public EntityManagerSessionFactory(object entityManagerFactory, bool handleTransactions, bool closeEntityManager)
	  {
		ensureNotNull("entityManagerFactory", entityManagerFactory);
		if (!(entityManagerFactory is EntityManagerFactory))
		{
		  throw new ProcessEngineException("EntityManagerFactory must implement 'javax.persistence.EntityManagerFactory'");
		}

		this.entityManagerFactory = (EntityManagerFactory) entityManagerFactory;
		this.handleTransactions = handleTransactions;
		this.closeEntityManager = closeEntityManager;
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(EntityManagerSession);
		  }
	  }

	  public virtual Session openSession()
	  {
		return new EntityManagerSessionImpl(entityManagerFactory, handleTransactions, closeEntityManager);
	  }

	  public virtual EntityManagerFactory EntityManagerFactory
	  {
		  get
		  {
			return entityManagerFactory;
		  }
	  }
	}

}