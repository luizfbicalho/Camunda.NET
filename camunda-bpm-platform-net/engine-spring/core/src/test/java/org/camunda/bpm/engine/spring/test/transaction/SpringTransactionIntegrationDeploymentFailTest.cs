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
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/transaction/SpringTransactionIntegrationDeploymentFailTest-context.xml") public class SpringTransactionIntegrationDeploymentFailTest extends org.camunda.bpm.engine.spring.test.SpringProcessEngineTestCase
	public class SpringTransactionIntegrationDeploymentFailTest : SpringProcessEngineTestCase
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
		protected internal override void tearDown()
		{
		//must not be needed after CAM-4250 is fixed
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();
		base.tearDown();
		}

	  public virtual void testFailingAfterDeployment()
	  {
	//    given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.model.bpmn.BpmnModelInstance model = org.camunda.bpm.model.bpmn.Bpmn.createExecutableProcess().startEvent().userTask().endEvent().done();
		BpmnModelInstance model = Bpmn.createExecutableProcess().startEvent().userTask().endEvent().done();

		//when
		// 1. deploy the process
		// 2. it fails in post command interceptor (see FailDeploymentsPlugin)
		// 3. transaction is rolling back
		// 4. DeploymentFailListener is called
		try
		{
		  processEngine.RepositoryService.createDeployment().addModelInstance("model.bpmn", model).deploy();
		}
		catch (Exception)
		{
		  //expected exception
		}

		//then
		// DeploymentFailListener succeeded to remove registered deployments
		assertEquals(0, processEngineConfiguration.RegisteredDeployments.Count);
	  }

	}

}