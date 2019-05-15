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
namespace org.camunda.bpm.engine.test.cmmn.sentry
{

	using CaseSentryPartEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartEntity;
	using CaseSentryPartQueryImpl = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartQueryImpl;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using CmmnProcessEngineTestCase = org.camunda.bpm.engine.impl.test.CmmnProcessEngineTestCase;
	using VariableTransition = org.camunda.bpm.model.cmmn.VariableTransition;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class SentryInitializationTest : CmmnProcessEngineTestCase
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testOnPart.cmmn"})]
	  public virtual void testOnPart()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// then
		IList<CaseSentryPartEntity> parts = createCaseSentryPartQuery().list();

		assertEquals(1, parts.Count);

		CaseSentryPartEntity part = parts[0];

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.PLAN_ITEM_ON_PART, part.Type);
		assertEquals("PI_HumanTask_1", part.Source);
		assertEquals("complete", part.StandardEvent);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testVariableOnPart.cmmn"})]
	  public virtual void testVariableOnPart()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// then
		IList<CaseSentryPartEntity> parts = createCaseSentryPartQuery().list();

		assertEquals(1, parts.Count);

		CaseSentryPartEntity part = parts[0];

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.VARIABLE_ON_PART, part.Type);
		assertEquals(VariableTransition.create.name(), part.VariableEvent);
		assertEquals("variable_1", part.VariableName);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testIfPart.cmmn"})]
	  public virtual void testIfPart()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("myVar", 0).create().Id;

		// then
		IList<CaseSentryPartEntity> parts = createCaseSentryPartQuery().list();

		assertEquals(1, parts.Count);

		CaseSentryPartEntity part = parts[0];

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.IF_PART, part.Type);
		assertNull(part.Source);
		assertNull(part.StandardEvent);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testOnPartIfPartAndVariableOnPart.cmmn"})]
	  public virtual void testOnPartIfPartAndVariableOnPart()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		// then
		CaseSentryPartQueryImpl query = createCaseSentryPartQuery();

		assertEquals(3, query.count());

		CaseSentryPartEntity part = query.type(CmmnSentryDeclaration.IF_PART).singleResult();

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.IF_PART, part.Type);
		assertNull(part.Source);
		assertNull(part.StandardEvent);
		assertFalse(part.Satisfied);

		part = query.type(CmmnSentryDeclaration.PLAN_ITEM_ON_PART).singleResult();

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.PLAN_ITEM_ON_PART, part.Type);
		assertEquals("PI_HumanTask_1", part.Source);
		assertEquals("complete", part.StandardEvent);
		assertFalse(part.Satisfied);

		part = query.type(CmmnSentryDeclaration.VARIABLE_ON_PART).singleResult();

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.VARIABLE_ON_PART, part.Type);
		assertEquals(VariableTransition.delete.name(), part.VariableEvent);
		assertEquals("variable_1", part.VariableName);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testMultipleSentries.cmmn"})]
	  public virtual void testMultipleSentries()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("myVar", 0).create().Id;

		// then
		CaseSentryPartQueryImpl query = createCaseSentryPartQuery();

		assertEquals(2, query.count());

		CaseSentryPartEntity part = query.sentryId("Sentry_1").singleResult();

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.IF_PART, part.Type);
		assertNull(part.Source);
		assertNull(part.StandardEvent);
		assertFalse(part.Satisfied);

		part = query.sentryId("Sentry_2").singleResult();

		assertEquals(caseInstanceId, part.CaseExecutionId);
		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals("Sentry_2", part.SentryId);
		assertEquals(CmmnSentryDeclaration.PLAN_ITEM_ON_PART, part.Type);
		assertEquals("PI_HumanTask_1", part.Source);
		assertEquals("complete", part.StandardEvent);
		assertFalse(part.Satisfied);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/cmmn/sentry/SentryInitializationTest.testMultipleSentriesWithinStage.cmmn"})]
	  public virtual void testMultipleSentriesWithinStage()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// when
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).setVariable("myVar", 0).create().Id;

		// then
		CaseSentryPartQueryImpl query = createCaseSentryPartQuery();

		assertEquals(2, query.count());

		// when
		string stageId = caseService.createCaseExecutionQuery().activityId("PI_Stage_1").singleResult().Id;

		// then
		assertEquals(2, query.count());

		CaseSentryPartEntity part = query.sentryId("Sentry_1").singleResult();

		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals(stageId, part.CaseExecutionId);
		assertEquals("Sentry_1", part.SentryId);
		assertEquals(CmmnSentryDeclaration.IF_PART, part.Type);
		assertNull(part.Source);
		assertNull(part.StandardEvent);
		assertFalse(part.Satisfied);

		part = query.sentryId("Sentry_2").singleResult();

		assertEquals(caseInstanceId, part.CaseInstanceId);
		assertEquals(stageId, part.CaseExecutionId);
		assertEquals("Sentry_2", part.SentryId);
		assertEquals(CmmnSentryDeclaration.PLAN_ITEM_ON_PART, part.Type);
		assertEquals("PI_HumanTask_1", part.Source);
		assertEquals("complete", part.StandardEvent);
		assertFalse(part.Satisfied);
	  }

	}

}