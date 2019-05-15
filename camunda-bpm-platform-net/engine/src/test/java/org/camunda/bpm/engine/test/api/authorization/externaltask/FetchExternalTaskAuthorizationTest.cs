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
namespace org.camunda.bpm.engine.test.api.authorization.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FetchExternalTaskAuthorizationTest : AuthorizationTest
	{

	  public const string WORKER_ID = "workerId";
	  public const long LOCK_TIME = 10000L;

	  protected internal new string deploymentId;

	  protected internal string instance1Id;
	  protected internal string instance2Id;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/externaltask/twoExternalTaskProcess.bpmn20.xml").Id;

		instance1Id = startProcessInstanceByKey("oneExternalTaskProcess").Id;
		instance2Id = startProcessInstanceByKey("twoExternalTaskProcess").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testFetchWithoutAuthorization()
	  {
		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

	  public virtual void testFetchWithReadOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, READ);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

	  public virtual void testFetchWithUpdateOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, READ);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

	  public virtual void testFetchWithReadAndUpdateOnProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, READ, UPDATE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(1, tasks.Count);
		assertEquals(instance1Id, tasks[0].ProcessInstanceId);
	  }

	  public virtual void testFetchWithReadInstanceOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "oneExternalTaskProcess", userId, READ_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

	  public virtual void testFetchWithUpdateInstanceOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "oneExternalTaskProcess", userId, UPDATE_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(0, tasks.Count);
	  }

	  public virtual void testFetchWithReadAndUpdateInstanceOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, "oneExternalTaskProcess", userId, READ_INSTANCE, UPDATE_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(1, tasks.Count);
		assertEquals(instance1Id, tasks[0].ProcessInstanceId);
	  }

	  public virtual void testFetchWithReadOnProcessInstanceAndUpdateInstanceOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, READ);
		createGrantAuthorization(PROCESS_DEFINITION, "oneExternalTaskProcess", userId, UPDATE_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(1, tasks.Count);
		assertEquals(instance1Id, tasks[0].ProcessInstanceId);
	  }

	  public virtual void testFetchWithUpdateOnProcessInstanceAndReadInstanceOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, UPDATE);
		createGrantAuthorization(PROCESS_DEFINITION, "oneExternalTaskProcess", userId, READ_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(1, tasks.Count);
		assertEquals(instance1Id, tasks[0].ProcessInstanceId);
	  }

	  public virtual void testFetchWithReadAndUpdateOnAnyProcessInstance()
	  {
		// given
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ, UPDATE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testQueryWithReadAndUpdateInstanceOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE, UPDATE_INSTANCE);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testQueryWithReadProcessInstanceAndUpdateInstanceOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, instance1Id, userId, READ);

		// when
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(5, WORKER_ID).topic("externalTaskTopic", LOCK_TIME).execute();

		// then
		assertEquals(1, tasks.Count);
		assertEquals(instance1Id, tasks[0].ProcessInstanceId);
	  }

	}

}