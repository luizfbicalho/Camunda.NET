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

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceQueryTest : PluggableProcessEngineTestCase
	{

	  private static string CASE_DEFINITION_KEY = "oneTaskCase";
	  private static string CASE_DEFINITION_KEY_2 = "oneTaskCase2";

	  private IList<string> caseInstanceIds;

	  /// <summary>
	  /// Setup starts 4 case instances of oneTaskCase
	  /// and 1 instance of oneTaskCase2
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		base.setUp();
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn").addClasspathResource("org/camunda/bpm/engine/test/api/cmmn/oneTaskCase2.cmmn").deploy();

		caseInstanceIds = new List<string>();
		for (int i = 0; i < 4; i++)
		{
		  string id = caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).businessKey(i.ToString()).create().Id;

		  caseInstanceIds.Add(id);
		}
		string id = caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY_2).businessKey("1").create().Id;

		caseInstanceIds.Add(id);
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

	  private void verifyQueryResults(CaseInstanceQuery query, int countExpected)
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

	  private void verifySingleResultFails(CaseInstanceQuery query)
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

	  public virtual void testCaseInstanceProperties()
	  {
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).singleResult().Id;

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().caseDefinitionKey(CASE_DEFINITION_KEY_2).singleResult();

		assertNotNull(caseInstance.Id);
		assertEquals(caseInstance.Id, caseInstance.CaseInstanceId);
		assertEquals("1", caseInstance.BusinessKey);
		assertEquals(caseDefinitionId, caseInstance.CaseDefinitionId);
		assertEquals("CasePlanModel_1", caseInstance.ActivityId);
		assertNull(caseInstance.ActivityName);
		assertNull(caseInstance.ParentId);
		assertTrue(caseInstance.Active);
		assertFalse(caseInstance.Enabled);

	  }

	  public virtual void testQueryWithoutQueryParameter()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		verifyQueryResults(query, 5);
	  }

	  public virtual void testQueryByCaseDefinitionKey()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.caseDefinitionKey(CASE_DEFINITION_KEY_2);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionKey()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.caseDefinitionId(caseDefinitionId);

		verifyQueryResults(query, 4);
	  }

	  public virtual void testQueryByInvalidCaseDefinitionId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

	  public virtual void testQueryByActive()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.active();

		verifyQueryResults(query, 5);
	  }

	  public virtual void testQueryByCompleted()
	  {

		foreach (org.camunda.bpm.engine.repository.Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}

		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/cmmn/oneTaskCaseWithManualActivation.cmmn").deploy();

		for (int i = 0; i < 4; i++)
		{
		  string id = caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).businessKey(i.ToString()).create().Id;

		  caseInstanceIds.Add(id);
		}

		IList<CaseExecution> executions = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").list();

		foreach (CaseExecution caseExecution in executions)
		{
		  caseService.withCaseExecution(caseExecution.Id).disable();
		}

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.completed();

		verifyQueryResults(query, 4);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/CaseInstanceQueryTest.testQueryByTerminated.cmmn"})]
	  public virtual void testQueryByTerminated()
	  {
		string caseInstanceId = caseService.withCaseDefinitionByKey("termination").create().Id;

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").caseInstanceId(caseInstanceId).singleResult().Id;

		caseService.withCaseExecution(caseExecutionId).complete();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.terminated();

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByCaseInstanceBusinessKey()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.caseInstanceBusinessKey("1");

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidCaseInstanceBusinessKey()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.caseInstanceBusinessKey("0").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKey("1").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKey("2").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKey("3").caseDefinitionKey(CASE_DEFINITION_KEY);

		verifyQueryResults(query, 1);

		query.caseInstanceBusinessKey("1").caseDefinitionKey(CASE_DEFINITION_KEY_2);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByCaseInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		foreach (string caseInstanceId in caseInstanceIds)
		{
		  query.caseInstanceId(caseInstanceId);

		  verifyQueryResults(query, 1);
		}

	  }

	  public virtual void testQueryByInvalidCaseInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

	  public virtual void testQueryByNullVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aNullValue", null).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aNullValue", null);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByStringVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aStringValue", "abc");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByBooleanVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aBooleanValue", true);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByShortVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aShortValue", (short) 123);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByIntegerVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("anIntegerValue", 456);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByLongVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aLongValue", (long) 789);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDateVariableValueEquals()
	  {
		DateTime now = DateTime.Now;
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aDateValue", now);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDoubleVariableValueEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueEquals("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByByteArrayVariableValueEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aStringValue", "abd");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByBooleanVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aBooleanValue", false);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByShortVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aShortValue", (short) 124);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByIntegerVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("anIntegerValue", 457);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByLongVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aLongValue", (long) 790);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDateVariableValueNotEquals()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		DateTime before = new DateTime(now.Ticks - 100000);

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aDateValue", before);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDoubleVariableValueNotEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueNotEquals("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByByteArrayVariableValueNotEquals()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueGreaterThan("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThan("aStringValue", "ab");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueGreaterThan("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThan("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThan("anIntegerValue", 455);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThan("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueGreaterThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.variableValueGreaterThan("aDateValue", before);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueGreaterThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThan("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueGreaterThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aStringValue", "ab");

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueGreaterThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aShortValue", (short) 122);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueGreaterThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("anIntegerValue", 455);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aLongValue", (long) 788);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueGreaterThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		DateTime before = new DateTime(now.Ticks - 100000);

		query.variableValueGreaterThanOrEqual("aDateValue", before);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueGreaterThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aDoubleValue", 1.4);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueGreaterThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueGreaterThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueLessThan("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThan("aStringValue", "abd");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueLessThan("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThan("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThan("anIntegerValue", 457);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThan("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueLessThan()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.variableValueLessThan("aDateValue", after);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueLessThan()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThan("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueLessThan()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aStringValue", "abd");

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aStringValue", "abc");

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByBooleanVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aBooleanValue", true).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueLessThanOrEqual("aBooleanValue", false).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByShortVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aShortValue", (short) 123).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aShortValue", (short) 124);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aShortValue", (short) 123);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByIntegerVariableValueLessThanOrEquals()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("anIntegerValue", 456).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("anIntegerValue", 457);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("anIntegerValue", 456);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByLongVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aLongValue", (long) 789).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aLongValue", (long) 790);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aLongValue", (long) 789);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDateVariableValueLessThanOrEqual()
	  {
		DateTime now = DateTime.Now;

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDateValue", now).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		DateTime after = new DateTime(now.Ticks + 100000);

		query.variableValueLessThanOrEqual("aDateValue", after);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aDateValue", now);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByDoubleVariableValueLessThanOrEqual()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aDoubleValue", 1.5).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aDoubleValue", 1.6);

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLessThanOrEqual("aDoubleValue", 1.5);

		verifyQueryResults(query, 1);

	  }

	  public virtual void testQueryByByteArrayVariableValueLessThanOrEqual()
	  {
		sbyte[] bytes = "somebytes".GetBytes();

		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aByteArrayValue", bytes).create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

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

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		try
		{
		  query.variableValueLike("aNullValue", null).list();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}

	  }

	  public virtual void testQueryByStringVariableValueLike()
	  {
		caseService.withCaseDefinitionByKey(CASE_DEFINITION_KEY).setVariable("aStringValue", "abc").create();

		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		query.variableValueLike("aStringValue", "ab%");

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLike("aStringValue", "%bc");

		verifyQueryResults(query, 1);

		query = caseService.createCaseInstanceQuery();

		query.variableValueLike("aStringValue", "%b%");

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQuerySorting()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery();

		// asc
		query.orderByCaseDefinitionId().asc();
		verifyQueryResults(query, 5);

		query = caseService.createCaseInstanceQuery();

		query.orderByCaseDefinitionKey().asc();
		verifyQueryResults(query, 5);

		query = caseService.createCaseInstanceQuery();

		query.orderByCaseInstanceId().asc();
		verifyQueryResults(query, 5);

		// desc

		query = caseService.createCaseInstanceQuery();

		query.orderByCaseDefinitionId().desc();
		verifyQueryResults(query, 5);

		query = caseService.createCaseInstanceQuery();

		query.orderByCaseDefinitionKey().desc();
		verifyQueryResults(query, 5);

		query = caseService.createCaseInstanceQuery();

		query.orderByCaseInstanceId().desc();
		verifyQueryResults(query, 5);

		query = caseService.createCaseInstanceQuery();

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

		assertEquals(4, caseService.createCaseInstanceQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, caseService.createCaseInstanceQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, caseService.createCaseInstanceQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, caseService.createCaseInstanceQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, caseService.createCaseInstanceQuery().variableValueEquals("var", Variables.numberValue(null)).count());

		// other operators
		assertEquals(4, caseService.createCaseInstanceQuery().variableValueNotEquals("var", Variables.numberValue(123)).count());
		assertEquals(1, caseService.createCaseInstanceQuery().variableValueGreaterThan("var", Variables.numberValue(123L)).count());
		assertEquals(5, caseService.createCaseInstanceQuery().variableValueGreaterThanOrEqual("var", Variables.numberValue(123.0d)).count());
		assertEquals(0, caseService.createCaseInstanceQuery().variableValueLessThan("var", Variables.numberValue((short) 123)).count());
		assertEquals(4, caseService.createCaseInstanceQuery().variableValueLessThanOrEqual("var", Variables.numberValue((short) 123)).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml"})]
	  public virtual void testQueryBySuperProcessInstanceId()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivityInsideSubProcess.bpmn20.xml"})]
	  public virtual void testQueryBySuperProcessInstanceIdNested()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidSuperProcessInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().superProcessInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.superProcessInstanceId(null);
		  fail();
		}
		catch (NotValidException)
		{
		}

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryBySubProcessInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneProcessTaskCase").Id;

		string subProcessInstanceId = runtimeService.createProcessInstanceQuery().superCaseInstanceId(superCaseInstanceId).singleResult().Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().subProcessInstanceId(subProcessInstanceId);

		verifyQueryResults(query, 1);

		CaseInstance caseInstance = query.singleResult();
		assertEquals(superCaseInstanceId, caseInstance.Id);
	  }

	  public virtual void testQueryByInvalidSubProcessInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().subProcessInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.subProcessInstanceId(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn"})]
	  public virtual void testQueryBySuperCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneCaseTaskCase").Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().superCaseInstanceId(superCaseInstanceId);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidSuperCaseInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().superCaseInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.superCaseInstanceId(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn"})]
	  public virtual void testQueryBySubCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneCaseTaskCase").Id;

		string subCaseInstanceId = caseService.createCaseInstanceQuery().superCaseInstanceId(superCaseInstanceId).singleResult().Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().subCaseInstanceId(subCaseInstanceId);

		verifyQueryResults(query, 1);

		CaseInstance caseInstance = query.singleResult();
		assertEquals(superCaseInstanceId, caseInstance.Id);
	  }

	  public virtual void testQueryByInvalidSubCaseInstanceId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().subCaseInstanceId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.subCaseInstanceId(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryByDeploymentId()
	  {
		string deploymentId = repositoryService.createDeploymentQuery().singleResult().Id;

		CaseInstanceQuery query = caseService.createCaseInstanceQuery().deploymentId(deploymentId);

		verifyQueryResults(query, 5);
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		CaseInstanceQuery query = caseService.createCaseInstanceQuery().deploymentId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.deploymentId(null);
		  fail();
		}
		catch (NotValidException)
		{
		  // expected
		}
	  }

	}

}