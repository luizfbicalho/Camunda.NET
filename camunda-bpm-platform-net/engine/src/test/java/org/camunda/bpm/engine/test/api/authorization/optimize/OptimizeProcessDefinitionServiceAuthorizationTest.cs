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
namespace org.camunda.bpm.engine.test.api.authorization.optimize
{

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using OptimizeService = org.camunda.bpm.engine.impl.OptimizeService;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class OptimizeProcessDefinitionServiceAuthorizationTest : AuthorizationTest
	{

	  protected internal new string deploymentId;
	  private OptimizeService optimizeService;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {

		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngine.ProcessEngineConfiguration;
		optimizeService = config.OptimizeService;

		DeploymentBuilder deploymentbuilder = repositoryService.createDeployment();
		BpmnModelInstance defaultModel = Bpmn.createExecutableProcess("process").startEvent().endEvent().done();
		deploymentId = deployment(deploymentbuilder, defaultModel);

		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testGetCompletedActivitiesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getCompletedHistoricActivityInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}

	  }

	  public virtual void testGetCompletedActivitiesWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(new DateTime(0L), null, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(2));
	  }

	  public virtual void testGetRunningActivitiesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getRunningHistoricActivityInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}

	  }

	  public virtual void testGetRunningActivitiesWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricActivityInstance> runningHistoricActivityInstances = optimizeService.getRunningHistoricActivityInstances(new DateTime(0L), null, 10);

		// then
		assertThat(runningHistoricActivityInstances.Count, @is(0));
	  }

	  public virtual void testGetCompletedTasksWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getCompletedHistoricTaskInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the tasks");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetCompletedTasksWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricTaskInstance> completedHistoricTaskInstances = optimizeService.getCompletedHistoricTaskInstances(new DateTime(0L), null, 10);

		// then
		assertThat(completedHistoricTaskInstances.Count, @is(0));
	  }

	  public virtual void testGetRunningTasksWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getRunningHistoricTaskInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the tasks");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetRunningTasksWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricTaskInstance> runningHistoricTaskInstances = optimizeService.getRunningHistoricTaskInstances(new DateTime(0L), null, 10);

		// then
		assertThat(runningHistoricTaskInstances.Count, @is(0));
	  }

	  public virtual void testGetOperationsLogWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getHistoricUserOperationLogs(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the logs");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetOperationsLogWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<UserOperationLogEntry> operationLogEntries = optimizeService.getHistoricUserOperationLogs(new DateTime(0L), null, 10);

		// then
		assertThat(operationLogEntries.Count, @is(0));
	  }

	  public virtual void testGetCompletedProcessInstancesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getCompletedHistoricProcessInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetCompletedProcessInstancesWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricProcessInstance> completedHistoricProcessInstances = optimizeService.getCompletedHistoricProcessInstances(new DateTime(0L), null, 10);

		// then
		assertThat(completedHistoricProcessInstances.Count, @is(1));
	  }

	  public virtual void testGetRunningProcessInstancesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getRunningHistoricProcessInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetRunningProcessInstancesWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricProcessInstance> runningHistoricProcessInstances = optimizeService.getRunningHistoricProcessInstances(new DateTime(0L), null, 10);

		// then
		assertThat(runningHistoricProcessInstances.Count, @is(0));
	  }

	  public virtual void testGetVariableUpdatesWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");

		try
		{
		  // when
		  optimizeService.getHistoricVariableUpdates(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGetVariableUpdatesWithAuthorization()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<HistoricVariableUpdate> historicVariableUpdates = optimizeService.getHistoricVariableUpdates(new DateTime(0L), null, 10);

		// then
		assertThat(historicVariableUpdates.Count, @is(0));
	  }

	  public virtual void testAuthorizationsOnSingleProcessDefinitionIsNotEnough()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "process", userId, READ_HISTORY);

		try
		{
		  // when
		  optimizeService.getCompletedHistoricActivityInstances(new DateTime(0L), null, 10);
		  fail("Exception expected: It should not be possible to retrieve the activities");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string exceptionMessage = e.Message;
		  assertTextPresent(userId, exceptionMessage);
		  assertTextPresent(READ_HISTORY.Name, exceptionMessage);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), exceptionMessage);
		}
	  }

	  public virtual void testGrantAuthorizationWithAllPermissions()
	  {
		// given
		startProcessInstanceByKey("process");
		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, ALL);
		createGrantAuthorization(PROCESS_INSTANCE, "*", userId, ALL);

		// when
		IList<HistoricActivityInstance> completedHistoricActivityInstances = optimizeService.getCompletedHistoricActivityInstances(new DateTime(0L), null, 10);

		// then
		assertThat(completedHistoricActivityInstances.Count, @is(2));
	  }

	}

}