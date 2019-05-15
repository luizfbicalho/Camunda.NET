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
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class TaskQueryDisabledAdhocExpressionsTest : PluggableProcessEngineTestCase
	{

	  protected internal const string EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE = "Expressions are forbidden in adhoc queries. "
		  + "This behavior can be toggled in the process engine configuration";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly string STATE_MANIPULATING_EXPRESSION = "${''.getClass().forName('" + typeof(TaskQueryDisabledAdhocExpressionsTest).FullName + "').getField('MUTABLE_FIELD').setLong(null, 42)}";

	  public static long MUTABLE_FIELD = 0;

	  public virtual void testDefaultSetting()
	  {
		assertTrue(processEngineConfiguration.EnableExpressionsInStoredQueries);
		assertFalse(processEngineConfiguration.EnableExpressionsInAdhocQueries);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		MUTABLE_FIELD = 0;
	  }

	  public virtual void testAdhocExpressionsFail()
	  {
		executeAndValidateFailingQuery(taskService.createTaskQuery().dueAfterExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().dueBeforeExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().dueDateExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().followUpAfterExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().followUpBeforeExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().followUpBeforeOrNotExistentExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().followUpDateExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskAssigneeExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskAssigneeLikeExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskCandidateGroupExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskCandidateGroupInExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskCandidateUserExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskCreatedAfterExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskCreatedBeforeExpression(STATE_MANIPULATING_EXPRESSION));
		executeAndValidateFailingQuery(taskService.createTaskQuery().taskOwnerExpression(STATE_MANIPULATING_EXPRESSION));
	  }

	  public virtual void testExtendStoredFilterByExpression()
	  {

		// given a stored filter
		TaskQuery taskQuery = taskService.createTaskQuery().dueAfterExpression("${now()}");
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = taskQuery;
		filterService.saveFilter(filter);

		// it is possible to execute the stored query with an expression
		assertEquals(new long?(0), filterService.count(filter.Id));
		assertEquals(0, filterService.list(filter.Id).Count);

		// but it is not possible to executed the filter with an extended query that uses expressions
		extendFilterAndValidateFailingQuery(filter, taskService.createTaskQuery().dueAfterExpression(STATE_MANIPULATING_EXPRESSION));

		// cleanup
		filterService.deleteFilter(filter.Id);
	  }

	  public virtual void testExtendStoredFilterByScalar()
	  {
		// given a stored filter
		TaskQuery taskQuery = taskService.createTaskQuery().dueAfterExpression("${now()}");
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = taskQuery;
		filterService.saveFilter(filter);

		// it is possible to execute the stored query with an expression
		assertEquals(new long?(0), filterService.count(filter.Id));
		assertEquals(0, filterService.list(filter.Id).Count);

		// and it is possible to extend the filter query when not using an expression
		assertEquals(new long?(0), filterService.count(filter.Id, taskService.createTaskQuery().dueAfter(DateTime.Now)));
		assertEquals(0, filterService.list(filter.Id, taskService.createTaskQuery().dueAfter(DateTime.Now)).Count);

		// cleanup
		filterService.deleteFilter(filter.Id);
	  }

	  protected internal virtual bool fieldIsUnchanged()
	  {
		return MUTABLE_FIELD == 0;
	  }

	  protected internal virtual void executeAndValidateFailingQuery(TaskQuery query)
	  {
		try
		{
		  query.list();
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());

		try
		{
		  query.count();
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());
	  }

	  protected internal virtual void extendFilterAndValidateFailingQuery(Filter filter, TaskQuery query)
	  {
		try
		{
		  filterService.list(filter.Id, query);
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());

		try
		{
		  filterService.count(filter.Id, query);
		}
		catch (BadUserRequestException e)
		{
		  assertTextPresent(EXPECTED_ADHOC_QUERY_FAILURE_MESSAGE, e.Message);
		}

		assertTrue(fieldIsUnchanged());
	  }
	}

}