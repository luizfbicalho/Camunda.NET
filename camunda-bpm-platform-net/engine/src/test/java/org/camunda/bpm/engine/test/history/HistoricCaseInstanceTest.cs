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
namespace org.camunda.bpm.engine.test.history
{
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoricCaseInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseInstanceEntity;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricCaseInstanceTest : CmmnProcessEngineTestCase
	{
		[Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageCase.cmmn"})]
		public virtual void testCaseInstanceProperties()
		{
		CaseInstance caseInstance = createCaseInstance();

		HistoricCaseInstance historicInstance = queryHistoricCaseInstance(caseInstance.Id);

		// assert case instance properties are set correctly
		assertEquals(caseInstance.Id, historicInstance.Id);
		assertEquals(caseInstance.BusinessKey, historicInstance.BusinessKey);
		assertEquals(caseInstance.CaseDefinitionId, historicInstance.CaseDefinitionId);
		}

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageWithManualActivationCase.cmmn"})]
	  public virtual void testCaseInstanceStates()
	  {
		string caseInstanceId = createCaseInstance().Id;

		HistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);

		assertTrue(historicCaseInstance.Active);
		assertCount(1, historicQuery().active());
		assertCount(1, historicQuery().notClosed());

		// start empty stage to complete case instance
		string stageExecutionId = queryCaseExecutionByActivityId("PI_Stage_1").Id;
		manualStart(stageExecutionId);

		historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
		assertTrue(historicCaseInstance.Completed);
		assertCount(1, historicQuery().completed());
		assertCount(1, historicQuery().notClosed());

		// reactive and terminate case instance
		reactivate(caseInstanceId);
		terminate(caseInstanceId);

		historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
		assertTrue(historicCaseInstance.Terminated);
		assertCount(1, historicQuery().terminated());
		assertCount(1, historicQuery().notClosed());

		// reactive and suspend case instance
		reactivate(caseInstanceId);
		suspend(caseInstanceId);

		historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
		// not public API
		assertTrue(((HistoricCaseInstanceEntity) historicCaseInstance).Suspended);
	//    assertCount(1, historicQuery().suspended());
		assertCount(1, historicQuery().notClosed());

		// close case instance
		close(caseInstanceId);

		historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
		assertTrue(historicCaseInstance.Closed);
		assertCount(1, historicQuery().closed());
		assertCount(0, historicQuery().notClosed());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageWithManualActivationCase.cmmn"})]
	  public virtual void testHistoricCaseInstanceDates()
	  {
		// create test dates
		long duration = 72 * 3600 * 1000;
		DateTime created = ClockUtil.CurrentTime;
		DateTime closed = new DateTime(created.Ticks + duration);

		// create instance
		ClockUtil.CurrentTime = created;
		string caseInstanceId = createCaseInstance().Id;

		terminate(caseInstanceId);

		// close instance
		ClockUtil.CurrentTime = closed;
		close(caseInstanceId);

		HistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);

		// read historic dates ignoring milliseconds
		DateTime createTime = historicCaseInstance.CreateTime;
		DateTime closeTime = historicCaseInstance.CloseTime;
		long? durationInMillis = historicCaseInstance.DurationInMillis;

		assertDateSimilar(created, createTime);
		assertDateSimilar(closed, closeTime);

		// test that duration is as expected with a maximal difference of one second
		assertTrue(durationInMillis.Value >= duration);
		assertTrue(durationInMillis.Value < duration + 1000);

		// test queries
		DateTime beforeCreate = new DateTime(created.Ticks - 3600 * 1000);
		DateTime afterClose = new DateTime(closed.Ticks + 3600 * 1000);

		assertCount(1, historicQuery().createdAfter(beforeCreate));
		assertCount(0, historicQuery().createdAfter(closed));

		assertCount(0, historicQuery().createdBefore(beforeCreate));
		assertCount(1, historicQuery().createdBefore(closed));

		assertCount(0, historicQuery().createdBefore(beforeCreate).createdAfter(closed));

		assertCount(1, historicQuery().closedAfter(created));
		assertCount(0, historicQuery().closedAfter(afterClose));

		assertCount(0, historicQuery().closedBefore(created));
		assertCount(1, historicQuery().closedBefore(afterClose));

		assertCount(0, historicQuery().closedBefore(created).closedAfter(afterClose));

		assertCount(1, historicQuery().closedBefore(afterClose).closedAfter(created));
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageCase.cmmn"})]
	  public virtual void testCreateUser()
	  {
		string userId = "test";
		identityService.AuthenticatedUserId = userId;

		string caseInstanceId = createCaseInstance().Id;

		HistoricCaseInstance historicCaseInstance = queryHistoricCaseInstance(caseInstanceId);
		assertEquals(userId, historicCaseInstance.CreateUserId);
		assertCount(1, historicQuery().createdBy(userId));

		identityService.AuthenticatedUserId = null;
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testSuperCaseInstance()
	  {
		string caseInstanceId = createCaseInstanceByKey("oneCaseTaskCase").Id;
		queryCaseExecutionByActivityId("PI_CaseTask_1").Id;

		HistoricCaseInstance historicCaseInstance = historicQuery().superCaseInstanceId(caseInstanceId).singleResult();

		assertNotNull(historicCaseInstance);
		assertEquals(caseInstanceId, historicCaseInstance.SuperCaseInstanceId);

		string superCaseInstanceId = historicQuery().subCaseInstanceId(historicCaseInstance.Id).singleResult().Id;

		assertEquals(caseInstanceId, superCaseInstanceId);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn", "org/camunda/bpm/engine/test/api/repository/three_.cmmn" })]
	  public virtual void testHistoricCaseInstanceQuery()
	  {
		CaseInstance oneTaskCase = createCaseInstanceByKey("oneTaskCase", "oneBusiness");
		CaseInstance twoTaskCase = createCaseInstanceByKey("twoTaskCase", "twoBusiness");
		createCaseInstanceByKey("xyz_", "xyz_");

		assertCount(1, historicQuery().caseInstanceId(oneTaskCase.Id));
		assertCount(1, historicQuery().caseInstanceId(twoTaskCase.Id));

		ISet<string> caseInstanceIds = new HashSet<string>();
		caseInstanceIds.Add(oneTaskCase.Id);
		caseInstanceIds.Add("unknown1");
		caseInstanceIds.Add(twoTaskCase.Id);
		caseInstanceIds.Add("unknown2");

		assertCount(2, historicQuery().caseInstanceIds(caseInstanceIds));
		assertCount(0, historicQuery().caseInstanceIds(caseInstanceIds).caseInstanceId("someOtherId"));

		assertCount(1, historicQuery().caseDefinitionId(oneTaskCase.CaseDefinitionId));

		assertCount(1, historicQuery().caseDefinitionKey("oneTaskCase"));

		assertCount(3, historicQuery().caseDefinitionKeyNotIn(new IList<string> {"unknown"}));
		assertCount(2, historicQuery().caseDefinitionKeyNotIn(new IList<string> {"oneTaskCase"}));
		assertCount(1, historicQuery().caseDefinitionKeyNotIn(new IList<string> {"oneTaskCase", "twoTaskCase"}));
		assertCount(0, historicQuery().caseDefinitionKeyNotIn(new IList<string> {"oneTaskCase"}).caseDefinitionKey("oneTaskCase"));


		try
		{
		  // oracle handles empty string like null which seems to lead to undefined behavior of the LIKE comparison
		  historicQuery().caseDefinitionKeyNotIn(new IList<string> {""});
		  fail("Exception expected");
		}
		catch (NotValidException)
		{
		  // expected
		}


		assertCount(1, historicQuery().caseDefinitionName("One Task Case"));

		assertCount(2, historicQuery().caseDefinitionNameLike("%T%"));
		assertCount(1, historicQuery().caseDefinitionNameLike("One%"));
		assertCount(0, historicQuery().caseDefinitionNameLike("%Process%"));
		assertCount(1, historicQuery().caseDefinitionNameLike("%z\\_"));

		assertCount(1, historicQuery().caseInstanceBusinessKey("oneBusiness"));

		assertCount(2, historicQuery().caseInstanceBusinessKeyLike("%Business"));
		assertCount(1, historicQuery().caseInstanceBusinessKeyLike("one%"));
		assertCount(0, historicQuery().caseInstanceBusinessKeyLike("%unknown%"));
		assertCount(1, historicQuery().caseInstanceBusinessKeyLike("%z\\_"));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryByVariable()
	  {
		string caseInstanceId = createCaseInstance().Id;
		caseService.setVariable(caseInstanceId, "foo", "bar");
		caseService.setVariable(caseInstanceId, "number", 10);

		assertCount(1, historicQuery().variableValueEquals("foo", "bar"));
		assertCount(0, historicQuery().variableValueNotEquals("foo", "bar"));
		assertCount(1, historicQuery().variableValueNotEquals("foo", "lol"));
		assertCount(0, historicQuery().variableValueEquals("foo", "lol"));
		assertCount(1, historicQuery().variableValueLike("foo", "%a%"));
		assertCount(0, historicQuery().variableValueLike("foo", "%lol%"));

		assertCount(1, historicQuery().variableValueEquals("number", 10));
		assertCount(0, historicQuery().variableValueNotEquals("number", 10));
		assertCount(1, historicQuery().variableValueNotEquals("number", 1));
		assertCount(1, historicQuery().variableValueGreaterThan("number", 1));
		assertCount(0, historicQuery().variableValueLessThan("number", 1));
		assertCount(1, historicQuery().variableValueGreaterThanOrEqual("number", 10));
		assertCount(0, historicQuery().variableValueLessThan("number", 10));
		assertCount(1, historicQuery().variableValueLessThan("number", 20));
		assertCount(0, historicQuery().variableValueGreaterThan("number", 20));
		assertCount(1, historicQuery().variableValueLessThanOrEqual("number", 10));
		assertCount(0, historicQuery().variableValueGreaterThan("number", 10));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn") public void testCaseVariableValueEqualsNumber() throws Exception
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testCaseVariableValueEqualsNumber()
	  {
		// long
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", 123L).create();

		// non-matching long
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", 12345L).create();

		// short
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", (short) 123).create();

		// double
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", 123.0d).create();

		// integer
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", 123).create();

		// untyped null (should not match)
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", null).create();

		// typed null (should not match)
		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", Variables.longValue(null)).create();

		caseService.withCaseDefinitionByKey("oneTaskCase").setVariable("var", "123").create();

		assertEquals(4, historicQuery().variableValueEquals("var", Variables.numberValue(123)).count());
		assertEquals(4, historicQuery().variableValueEquals("var", Variables.numberValue(123L)).count());
		assertEquals(4, historicQuery().variableValueEquals("var", Variables.numberValue(123.0d)).count());
		assertEquals(4, historicQuery().variableValueEquals("var", Variables.numberValue((short) 123)).count());

		assertEquals(1, historicQuery().variableValueEquals("var", Variables.numberValue(null)).count());
	  }


	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryPaging()
	  {
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		assertEquals(3, historicQuery().listPage(0, 3).size());
		assertEquals(2, historicQuery().listPage(2, 2).size());
		assertEquals(1, historicQuery().listPage(3, 2).size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" }) @SuppressWarnings("unchecked") public void testQuerySorting()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" })]
	  public virtual void testQuerySorting()
	  {
		string oneCaseInstanceId = createCaseInstanceByKey("oneTaskCase", "oneBusinessKey").Id;
		string twoCaseInstanceId = createCaseInstanceByKey("twoTaskCase", "twoBusinessKey").Id;

		// terminate and close case instances => close time and duration is set
		terminate(oneCaseInstanceId);
		close(oneCaseInstanceId);
		// set time ahead to get different durations
		ClockUtil.CurrentTime = DateTimeUtil.now().plusHours(1).toDate();
		terminate(twoCaseInstanceId);
		close(twoCaseInstanceId);

		HistoricCaseInstance oneCaseInstance = queryHistoricCaseInstance(oneCaseInstanceId);
		HistoricCaseInstance twoCaseInstance = queryHistoricCaseInstance(twoCaseInstanceId);

		// sort by case instance ids
		string property = "id";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<? extends Comparable> sortedList = Arrays.asList(oneCaseInstance.getId(), twoCaseInstance.getId());
		IList<IComparable> sortedList = new IList<IComparable> {oneCaseInstance.Id, twoCaseInstance.Id};
		sortedList.Sort();

		IList<HistoricCaseInstance> instances = historicQuery().orderByCaseInstanceId().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseInstanceId().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

		// sort by case definition ids
		property = "caseDefinitionId";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: sortedList = Arrays.asList(oneCaseInstance.getCaseDefinitionId(), twoCaseInstance.getCaseDefinitionId());
		sortedList = new IList<IComparable> {oneCaseInstance.CaseDefinitionId, twoCaseInstance.CaseDefinitionId};
		sortedList.Sort();

		instances = historicQuery().orderByCaseDefinitionId().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseDefinitionId().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

		// sort by business keys
		property = "businessKey";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: sortedList = Arrays.asList(oneCaseInstance.getBusinessKey(), twoCaseInstance.getBusinessKey());
		sortedList = new IList<IComparable> {oneCaseInstance.BusinessKey, twoCaseInstance.BusinessKey};
		sortedList.Sort();

		instances = historicQuery().orderByCaseInstanceBusinessKey().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseInstanceBusinessKey().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

		// sort by create time
		property = "createTime";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: sortedList = Arrays.asList(oneCaseInstance.getCreateTime(), twoCaseInstance.getCreateTime());
		sortedList = new IList<IComparable> {oneCaseInstance.CreateTime, twoCaseInstance.CreateTime};
		sortedList.Sort();

		instances = historicQuery().orderByCaseInstanceCreateTime().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseInstanceCreateTime().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

		// sort by close time
		property = "closeTime";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: sortedList = Arrays.asList(oneCaseInstance.getCloseTime(), twoCaseInstance.getCloseTime());
		sortedList = new IList<IComparable> {oneCaseInstance.CloseTime, twoCaseInstance.CloseTime};
		sortedList.Sort();

		instances = historicQuery().orderByCaseInstanceCloseTime().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseInstanceCloseTime().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

		// sort by duration
		property = "durationInMillis";
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: sortedList = Arrays.asList(oneCaseInstance.getDurationInMillis(), twoCaseInstance.getDurationInMillis());
		sortedList = new IList<IComparable> {oneCaseInstance.DurationInMillis, twoCaseInstance.DurationInMillis};
		sortedList.Sort();

		instances = historicQuery().orderByCaseInstanceDuration().asc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[0])), hasProperty(property, equalTo(sortedList[1]))));

		instances = historicQuery().orderByCaseInstanceDuration().desc().list();
		assertEquals(2, instances.Count);
		assertThat(instances, contains(hasProperty(property, equalTo(sortedList[1])), hasProperty(property, equalTo(sortedList[0]))));

	  }

	  public virtual void testInvalidSorting()
	  {
		try
		{
		  historicQuery().asc();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  historicQuery().desc();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  historicQuery().orderByCaseInstanceId().count();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testNativeQuery()
	  {
		string id = createCaseInstance().Id;
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		string tableName = managementService.getTableName(typeof(HistoricCaseInstance));

		assertEquals(tablePrefix + "ACT_HI_CASEINST", tableName);
		assertEquals(tableName, managementService.getTableName(typeof(HistoricCaseInstanceEntity)));

		assertEquals(4, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT * FROM " + tableName).list().size());
		assertEquals(4, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT count(*) FROM " + tableName).count());

		assertEquals(16, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H1, " + tableName + " H2").count());

		// select with distinct
		assertEquals(4, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT DISTINCT * FROM " + tableName).list().size());

		assertEquals(1, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H WHERE H.ID_ = '" + id + "'").count());
		assertEquals(1, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT * FROM " + tableName + " H WHERE H.ID_ = '" + id + "'").list().size());

		// use parameters
		assertEquals(1, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT count(*) FROM " + tableName + " H WHERE H.ID_ = #{caseInstanceId}").parameter("caseInstanceId", id).count());
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testNativeQueryPaging()
	  {
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();
		createCaseInstance();

		string tableName = managementService.getTableName(typeof(HistoricCaseInstance));
		assertEquals(3, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT * FROM " + tableName).listPage(0, 3).size());
		assertEquals(2, historyService.createNativeHistoricCaseInstanceQuery().sql("SELECT * FROM " + tableName).listPage(2, 2).size());
	  }

	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/emptyStageWithManualActivationCase.cmmn"})]
	  public virtual void testDeleteHistoricCaseInstance()
	  {
		CaseInstance caseInstance = createCaseInstance();

		string caseInstanceId = caseInstance.Id;
		HistoricCaseInstance historicInstance = queryHistoricCaseInstance(caseInstanceId);
		assertNotNull(historicInstance);

		try
		{
		  // should not be able to delete historic case instance cause the case instance is still running
		  historyService.deleteHistoricCaseInstance(historicInstance.Id);
		  fail("Exception expected");
		}
		catch (NullValueException)
		{
		  // expected
		}

		terminate(caseInstanceId);
		close(caseInstanceId);

		identityService.AuthenticatedUserId = "testUser";
		historyService.deleteHistoricCaseInstance(historicInstance.Id);
		identityService.clearAuthentication();

		if (processEngineConfiguration.HistoryLevel.Id >= org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL.Id)
		{
		  // a user operation log should have been created
		  assertEquals(1, historyService.createUserOperationLogQuery().count());
		  UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();
		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
		  assertEquals(EntityTypes.CASE_INSTANCE, entry.EntityType);
		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, entry.OperationType);
		  assertEquals(caseInstanceId, entry.CaseInstanceId);
		  assertNull(entry.Property);
		  assertNull(entry.OrgValue);
		  assertNull(entry.NewValue);
		}

		assertCount(0, historicQuery());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySuperProcessInstanceId()
	  {
		string superProcessInstanceId = runtimeService.startProcessInstanceByKey("subProcessQueryTest").Id;

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().superProcessInstanceId(superProcessInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricCaseInstance subCaseInstance = query.singleResult();
		assertNotNull(subCaseInstance);
		assertEquals(superProcessInstanceId, subCaseInstance.SuperProcessInstanceId);
		assertNull(subCaseInstance.SuperCaseInstanceId);
	  }

	  public virtual void testQueryByInvalidSuperProcessInstanceId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		query.superProcessInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneProcessTaskCase.cmmn", "org/camunda/bpm/engine/test/api/runtime/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testQueryBySubProcessInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneProcessTaskCase").Id;

		string subProcessInstanceId = runtimeService.createProcessInstanceQuery().superCaseInstanceId(superCaseInstanceId).singleResult().Id;

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().subProcessInstanceId(subProcessInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricCaseInstance caseInstance = query.singleResult();
		assertEquals(superCaseInstanceId, caseInstance.Id);
		assertNull(caseInstance.SuperCaseInstanceId);
		assertNull(caseInstance.SuperProcessInstanceId);
	  }

	  public virtual void testQueryByInvalidSubProcessInstanceId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		query.subProcessInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySuperCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneCaseTaskCase").Id;

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().superCaseInstanceId(superCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricCaseInstance caseInstance = query.singleResult();
		assertEquals(superCaseInstanceId, caseInstance.SuperCaseInstanceId);
		assertNull(caseInstance.SuperProcessInstanceId);
	  }

	  public virtual void testQueryByInvalidSuperCaseInstanceId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		query.superCaseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryBySubCaseInstanceId()
	  {
		string superCaseInstanceId = caseService.createCaseInstanceByKey("oneCaseTaskCase").Id;

		string subCaseInstanceId = caseService.createCaseInstanceQuery().superCaseInstanceId(superCaseInstanceId).singleResult().Id;

		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().subCaseInstanceId(subCaseInstanceId);

		assertEquals(1, query.list().size());
		assertEquals(1, query.count());

		HistoricCaseInstance caseInstance = query.singleResult();
		assertEquals(superCaseInstanceId, caseInstance.Id);
		assertNull(caseInstance.SuperProcessInstanceId);
		assertNull(caseInstance.SuperCaseInstanceId);
	  }

	  public virtual void testQueryByInvalidSubCaseInstanceId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery();

		query.subCaseInstanceId("invalid");

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());

		query.caseInstanceId(null);

		assertEquals(0, query.count());
		assertEquals(0, query.list().size());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryByCaseActivityId()
	  {

		// given
		createCaseInstanceByKey("oneTaskCase");

		// when
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().caseActivityIdIn("PI_HumanTask_1");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneCaseTaskCase.cmmn", "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testQueryByCaseActivityIds()
	  {

		// given
		createCaseInstanceByKey("oneCaseTaskCase");

		// when
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().caseActivityIdIn("PI_HumanTask_1", "PI_CaseTask_1");

		// then
		assertEquals(2, query.list().size());
		assertEquals(2, query.count());
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn" })]
	  public virtual void testDistinctQueryByCaseActivityIds()
	  {

		// given
		createCaseInstanceByKey("twoTaskCase");

		// when
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().caseActivityIdIn("PI_HumanTask_1", "PI_HumanTask_2");

		// then
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByNonExistingCaseActivityId()
	  {
		HistoricCaseInstanceQuery query = historyService.createHistoricCaseInstanceQuery().caseActivityIdIn("nonExisting");

		assertEquals(0, query.count());
	  }

	  public virtual void testFailQueryByCaseActivityIdNull()
	  {
		try
		{
		  historyService.createHistoricCaseInstanceQuery().caseActivityIdIn((string) null);

		  fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testRetrieveCaseDefinitionKey()
	  {

		// given
		string id = createCaseInstance("oneTaskCase").Id;

		// when
		HistoricCaseInstance caseInstance = historyService.createHistoricCaseInstanceQuery().caseInstanceId(id).singleResult();

		// then
		assertEquals("oneTaskCase", caseInstance.CaseDefinitionKey);

	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn")]
	  public virtual void testRetrieveCaseDefinitionName()
	  {

		// given
		string id = createCaseInstance("oneTaskCase").Id;

		// when
		HistoricCaseInstance caseInstance = historyService.createHistoricCaseInstanceQuery().caseInstanceId(id).singleResult();

		// then
		assertEquals("One Task Case", caseInstance.CaseDefinitionName);

	  }

	  protected internal virtual HistoricCaseInstance queryHistoricCaseInstance(string caseInstanceId)
	  {
		HistoricCaseInstance historicCaseInstance = historicQuery().caseInstanceId(caseInstanceId).singleResult();
		assertNotNull(historicCaseInstance);
		return historicCaseInstance;
	  }

	  protected internal virtual HistoricCaseInstanceQuery historicQuery()
	  {
		return historyService.createHistoricCaseInstanceQuery();
	  }

	  protected internal virtual void assertCount(long count, HistoricCaseInstanceQuery historicQuery)
	  {
		assertEquals(count, historicQuery.count());
	  }

	  protected internal virtual void assertDateSimilar(DateTime date1, DateTime date2)
	  {
		long difference = Math.Abs(date1.Ticks - date2.Ticks);
		assertTrue(difference < 1000);
	  }

	}

}