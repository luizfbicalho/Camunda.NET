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
namespace org.camunda.bpm.engine.test.api.multitenancy
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Filter = org.camunda.bpm.engine.filter.Filter;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	public class MultiTenancyFilterServiceTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
	  protected internal static readonly string[] TENANT_IDS = new string[] {TENANT_ONE, TENANT_TWO};

	  protected internal string filterId = null;
	  protected internal readonly IList<string> taskIds = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		createTaskWithoutTenantId();
		createTaskForTenant(TENANT_ONE);
		createTaskForTenant(TENANT_TWO);
	  }

	  public virtual void testCreateFilterWithTenantIdCriteria()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_IDS);
		filterId = createFilter(query);

		Filter savedFilter = filterService.getFilter(filterId);
		TaskQueryImpl savedQuery = savedFilter.Query;

		assertThat(savedQuery.TenantIds, @is(TENANT_IDS));
	  }

	  public virtual void testCreateFilterWithNoTenantIdCriteria()
	  {
		TaskQuery query = taskService.createTaskQuery().withoutTenantId();
		filterId = createFilter(query);

		Filter savedFilter = filterService.getFilter(filterId);
		TaskQueryImpl savedQuery = savedFilter.Query;

		assertThat(savedQuery.TenantIdSet, @is(true));
		assertThat(savedQuery.TenantIds, @is(nullValue()));
	  }

	  public virtual void testFilterTasksNoTenantIdSet()
	  {
		TaskQuery query = taskService.createTaskQuery();
		filterId = createFilter(query);

		assertThat(filterService.count(filterId), @is(3L));
	  }

	  public virtual void testFilterTasksByTenantIds()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_IDS);
		filterId = createFilter(query);

		assertThat(filterService.count(filterId), @is(2L));

		TaskQuery extendingQuery = taskService.createTaskQuery().taskName("testTask");
		assertThat(filterService.count(filterId, extendingQuery), @is(2L));
	  }

	  public virtual void testFilterTasksWithoutTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().withoutTenantId();
		filterId = createFilter(query);

		assertThat(filterService.count(filterId), @is(1L));

		TaskQuery extendingQuery = taskService.createTaskQuery().taskName("testTask");
		assertThat(filterService.count(filterId, extendingQuery), @is(1L));
	  }

	  public virtual void testFilterTasksByExtendingQueryWithTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("testTask");
		filterId = createFilter(query);

		TaskQuery extendingQuery = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);
		assertThat(filterService.count(filterId, extendingQuery), @is(1L));
	  }

	  public virtual void testFilterTasksByExtendingQueryWithoutTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("testTask");
		filterId = createFilter(query);

		TaskQuery extendingQuery = taskService.createTaskQuery().withoutTenantId();
		assertThat(filterService.count(filterId, extendingQuery), @is(1L));
	  }

	  public virtual void testFilterTasksWithNoAuthenticatedTenants()
	  {
		TaskQuery query = taskService.createTaskQuery();
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, null);

		assertThat(filterService.count(filterId), @is(1L));
	  }

	  public virtual void testFilterTasksWithAuthenticatedTenant()
	  {
		TaskQuery query = taskService.createTaskQuery();
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		assertThat(filterService.count(filterId), @is(2L));
	  }

	  public virtual void testFilterTasksWithAuthenticatedTenants()
	  {
		TaskQuery query = taskService.createTaskQuery();
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		assertThat(filterService.count(filterId), @is(3L));
	  }

	  public virtual void testFilterTasksByTenantIdNoAuthenticatedTenants()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, null);

		assertThat(filterService.count(filterId), @is(0L));
	  }

	  public virtual void testFilterTasksByTenantIdWithAuthenticatedTenant()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		assertThat(filterService.count(filterId), @is(1L));
	  }

	  public virtual void testFilterTasksByExtendingQueryWithTenantIdNoAuthenticatedTenants()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("testTask");
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, null);

		TaskQuery extendingQuery = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);
		assertThat(filterService.count(filterId, extendingQuery), @is(0L));
	  }

	  public virtual void testFilterTasksByExtendingQueryWithTenantIdAuthenticatedTenant()
	  {
		TaskQuery query = taskService.createTaskQuery().taskName("testTask");
		filterId = createFilter(query);

		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		TaskQuery extendingQuery = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);
		assertThat(filterService.count(filterId, extendingQuery), @is(1L));
	  }

	  public virtual void testFilterTasksWithDisabledTenantCheck()
	  {
		TaskQuery query = taskService.createTaskQuery();
		filterId = createFilter(query);

		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		assertThat(filterService.count(filterId), @is(3L));
	  }

	  protected internal virtual void createTaskWithoutTenantId()
	  {
		createTaskForTenant(null);
	  }

	  protected internal virtual void createTaskForTenant(string tenantId)
	  {
		Task newTask = taskService.newTask();
		newTask.Name = "testTask";

		if (!string.ReferenceEquals(tenantId, null))
		{
		  newTask.TenantId = tenantId;
		}

		taskService.saveTask(newTask);

		taskIds.Add(newTask.Id);
	  }

	  protected internal virtual string createFilter(TaskQuery query)
	  {
		Filter newFilter = filterService.newTaskFilter("myFilter");
		newFilter.Query = query;

		return filterService.saveFilter(newFilter).Id;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		filterService.deleteFilter(filterId);
		identityService.clearAuthentication();
		foreach (string taskId in taskIds)
		{
		  taskService.deleteTask(taskId, true);
		}
	  }
	}

}