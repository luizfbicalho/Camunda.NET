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
namespace org.camunda.bpm.integrationtest.functional.jpa
{
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;



	/// <summary>
	/// <para>Checks that activiti / application transaction sharing works as expected</para>
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JPAIntegrationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JPAIntegrationTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(SomeEntity)).addClass(typeof(PersistenceDelegateBean)).addClass(typeof(AsyncPersistenceDelegateBean)).addAsResource("org/camunda/bpm/integrationtest/functional/jpa/TransactionIntegrationTest.testDelegateParticipateInApplicationTx.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/jpa/TransactionIntegrationTest.testAsyncDelegateNewTx.bpmn20.xml").addAsWebInfResource("persistence.xml", "classes/META-INF/persistence.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private javax.transaction.UserTransaction utx;
	  private UserTransaction utx;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
	  private new RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PersistenceContext private javax.persistence.EntityManager entityManager;
	  private EntityManager entityManager;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private PersistenceDelegateBean persistenceDelegateBean;
	  private PersistenceDelegateBean persistenceDelegateBean;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private AsyncPersistenceDelegateBean asyncPersistenceDelegateBean;
	  private AsyncPersistenceDelegateBean asyncPersistenceDelegateBean;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegateParticipateInApplicationTx() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDelegateParticipateInApplicationTx()
	  {

		/* if we start a transaction here, persist an entity and then
		 * start a process instance which synchronously invokes a java delegate,
		 * that delegate is invoked in the same transaction and thus has access to
		 * the same entity manager.
		 */

		try
		{
		  utx.begin();

		  SomeEntity e = new SomeEntity();
		  entityManager.persist(e);

		  persistenceDelegateBean.Entity = e;

		  runtimeService.startProcessInstanceByKey("testDelegateParticipateInApplicationTx");

		  utx.commit();
		}
		catch (Exception e)
		{
		  utx.rollback();
		  throw e;
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAsyncDelegateNewTx() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testAsyncDelegateNewTx()
	  {

		/* if we start a transaction here, persist an entity and then
		 * start a process instance which asynchronously invokes a java delegate,
		 * that delegate is invoked in a new transaction and thus does not have access to
		 * the same entity manager.
		 */

		try
		{
		  utx.begin();

		  SomeEntity e = new SomeEntity();
		  entityManager.persist(e);

		  asyncPersistenceDelegateBean.Entity = e;

		  runtimeService.startProcessInstanceByKey("testAsyncDelegateNewTx");

		  utx.commit();

		}
		catch (Exception e)
		{
		  utx.rollback();
		  throw e;
		}

		waitForJobExecutorToProcessAllJobs();

	  }
	}

}