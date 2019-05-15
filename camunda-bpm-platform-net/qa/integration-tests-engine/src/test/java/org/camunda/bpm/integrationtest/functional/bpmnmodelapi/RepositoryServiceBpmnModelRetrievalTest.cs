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
namespace org.camunda.bpm.integrationtest.functional.bpmnmodelapi
{
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class RepositoryServiceBpmnModelRetrievalTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class RepositoryServiceBpmnModelRetrievalTest : AbstractFoxPlatformIntegrationTest
	{

	  private const string TEST_PROCESS = "testProcess";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessApplication()
	  public static WebArchive createProcessApplication()
	  {
		return initWebArchiveDeployment().addAsResource(new StringAsset(Bpmn.convertToString(Bpmn.createExecutableProcess(TEST_PROCESS).done())), "testProcess.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldReturnBpmnModelInstance()
	  public virtual void shouldReturnBpmnModelInstance()
	  {

		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(TEST_PROCESS).singleResult();

		BpmnModelInstance bpmnModelInstance = repositoryService.getBpmnModelInstance(processDefinition.Id);
		Assert.assertNotNull(bpmnModelInstance);

	  }
	}

}