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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using EventSubscriptionQuery = org.camunda.bpm.engine.runtime.EventSubscriptionQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class EventSubscriptionAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
	  protected internal const string SIGNAL_BOUNDARY_PROCESS_KEY = "signalBoundaryProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneMessageBoundaryEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/signalBoundaryEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);

		EventSubscription eventSubscription = query.singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processInstanceId, eventSubscription.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);

		EventSubscription eventSubscription = query.singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processInstanceId, eventSubscription.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);

		EventSubscription eventSubscription = query.singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processInstanceId, eventSubscription.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);

		EventSubscription eventSubscription = query.singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processInstanceId, eventSubscription.ProcessInstanceId);
	  }

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;

		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 1);

		EventSubscription eventSubscription = query.singleResult();
		assertNotNull(eventSubscription);
		assertEquals(processInstanceId, eventSubscription.ProcessInstanceId);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(SIGNAL_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		EventSubscriptionQuery query = runtimeService.createEventSubscriptionQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  protected internal virtual void verifyQueryResults(EventSubscriptionQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}