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
namespace org.camunda.bpm.engine.test.standalone.deploy
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.standalone.deploy.TestCmmnTransformListener.numberOfRegistered;

	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using Case = org.camunda.bpm.model.cmmn.instance.Case;
	using CasePlanModel = org.camunda.bpm.model.cmmn.instance.CasePlanModel;
	using CaseTask = org.camunda.bpm.model.cmmn.instance.CaseTask;
	using DecisionTask = org.camunda.bpm.model.cmmn.instance.DecisionTask;
	using Definitions = org.camunda.bpm.model.cmmn.instance.Definitions;
	using EventListener = org.camunda.bpm.model.cmmn.instance.EventListener;
	using HumanTask = org.camunda.bpm.model.cmmn.instance.HumanTask;
	using Milestone = org.camunda.bpm.model.cmmn.instance.Milestone;
	using ProcessTask = org.camunda.bpm.model.cmmn.instance.ProcessTask;
	using Sentry = org.camunda.bpm.model.cmmn.instance.Sentry;
	using Stage = org.camunda.bpm.model.cmmn.instance.Stage;
	using Task = org.camunda.bpm.model.cmmn.instance.Task;
	using After = org.junit.After;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CmmnTransformListenerTest : ResourceProcessEngineTestCase
	{

	  public CmmnTransformListenerTest() : base("org/camunda/bpm/engine/test/standalone/deploy/cmmn.transform.listener.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		TestCmmnTransformListener.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testListenerInvocation()
	  public virtual void testListenerInvocation()
	  {
		// Check if case definition has different key
		assertEquals(0, repositoryService.createCaseDefinitionQuery().caseDefinitionKey("testCase").count());
		assertEquals(0, repositoryService.createCaseDefinitionQuery().caseDefinitionKey("testCase-modified").count());
		assertEquals(1, repositoryService.createCaseDefinitionQuery().caseDefinitionKey("testCase-modified-modified").count());

		assertEquals(1, numberOfRegistered(typeof(Definitions)));
		assertEquals(1, numberOfRegistered(typeof(Case)));
		assertEquals(1, numberOfRegistered(typeof(CasePlanModel)));
		assertEquals(3, numberOfRegistered(typeof(HumanTask)));
		assertEquals(1, numberOfRegistered(typeof(ProcessTask)));
		assertEquals(1, numberOfRegistered(typeof(CaseTask)));
		assertEquals(1, numberOfRegistered(typeof(DecisionTask)));
		// 3x HumanTask, 1x ProcessTask, 1x CaseTask, 1x DecisionTask, 1x Task
		assertEquals(7, numberOfRegistered(typeof(Task)));
		// 1x CasePlanModel, 1x Stage
		assertEquals(2, numberOfRegistered(typeof(Stage)));
		assertEquals(1, numberOfRegistered(typeof(Milestone)));
		// Note: EventListener is currently not supported!
		assertEquals(0, numberOfRegistered(typeof(EventListener)));
		assertEquals(3, numberOfRegistered(typeof(Sentry)));

		assertEquals(11, TestCmmnTransformListener.cmmnActivities.Count);
		assertEquals(24, TestCmmnTransformListener.modelElementInstances.Count);
		assertEquals(3, TestCmmnTransformListener.sentryDeclarations.Count);
	  }

	}

}