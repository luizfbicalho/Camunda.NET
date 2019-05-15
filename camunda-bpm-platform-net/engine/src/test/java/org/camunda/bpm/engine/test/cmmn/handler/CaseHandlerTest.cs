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
namespace org.camunda.bpm.engine.test.cmmn.handler
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseHandler = org.camunda.bpm.engine.impl.cmmn.handler.CaseHandler;
	using CmmnHandlerContext = org.camunda.bpm.engine.impl.cmmn.handler.CmmnHandlerContext;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseHandlerTest : CmmnElementHandlerTest
	{

	  protected internal CaseHandler handler = new CaseHandler();
	  protected internal new CmmnHandlerContext context;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		context = new CmmnHandlerContext();

		DeploymentEntity deployment = new DeploymentEntity();
		deployment.Id = "aDeploymentId";

		context.Deployment = deployment;
		context.Model = modelInstance;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActivityName()
	  public virtual void testCaseActivityName()
	  {
		// given:
		// the case has a name "A Case"
		string name = "A Case";
		caseDefinition.Name = name;

		// when
		CmmnActivity activity = handler.handleElement(caseDefinition, context);

		// then
		assertEquals(name, activity.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityBehavior()
	  public virtual void testActivityBehavior()
	  {
		// given: a case

		// when
		CmmnActivity activity = handler.handleElement(caseDefinition, context);

		// then
		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		assertNull(behavior);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseHasNoParent()
	  public virtual void testCaseHasNoParent()
	  {
		// given: a caseDefinition

		// when
		CmmnActivity activity = handler.handleElement(caseDefinition, context);

		// then
		assertNull(activity.Parent);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseDefinitionKey()
	  public virtual void testCaseDefinitionKey()
	  {
		// given: a caseDefinition

		// when
		CaseDefinitionEntity activity = (CaseDefinitionEntity) handler.handleElement(caseDefinition, context);

		// then
		assertEquals(caseDefinition.Id, activity.Key);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentId()
	  public virtual void testDeploymentId()
	  {
		// given: a caseDefinition

		// when
		CaseDefinitionEntity activity = (CaseDefinitionEntity) handler.handleElement(caseDefinition, context);

		// then
		string deploymentId = context.Deployment.Id;
		assertEquals(deploymentId, activity.DeploymentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryTimeToLiveNull()
	  public virtual void testHistoryTimeToLiveNull()
	  {
		// given: a caseDefinition

		// when
		CaseDefinitionEntity activity = (CaseDefinitionEntity) handler.handleElement(caseDefinition, context);

		// then
		assertNull(activity.HistoryTimeToLive);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryTimeToLive()
	  public virtual void testHistoryTimeToLive()
	  {
		// given: a caseDefinition
		int? historyTimeToLive = 6;
		caseDefinition.CamundaHistoryTimeToLive = historyTimeToLive;

		// when
		CaseDefinitionEntity activity = (CaseDefinitionEntity) handler.handleElement(caseDefinition, context);

		// then
		assertEquals(Convert.ToInt32(historyTimeToLive), activity.HistoryTimeToLive);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoryTimeToLiveNegative()
	  public virtual void testHistoryTimeToLiveNegative()
	  {
		// given: a caseDefinition
		int? historyTimeToLive = -6;
		caseDefinition.CamundaHistoryTimeToLive = historyTimeToLive;

		try
		{
		  // when
		  handler.handleElement(caseDefinition, context);
		  fail("Exception is expected, that negative value is not allowed.");
		}
		catch (NotValidException ex)
		{
		  assertTrue(ex.Message.contains("negative value is not allowed"));
		}
	  }

	}

}