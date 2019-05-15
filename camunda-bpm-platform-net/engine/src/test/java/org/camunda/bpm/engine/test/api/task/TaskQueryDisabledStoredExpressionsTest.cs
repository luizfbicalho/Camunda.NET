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
namespace org.camunda.bpm.engine.test.api.task
{

	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TaskQueryDisabledStoredExpressionsTest : ResourceProcessEngineTestCase
	{


	  protected internal const string EXPECTED_STORED_QUERY_FAILURE_MESSAGE = "Expressions are forbidden in stored queries. This behavior can be toggled in the process engine configuration";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly string STATE_MANIPULATING_EXPRESSION = "${''.getClass().forName('" + typeof(TaskQueryDisabledStoredExpressionsTest).FullName + "').getField('MUTABLE_FIELD').setLong(null, 42)}";

	  public static long MUTABLE_FIELD = 0;

	  public TaskQueryDisabledStoredExpressionsTest() : base("org/camunda/bpm/engine/test/api/task/task-query-disabled-stored-expressions-test.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		MUTABLE_FIELD = 0;
	  }

	  public virtual void testStoreFilterWithoutExpression()
	  {
		TaskQuery taskQuery = taskService.createTaskQuery().dueAfter(DateTime.Now);
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = taskQuery;

		// saving the filter suceeds
		filterService.saveFilter(filter);
		assertEquals(1, filterService.createFilterQuery().count());

		// cleanup
		filterService.deleteFilter(filter.Id);
	  }

	  public virtual void testStoreFilterWithExpression()
	  {
		TaskQuery taskQuery = taskService.createTaskQuery().dueAfterExpression(STATE_MANIPULATING_EXPRESSION);
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = taskQuery;

		try
		{
		  filterService.saveFilter(filter);
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
		}
		assertTrue(fieldIsUnchanged());
	  }

	  public virtual void testUpdateFilterWithExpression()
	  {
		// given a stored filter
		TaskQuery taskQuery = taskService.createTaskQuery().dueAfter(DateTime.Now);
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = taskQuery;
		filterService.saveFilter(filter);

		// updating the filter with an expression does not suceed
		filter.Query = taskQuery.dueBeforeExpression(STATE_MANIPULATING_EXPRESSION);
		assertEquals(1, filterService.createFilterQuery().count());

		try
		{
		  filterService.saveFilter(filter);
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
		}
		assertTrue(fieldIsUnchanged());

		// cleanup
		filterService.deleteFilter(filter.Id);
	  }

	  public virtual void testCannotExecuteStoredFilter()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.TaskQuery filterQuery = taskService.createTaskQuery().dueAfterExpression(STATE_MANIPULATING_EXPRESSION);
		TaskQuery filterQuery = taskService.createTaskQuery().dueAfterExpression(STATE_MANIPULATING_EXPRESSION);

		// store a filter bypassing validation
		// the API way of doing this would be by reconfiguring the engine
		string filterId = processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, filterQuery));

		extendFilterAndValidateFailingQuery(filterId, null);

		// cleanup
		filterService.deleteFilter(filterId);
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly TaskQueryDisabledStoredExpressionsTest outerInstance;

		  private TaskQuery filterQuery;

		  public CommandAnonymousInnerClass(TaskQueryDisabledStoredExpressionsTest outerInstance, TaskQuery filterQuery)
		  {
			  this.outerInstance = outerInstance;
			  this.filterQuery = filterQuery;
		  }


		  public string execute(CommandContext commandContext)
		  {
			FilterEntity filter = new FilterEntity(EntityTypes.TASK);
			filter.setQuery(filterQuery);
			filter.Name = "filter";
			commandContext.DbEntityManager.insert(filter);
			return filter.Id;
		  }
	  }

	  protected internal virtual bool fieldIsUnchanged()
	  {
		return MUTABLE_FIELD == 0;
	  }

	  protected internal virtual void extendFilterAndValidateFailingQuery(string filterId, TaskQuery query)
	  {
		try
		{
		  filterService.list(filterId, query);
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());

		try
		{
		  filterService.count(filterId, query);
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_STORED_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());
	  }
	}

}