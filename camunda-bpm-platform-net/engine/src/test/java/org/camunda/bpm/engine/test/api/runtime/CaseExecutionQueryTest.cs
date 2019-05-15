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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.caseExecutionByDefinitionId;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.caseExecutionByDefinitionKey;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.caseExecutionById;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using NullTolerantComparator = org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.NullTolerantComparator;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionQueryTest : PluggableProcessEngineTestCase
	{

	  private static string CASE_DEFINITION_KEY = "oneTaskCase";
	  private static string CASE_DEFINITION_KEY_2 = "twoTaskCase";

	  /// <summary>
	  /// Setup starts 4 case instances of oneTaskCase
	  /// and 1 instance of twoTaskCase
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn").addClasspathResource("org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn").deploy();

		for (int i = 0; i < 4; i++)
		{
		  caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).businessKey(i + "").create();
		}
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY_2).businessKey("1").create();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
		base.tearDown();
	  }

	  private void verifyQueryResults(CaseExecutionQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  protected internal virtual void verifyQueryWithOrdering(CaseExecutionQuery query, int countExpected, NullTolerantComparator<CaseExecution> expectedOrdering)
	  {
		verifyQueryResults(query, countExpected);
		TestOrderingUtil.verifySorting(query.list(), expectedOrdering);
	  }

	  private void verifySingleResultFails(CaseExecutionQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryWithoutQueryParameter()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		verifyQueryResults(query, 11);
	  }

	  public virtual void testQueryByCaseDefinitionKey()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 8);

		query.caseDefinitionKey(CASE_DEFINITION_KEY_2);

		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionKey()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseDefinitionKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionKey(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByCaseDefinitionId()
	  {
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).singleResult().Id;

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseDefinitionId(caseDefinitionId);

		verifyQueryResults(query, 8);

		caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).singleResult().Id;

		query.caseDefinitionId(caseDefinitionId);

		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseDefinitionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseDefinitionId(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByCaseInstaceId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		IList<CaseInstance> caseInstances = caseService.createCaseInstanceQuery().caseDefinitionKey(CASE_DEFINITION_KEY).list();

		foreach (CaseInstance caseInstance in caseInstances)
		{
		  query.caseInstanceId(caseInstance.Id);

		  verifyQueryResults(query, 2);
		}

		CaseInstance instance = caseService.createCaseInstanceQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).singleResult();

		query.caseInstanceId(instance.Id);

		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseInstanceId(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByCaseInstanceBusinessKey()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceBusinessKey("0");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidCaseInstanceBusinessKey()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceBusinessKey("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseInstanceBusinessKey(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByCaseInstanceBusinessKeyAndCaseDefinitionKey()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceBusinessKey("0").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 2);

		query.caseInstanceBusinessKey("1").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 2);

		query.caseInstanceBusinessKey("2").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 2);

		query.caseInstanceBusinessKey("3").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 2);

		query.caseInstanceBusinessKey("1").caseDefinitionKey(CASE_DEFINITION_KEY_2);

		verifyQueryResults(query, 3);

	  }

	  public virtual void testQueryByCaseExecutionId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		IList<CaseExecution> executions = caseService.createCaseExecutionQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).list();

		foreach (CaseExecution execution in executions)
		{
		  query.caseExecutionId(execution.Id);

		  verifyQueryResults(query, 1);
		}

	  }

	  public virtual void testQueryByInvalidCaseExecutionId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseExecutionId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.caseExecutionId(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByActivityId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.activityId("PI_HumanTask_1");

		verifyQueryResults(query, 5);

		query.activityId("PI_HumanTask_2");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByInvalidActivityId()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.activityId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.activityId(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn"})]
	  public virtual void testQueryByAvailable()
	  {
		caseService.withCaseDefinitionByKey("oneMilestoneCase").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.available();

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByEnabled()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.enabled();

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByActive()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.active();

		verifyQueryResults(query, 9);

	  }

	  public virtual void testQueryByDisabled()
	  {
		IList<CaseExecution> caseExecutions = caseService.createCaseExecutionQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).activityId("PI_HumanTask_1").list();

		foreach (CaseExecution caseExecution in caseExecutions)
		{
		  caseService.withCaseExecution(caseExecution.Id).disable();
		}

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.disabled();

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByNullVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aNullValue", null);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByStringVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aStringValue", "abc");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByBooleanVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aBooleanValue", true);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByShortVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByIntegerVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("anIntegerValue", 456);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByLongVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDateVariableValueEquals()
	  {
		DateTime now = DateTime.Now;
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aDateValue", now);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDoubleVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByByteArrayVariableValueEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableValueEquals()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByStringVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aStringValue", "abd");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByBooleanVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aBooleanValue", false);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByShortVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aShortValue", (short) 124);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByIntegerVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("anIntegerValue", 457);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByLongVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aLongValue", (long) 790);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDateVariableValueNotEquals()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		DateTime before = new DateTime(now.Ticks - 100000);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aDateValue", before);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDoubleVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueNotEquals("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByByteArrayVariableValueNotEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueNotEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableValueNotEquals()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueNotEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThan("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThan("aStringValue", "ab");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThan("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThan("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThan("anIntegerValue", 455);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThan("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueGreaterThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.variableValueGreaterThan("aDateValue", before);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThan("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueGreaterThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableGreaterThan()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aStringValue", "ab");

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueGreaterThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("anIntegerValue", 455);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueGreaterThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.variableValueGreaterThanOrEqual("aDateValue", before);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueGreaterThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueGreaterThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableGreaterThanOrEqual()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThan("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThan("aStringValue", "abd");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThan("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThan("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThan("anIntegerValue", 457);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThan("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueLessThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.variableValueLessThan("aDateValue", after);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThan("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueLessThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableLessThan()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aStringValue", "abd");

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueLessThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("anIntegerValue", 457);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueLessThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.variableValueLessThanOrEqual("aDateValue", after);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLessThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueLessThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableVariableLessThanOrEqual()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullVariableValueLike()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.variableValueLike("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLike()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.variableValueLike("aStringValue", "ab%");

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLike("aStringValue", "%bc");

		verifyQueryResults(query, 1);

		query = caseService.createCaseExecutionQuery();

		query.variableValueLike("aStringValue", "%b%");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aNullValue", null);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aStringValue", "abc");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aBooleanValue", true);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("anIntegerValue", 456);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueEquals()
	  {
		DateTime now = DateTime.Now;
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aDateValue", now);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableValueEquals()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aStringValue", "abd");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aBooleanValue", false);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aShortValue", (short) 124);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("anIntegerValue", 457);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aLongValue", (long) 790);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueNotEquals()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		DateTime before = new DateTime(now.Ticks - 100000);

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aDateValue", before);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueNotEquals("aDoubleValue", 1.6);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueNotEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueNotEquals("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableValueNotEquals()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueNotEquals("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThan("aStringValue", "ab");

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThan("aShortValue", (short) 122);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThan("anIntegerValue", 455);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThan("aLongValue", (long) 788);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueGreaterThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.caseInstanceVariableValueGreaterThan("aDateValue", before);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThan("aDoubleValue", 1.4);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableGreaterThan()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aStringValue", "ab");

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aShortValue", (short) 122);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("anIntegerValue", 455);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aLongValue", (long) 788);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.caseInstanceVariableValueGreaterThanOrEqual("aDateValue", before);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aDoubleValue", 1.4);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueGreaterThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEqual("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableGreaterThanOrEqual()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueGreaterThanOrEqual("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThan("aStringValue", "abd");

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThan("aShortValue", (short) 124);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThan("anIntegerValue", 457);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThan("aLongValue", (long) 790);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueLessThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.caseInstanceVariableValueLessThan("aDateValue", after);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThan("aDoubleValue", 1.6);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableLessThan()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThan("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aStringValue", "abd");

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByBooleanCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByShortCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aShortValue", (short) 124);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByIntegerCaseInstanceVariableValueLessThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("anIntegerValue", 457);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByLongCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aLongValue", (long) 790);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDateCaseInstanceVariableValueLessThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.caseInstanceVariableValueLessThanOrEqual("aDateValue", after);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByDoubleCaseInstanceVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aDoubleValue", 1.6);

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLessThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 2);

	  }

	  public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEqual("aByteArrayValue", bytes).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryBySerializableCaseInstanceVariableLessThanOrEqual()
	  {
		IList<string> serializable = new List<string>();
		serializable.Add("one");
		serializable.Add("two");
		serializable.Add("three");

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aSerializableValue", serializable).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLessThanOrEqual("aSerializableValue", serializable).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual void testQueryByNullCaseInstanceVariableValueLike()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		try
		{
		  query.caseInstanceVariableValueLike("aNullValue", null).list();
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  public virtual void testQueryByStringCaseInstanceVariableValueLike()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLike("aStringValue", "ab%");

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLike("aStringValue", "%bc");

		verifyQueryResults(query, 2);

		query = caseService.createCaseExecutionQuery();

		query.caseInstanceVariableValueLike("aStringValue", "%b%");

		verifyQueryResults(query, 2);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testCaseVariableValueEqualsNumber() throws Exception
	  public virtual void testCaseVariableValueEqualsNumber()
	  {
		// long
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", 123L).create();

		// non-matching long
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", 12345L).create();

		// short
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", (short) 123).create();

		// double
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", 123.0d).create();

		// integer
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", 123).create();

		// untyped null (should not match)
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", null).create();

		// typed null (should not match)
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", Variables.longValue(null)).create();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("var", "123").create();

		assertEquals(4, caseService.createCaseExecutionQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, caseService.createCaseExecutionQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, caseService.createCaseExecutionQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, caseService.createCaseExecutionQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, caseService.createCaseExecutionQuery().variableValueEquals("var", Variables.numberValue(null)).count());

		// other operators
		assertEquals(4, caseService.createCaseExecutionQuery().variableValueNotEquals("var", Variables.numberValue(123)).count());
		assertEquals(1, caseService.createCaseExecutionQuery().variableValueGreaterThan("var", Variables.numberValue(123L)).count());
		assertEquals(5, caseService.createCaseExecutionQuery().variableValueGreaterThanOrEqual("var", Variables.numberValue(123.0d)).count());
		assertEquals(0, caseService.createCaseExecutionQuery().variableValueLessThan("var", Variables.numberValue((short) 123)).count());
		assertEquals(4, caseService.createCaseExecutionQuery().variableValueLessThanOrEqual("var", Variables.numberValue((short) 123)).count());

		// two executions per case instance match the query
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(2, caseService.createCaseExecutionQuery().caseInstanceVariableValueEquals("var", Variables.numberValue(null)).count());

		// other operators
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueNotEquals("var", Variables.numberValue(123)).count());
		assertEquals(2, caseService.createCaseExecutionQuery().caseInstanceVariableValueGreaterThan("var", Variables.numberValue(123L)).count());
		assertEquals(10, caseService.createCaseExecutionQuery().caseInstanceVariableValueGreaterThanOrEqual("var", Variables.numberValue(123.0d)).count());
		assertEquals(0, caseService.createCaseExecutionQuery().caseInstanceVariableValueLessThan("var", Variables.numberValue((short) 123)).count());
		assertEquals(8, caseService.createCaseExecutionQuery().caseInstanceVariableValueLessThanOrEqual("var", Variables.numberValue((short) 123)).count());

	  }


	  public virtual void testQuerySorting()
	  {
		CaseExecutionQuery query = caseService.createCaseExecutionQuery();

		// asc
		query.orderByCaseDefinitionId().asc();
		verifyQueryWithOrdering(query, 11, caseExecutionByDefinitionId());

		query = caseService.createCaseExecutionQuery();

		query.orderByCaseDefinitionKey().asc();
		verifyQueryWithOrdering(query, 11, caseExecutionByDefinitionKey(processEngine));

		query = caseService.createCaseExecutionQuery();

		query.orderByCaseExecutionId().asc();
		verifyQueryWithOrdering(query, 11, caseExecutionById());


		// desc

		query = caseService.createCaseExecutionQuery();

		query.orderByCaseDefinitionId().desc();
		verifyQueryWithOrdering(query, 11, inverted(caseExecutionByDefinitionId()));

		query = caseService.createCaseExecutionQuery();

		query.orderByCaseDefinitionKey().desc();
		verifyQueryWithOrdering(query, 11, inverted(caseExecutionByDefinitionKey(processEngine)));

		query = caseService.createCaseExecutionQuery();

		query.orderByCaseExecutionId().desc();
		verifyQueryWithOrdering(query, 11, inverted(caseExecutionById()));

		query = caseService.createCaseExecutionQuery();

	  }

	  public virtual void testCaseExecutionProperties()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY).singleResult().Id;

		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// when
		CaseExecution task = caseService.createCaseExecutionQuery().caseInstanceId(caseInstanceId).activityId("PI_HumanTask_1").singleResult();

		// then
		assertEquals("PI_HumanTask_1", task.ActivityId);
		assertEquals("A HumanTask", task.ActivityName);
		assertEquals(caseDefinitionId, task.CaseDefinitionId);
		assertEquals(caseInstanceId, task.CaseInstanceId);
		assertEquals(caseInstanceId, task.ParentId);
		assertEquals("humanTask", task.ActivityType);
		assertNotNull(task.ActivityDescription);
		assertNotNull(task.Id);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/cmmn/required/RequiredRuleTest.testVariableBasedRule.cmmn")]
	  public virtual void testQueryByRequired()
	  {
		caseService.createCaseInstanceByKey("case", Collections.singletonMap<string, object>("required", true));

		CaseExecutionQuery query = caseService.createCaseExecutionQuery().required();

		verifyQueryResults(query, 1);

		CaseExecution execution = query.singleResult();
		assertNotNull(execution);
		assertTrue(execution.Required);
	  }

	  public virtual void testNullBusinessKeyForChildExecutions()
	  {
		caseService.createCaseInstanceByKey(CASE_DEFINITION_KEY, "7890");
		IList<CaseExecution> executions = caseService.createCaseExecutionQuery().caseInstanceBusinessKey("7890").list();
		foreach (CaseExecution e in executions)
		{
		  if (((CaseExecutionEntity) e).CaseInstanceExecution)
		  {
			assertEquals("7890", ((CaseExecutionEntity) e).BusinessKeyWithoutCascade);
		  }
		  else
		  {
			assertNull(((CaseExecutionEntity) e).BusinessKeyWithoutCascade);
		  }
		}
	  }

	}

}