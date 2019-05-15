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
namespace org.camunda.bpm.engine.test.api.multitenancy.query
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MultiTenancyTaskQueryTest : PluggableProcessEngineTestCase
	{

	  private const string TENANT_ONE = "tenant1";
	  private const string TENANT_TWO = "tenant2";
	  private const string TENANT_NON_EXISTING = "nonExistingTenant";

	  private readonly IList<string> taskIds = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {

		createTaskWithoutTenant();
		createTaskForTenant(TENANT_ONE);
		createTaskForTenant(TENANT_TWO);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryNoTenantIdSet()
	  public virtual void testQueryNoTenantIdSet()
	  {
		TaskQuery query = taskService.createTaskQuery();

		assertThat(query.count(), @is(3L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_ONE);

		assertThat(query.count(), @is(1L));

		query = taskService.createTaskQuery().tenantIdIn(TENANT_TWO);

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIds()
	  public virtual void testQueryByTenantIds()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_ONE, TENANT_TWO);

		assertThat(query.count(), @is(2L));

		query = taskService.createTaskQuery().tenantIdIn(TENANT_ONE, TENANT_NON_EXISTING);

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTasksWithoutTenantId()
	  public virtual void testQueryByTasksWithoutTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().withoutTenantId();

		assertThat(query.count(), @is(1L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByNonExistingTenantId()
	  public virtual void testQueryByNonExistingTenantId()
	  {
		TaskQuery query = taskService.createTaskQuery().tenantIdIn(TENANT_NON_EXISTING);

		assertThat(query.count(), @is(0L));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIdNullFails()
	  public virtual void testQueryByTenantIdNullFails()
	  {
		try
		{
		  assertEquals(0, taskService.createTaskQuery().tenantIdIn((string)null));

		  fail("Exception expected");
		}
		catch (NullValueException)
		{
		  // expected
		}
	  }

	  public virtual void testQuerySortingAsc()
	  {
		// exclude tasks without tenant id because of database-specific ordering
		IList<Task> tasks = taskService.createTaskQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().asc().list();

		assertThat(tasks.Count, @is(2));
		assertThat(tasks[0].TenantId, @is(TENANT_ONE));
		assertThat(tasks[1].TenantId, @is(TENANT_TWO));
	  }

	  public virtual void testQuerySortingDesc()
	  {
		// exclude tasks without tenant id because of database-specific ordering
		IList<Task> tasks = taskService.createTaskQuery().tenantIdIn(TENANT_ONE, TENANT_TWO).orderByTenantId().desc().list();

		assertThat(tasks.Count, @is(2));
		assertThat(tasks[0].TenantId, @is(TENANT_TWO));
		assertThat(tasks[1].TenantId, @is(TENANT_ONE));
	  }

	  public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, null);

		TaskQuery query = taskService.createTaskQuery();
		assertThat(query.count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE));

		TaskQuery query = taskService.createTaskQuery();

		assertThat(query.count(), @is(2L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(0L));
		assertThat(query.tenantIdIn(TENANT_ONE, TENANT_TWO).count(), @is(1L));
	  }

	  public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.setAuthentication("user", null, Arrays.asList(TENANT_ONE, TENANT_TWO));

		TaskQuery query = taskService.createTaskQuery();

		assertThat(query.count(), @is(3L));
		assertThat(query.tenantIdIn(TENANT_ONE).count(), @is(1L));
		assertThat(query.tenantIdIn(TENANT_TWO).count(), @is(1L));
		assertThat(query.withoutTenantId().count(), @is(1L));
	  }

	  public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.TenantCheckEnabled = false;
		identityService.setAuthentication("user", null, null);

		TaskQuery query = taskService.createTaskQuery();
		assertThat(query.count(), @is(3L));
	  }

	  protected internal virtual string createTaskWithoutTenant()
	  {
		return createTaskForTenant(null);
	  }

	  protected internal virtual string createTaskForTenant(string tenantId)
	  {
		Task task = taskService.newTask();
		if (!string.ReferenceEquals(tenantId, null))
		{
		  task.TenantId = tenantId;
		}
		taskService.saveTask(task);

		string taskId = task.Id;
		taskIds.Add(taskId);

		return taskId;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		identityService.clearAuthentication();
		foreach (string taskId in taskIds)
		{
		  taskService.deleteTask(taskId, true);
		}
		taskIds.Clear();
	  }

	}

}