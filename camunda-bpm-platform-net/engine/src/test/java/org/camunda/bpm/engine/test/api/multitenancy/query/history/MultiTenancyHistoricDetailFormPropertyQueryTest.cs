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
namespace org.camunda.bpm.engine.test.api.multitenancy.query.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class MultiTenancyHistoricDetailFormPropertyQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws java.io.IOException
	  protected internal override void setUp()
	  {
		BpmnModelInstance oneTaskProcess = Bpmn.createExecutableProcess("testProcess").startEvent().userTask("userTask").camundaFormField().camundaId("myFormField").camundaType("string").camundaFormFieldDone().endEvent().done();

		deploymentForTenant(TENANT_ONE, oneTaskProcess);
		deploymentForTenant(TENANT_TWO, oneTaskProcess);

		ProcessInstance processInstanceOne = startProcessInstanceForTenant(TENANT_ONE);
		ProcessInstance processInstanceTwo = startProcessInstanceForTenant(TENANT_TWO);

		completeUserTask(processInstanceOne);
		completeUserTask(processInstanceTwo);
	  }

	  public virtual void testQueryWithoutTenantId()
	  {
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByTenantId()
	  {
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = historyService.createHistoricDetailQuery().formFields().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryByTenantIds()
	  {
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));
	  }

	  public virtual void testQueryByNonExistingTenantId()
	  {
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields().tenantIdIn("nonExisting");

		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  historyService.createHistoricDetailQuery().formFields().tenantIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().formFields().orderByTenantId().asc().list();

		assertThat(historicDetails.Count, @is(2));
		assertThat(historicDetails[0].TenantId, @is(TENANT_ONE));
		assertThat(historicDetails[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		IList<HistoricDetail> historicDetails = historyService.createHistoricDetailQuery().formFields().orderByTenantId().desc().list();

		assertThat(historicDetails.Count, @is(2));
		assertThat(historicDetails[0].TenantId, @is(TENANT_TWO));
		assertThat(historicDetails[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		HistoricDetailQuery query = historyService.createHistoricDetailQuery();
		assertThat(query.count(), @is(0L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(2L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		assertThat(query.count(), @is(4L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(2L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		HistoricDetailQuery query = historyService.createHistoricDetailQuery();
		assertThat(query.count(), @is(4L));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceForTenant(string tenant)
	  {
		return runtimeService.createProcessInstanceByKey("testProcess").processDefinitionTenantId(tenant).execute();
	  }

	  protected internal virtual void completeUserTask(ProcessInstance processInstance)
	  {
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertThat(task, @is(notNullValue()));

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["myFormField"] = "myFormFieldValue";
		formService.submitTaskForm(task.Id, properties);
	  }

	}

}