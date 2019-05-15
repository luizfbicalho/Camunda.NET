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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageStartEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // historic activity instance query /////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testSimpleQueryMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // historic activity instance query (multiple process instances) ////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 6);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 14);
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		enableAuthorization();

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		// when
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery();

		// then
		verifyQueryResults(query, 9);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricActivityInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}