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
namespace org.camunda.bpm.engine.test.api.filter
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Tobias Metzke
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class FilterServiceUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public FilterServiceUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(engineRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal FilterService filterService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  protected internal IdentityService identityService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		filterService = engineRule.FilterService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		identityService = engineRule.IdentityService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		// delete all existing filters
		foreach (Filter filter in filterService.createTaskFilterQuery().list())
		{
		  filterService.deleteFilter(filter.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateFilter()
	  public virtual void testCreateFilter()
	  {
		// given
		Filter filter = filterService.newTaskFilter().setName("name").setOwner("owner").setQuery(taskService.createTaskQuery()).setProperties(new Dictionary<string, object>());

		// when
		identityService.AuthenticatedUserId = "userId";
		filterService.saveFilter(filter);
		identityService.clearAuthentication();

		// then
		assertEquals(1L, historyService.createUserOperationLogQuery().count());
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.FILTER));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE));
		assertThat(logEntry.Property, @is("filterId"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is(filter.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFilter()
	  public virtual void testUpdateFilter()
	  {
		// given
		Filter filter = filterService.newTaskFilter().setName("name").setOwner("owner").setQuery(taskService.createTaskQuery()).setProperties(new Dictionary<string, object>());
		filterService.saveFilter(filter);

		// when
		identityService.AuthenticatedUserId = "userId";
		filter.Name = filter.Name + "_new";
		filterService.saveFilter(filter);
		identityService.clearAuthentication();

		// then
		assertEquals(1L, historyService.createUserOperationLogQuery().count());
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.FILTER));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE));
		assertThat(logEntry.Property, @is("filterId"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is(filter.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteFilter()
	  public virtual void testDeleteFilter()
	  {
		// given
		Filter filter = filterService.newTaskFilter().setName("name").setOwner("owner").setQuery(taskService.createTaskQuery()).setProperties(new Dictionary<string, object>());
		filterService.saveFilter(filter);

		// when
		identityService.AuthenticatedUserId = "userId";
		filterService.deleteFilter(filter.Id);
		identityService.clearAuthentication();

		// then
		assertEquals(1L, historyService.createUserOperationLogQuery().count());
		UserOperationLogEntry logEntry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(logEntry.EntityType, @is(EntityTypes.FILTER));
		assertThat(logEntry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(logEntry.Property, @is("filterId"));
		assertThat(logEntry.OrgValue, nullValue());
		assertThat(logEntry.NewValue, @is(filter.Id));
	  }

	}

}