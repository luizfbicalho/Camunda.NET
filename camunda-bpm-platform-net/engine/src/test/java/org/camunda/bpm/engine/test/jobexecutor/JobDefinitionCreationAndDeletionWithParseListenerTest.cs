using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using AbstractBpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.AbstractBpmnParseListener;
	using BpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseListener;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// Represents a test class, which uses parse listeners
	/// to delete job definitions for asyncBefore activities and create job
	/// definitions for asyncAfter.
	/// The parse listeners are called after the bpmn xml was parsed.
	/// They set the activity asyncBefore property to false and the asyncAfter to true.
	/// In this case there should delete and create some job declarations for the activity which
	/// was asyncBefore before and are now asyncAfter.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class JobDefinitionCreationAndDeletionWithParseListenerTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobDefinitionCreationAndDeletionWithParseListenerTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule);
		}


	  /// <summary>
	  /// The custom rule which adjust the process engine configuration.
	  /// </summary>
	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			IList<BpmnParseListener> listeners = new List<BpmnParseListener>();
			listeners.Add(new AbstractBpmnParseListenerAnonymousInnerClass(this));

			configuration.CustomPreBPMNParseListeners = listeners;
			return configuration;
		  }

		  private class AbstractBpmnParseListenerAnonymousInnerClass : AbstractBpmnParseListener
		  {
			  private readonly ProcessEngineBootstrapRuleAnonymousInnerClass outerInstance;

			  public AbstractBpmnParseListenerAnonymousInnerClass(ProcessEngineBootstrapRuleAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }


			  public override void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
			  {
				activity.AsyncBefore = false;
				activity.AsyncAfter = true;
			  }
		  }
	  }

	  /// <summary>
	  /// The engine rule.
	  /// </summary>
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  /// <summary>
	  /// The rule chain for the defined rules.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingAndCreateNewJobDefinitionWithParseListener()
	  public virtual void testDeleteNonExistingAndCreateNewJobDefinitionWithParseListener()
	  {
		//given
		string modelFileName = "jobCreationWithinParseListener.bpmn20.xml";
		Stream @in = typeof(JobDefinitionCreationWithParseListenerTest).getResourceAsStream(modelFileName);
		DeploymentBuilder builder = engineRule.RepositoryService.createDeployment().addInputStream(modelFileName, @in);

		//when the asyncBefore is set to false and the asyncAfter to true in the parse listener
		Deployment deployment = builder.deploy();
		engineRule.manageDeployment(deployment);

		//then there exists one job definition
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		JobDefinition jobDef = query.singleResult();
		assertNotNull(jobDef);
		assertEquals(jobDef.ProcessDefinitionKey, "oneTaskProcess");
		assertEquals(jobDef.ActivityId, "servicetask1");
		assertEquals(jobDef.JobConfiguration, MessageJobDeclaration.ASYNC_AFTER);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobDefinitionWithParseListenerAndAsyncInXml()
	  public virtual void testDeleteJobDefinitionWithParseListenerAndAsyncInXml()
	  {
		//given the asyncBefore is set in the xml
		string modelFileName = "jobAsyncBeforeCreationWithinParseListener.bpmn20.xml";
		Stream @in = typeof(JobDefinitionCreationWithParseListenerTest).getResourceAsStream(modelFileName);
		DeploymentBuilder builder = engineRule.RepositoryService.createDeployment().addInputStream(modelFileName, @in);

		//when the asyncBefore is set to false and the asyncAfter to true in the parse listener
		Deployment deployment = builder.deploy();
		engineRule.manageDeployment(deployment);

		//then there exists one job definition
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		JobDefinition jobDef = query.singleResult();
		assertNotNull(jobDef);
		assertEquals(jobDef.ProcessDefinitionKey, "oneTaskProcess");
		assertEquals(jobDef.ActivityId, "servicetask1");
		assertEquals(jobDef.JobConfiguration, MessageJobDeclaration.ASYNC_AFTER);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteJobDefinitionWithParseListenerAndAsyncBothInXml()
	  public virtual void testDeleteJobDefinitionWithParseListenerAndAsyncBothInXml()
	  {
		//given the asyncBefore AND asyncAfter is set in the xml
		string modelFileName = "jobAsyncBothCreationWithinParseListener.bpmn20.xml";
		Stream @in = typeof(JobDefinitionCreationWithParseListenerTest).getResourceAsStream(modelFileName);
		DeploymentBuilder builder = engineRule.RepositoryService.createDeployment().addInputStream(modelFileName, @in);

		//when the asyncBefore is set to false and the asyncAfter to true in the parse listener
		Deployment deployment = builder.deploy();
		engineRule.manageDeployment(deployment);

		//then there exists one job definition
		JobDefinitionQuery query = engineRule.ManagementService.createJobDefinitionQuery();
		JobDefinition jobDef = query.singleResult();
		assertNotNull(jobDef);
		assertEquals(jobDef.ProcessDefinitionKey, "oneTaskProcess");
		assertEquals(jobDef.ActivityId, "servicetask1");
		assertEquals(jobDef.JobConfiguration, MessageJobDeclaration.ASYNC_AFTER);
	  }
	}

}