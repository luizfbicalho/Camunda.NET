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
namespace org.camunda.bpm.engine.spring.test.transaction
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/transaction/SpringTransactionIntegrationDeleteDeploymentFailTest-context.xml") public class SpringTransactionIntegrationDeleteDeploymentFailTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class SpringTransactionIntegrationDeleteDeploymentFailTest : SpringProcessEngineTestCase
	{

	  private new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));


		base.tearDown();
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly SpringTransactionIntegrationDeleteDeploymentFailTest outerInstance;

		  public CommandAnonymousInnerClass(SpringTransactionIntegrationDeleteDeploymentFailTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			commandContext.DeploymentManager.deleteDeployment(outerInstance.deploymentId, false, false, false);
			return null;
		  }
	  }

	  public virtual void testFailingAfterDeleteDeployment()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance model = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess().startEvent().userTask().endEvent().done();
		BpmnModelInstance model = Bpmn.createExecutableProcess().startEvent().userTask().endEvent().done();
		deploymentId = processEngine.RepositoryService.createDeployment().addModelInstance("model.bpmn", model).deploy().Id;

		//when
		// 1. delete deployment
		// 2. it fails in post command interceptor (see FailDeleteDeploymentsPlugin)
		// 3. transaction is rolling back
		// 4. DeleteDeploymentFailListener is called
		try
		{
		  processEngine.RepositoryService.deleteDeployment(deploymentId);
		}
		catch (Exception)
		{
		  //expected exception
		}

		//then
		// DeleteDeploymentFailListener succeeded to registered deployments back
		assertEquals(1, processEngineConfiguration.RegisteredDeployments.Count);
	  }

	}

}