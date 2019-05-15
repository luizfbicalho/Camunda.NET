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
namespace org.camunda.bpm.engine.test.api.cmmn
{
	using NotAllowedException = org.camunda.bpm.engine.exception.NotAllowedException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseServiceMilestoneTest : PluggableProcessEngineTestCase
	{

	  protected internal readonly string DEFINITION_KEY = "oneMilestoneCase";
	  protected internal readonly string MILESTONE_KEY = "PI_Milestone_1";

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testManualStart()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(MILESTONE_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).manualStart();
		  fail();
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testDisable()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(MILESTONE_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).disable();
		  fail();
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testReenable()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(MILESTONE_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).reenable();
		  fail();
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testComplete()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		string caseTaskId = queryCaseExecutionByActivityId(MILESTONE_KEY).Id;

		try
		{
		  // when
		  caseService.withCaseExecution(caseTaskId).complete();
		  fail();
		}
		catch (NotAllowedException)
		{
		}
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testTerminate()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;

		string caseTaskId = queryCaseExecutionByActivityId(MILESTONE_KEY).Id;

		caseService.withCaseExecution(caseTaskId).terminate();

		CaseExecution caseMilestone = queryCaseExecutionByActivityId(MILESTONE_KEY);
		assertNull(caseMilestone);
	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/cmmn/oneMilestoneCase.cmmn" })]
	  public virtual void testTerminateNonFluent()
	  {
		// given
		createCaseInstance(DEFINITION_KEY).Id;
		CaseExecution caseMilestone = queryCaseExecutionByActivityId(MILESTONE_KEY);
		assertNotNull(caseMilestone);

		caseService.terminateCaseExecution(caseMilestone.Id);

		caseMilestone = queryCaseExecutionByActivityId(MILESTONE_KEY);
		assertNull(caseMilestone);

	  }

	  protected internal virtual CaseInstance createCaseInstance(string caseDefinitionKey)
	  {
		return caseService.withCaseDefinitionByKey(caseDefinitionKey).create();
	  }

	  protected internal virtual CaseExecution queryCaseExecutionByActivityId(string activityId)
	  {
		return caseService.createCaseExecutionQuery().activityId(activityId).singleResult();
	  }

	  protected internal virtual CaseInstance queryCaseInstanceByKey(string caseDefinitionKey)
	  {
		return caseService.createCaseInstanceQuery().caseDefinitionKey(caseDefinitionKey).singleResult();
	  }

	  protected internal virtual Task queryTask()
	  {
		return taskService.createTaskQuery().singleResult();
	  }

	}

}