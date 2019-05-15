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
namespace org.camunda.bpm.engine.spring.test.autodeployment
{
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentQuery = org.camunda.bpm.engine.repository.DeploymentQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using AbstractXmlApplicationContext = org.springframework.context.support.AbstractXmlApplicationContext;
	using ClassPathXmlApplicationContext = org.springframework.context.support.ClassPathXmlApplicationContext;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class SpringAutoDeployTest : PvmTestCase
	{

	  protected internal const string CTX_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployTest-context.xml";
	  protected internal const string CTX_CREATE_DROP_CLEAN_DB = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployTest-create-drop-clean-db-context.xml";
	  protected internal const string CTX_DYNAMIC_DEPLOY_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployTest-dynamic-deployment-context.xml";

	  protected internal const string CTX_CMMN_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployCmmnTest-context.xml";

	  protected internal const string CTX_CMMN_BPMN_TOGETHER_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployCmmnBpmnTest-context.xml";

	  protected internal const string CTX_DEPLOY_CHANGE_ONLY_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployDeployChangeOnlyTest-context.xml";

	  protected internal const string CTX_TENANT_ID_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployTenantIdTest-context.xml";

	  protected internal const string CTX_CUSTOM_NAME_PATH = "org/camunda/bpm/engine/spring/test/autodeployment/SpringAutoDeployCustomNameTest-context.xml";


	  protected internal ApplicationContext applicationContext;
	  protected internal RepositoryService repositoryService;

	  protected internal virtual void createAppContext(string path)
	  {
		this.applicationContext = new ClassPathXmlApplicationContext(path);
		this.repositoryService = applicationContext.getBean(typeof(RepositoryService));
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		DynamicResourceProducer.clearResources();
		removeAllDeployments();
		this.applicationContext = null;
		this.repositoryService = null;
		base.tearDown();
	  }

	  public virtual void testBasicActivitiSpringIntegration()
	  {
		createAppContext(CTX_PATH);
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();

		ISet<string> processDefinitionKeys = new HashSet<string>();
		foreach (ProcessDefinition processDefinition in processDefinitions)
		{
		  processDefinitionKeys.Add(processDefinition.Key);
		}

		ISet<string> expectedProcessDefinitionKeys = new HashSet<string>();
		expectedProcessDefinitionKeys.Add("a");
		expectedProcessDefinitionKeys.Add("b");
		expectedProcessDefinitionKeys.Add("c");

		assertEquals(expectedProcessDefinitionKeys, processDefinitionKeys);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testNoRedeploymentForSpringContainerRestart() throws Exception
	  public virtual void testNoRedeploymentForSpringContainerRestart()
	  {
		createAppContext(CTX_PATH);
		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();
		assertEquals(1, deploymentQuery.count());
		ProcessDefinitionQuery processDefinitionQuery = repositoryService.createProcessDefinitionQuery();
		assertEquals(3, processDefinitionQuery.count());

		// Creating a new app context with same resources doesn't lead to more deployments
		((AbstractXmlApplicationContext) applicationContext).destroy();
		applicationContext = new ClassPathXmlApplicationContext(CTX_PATH);
		assertEquals(1, deploymentQuery.count());
		assertEquals(3, processDefinitionQuery.count());
	  }

	  public virtual void testAutoDeployCmmn()
	  {
		createAppContext(CTX_CMMN_PATH);

		IList<CaseDefinition> definitions = repositoryService.createCaseDefinitionQuery().list();

		assertEquals(1, definitions.Count);
	  }

	  public virtual void testAutoDeployCmmnAndBpmnTogether()
	  {
		createAppContext(CTX_CMMN_BPMN_TOGETHER_PATH);

		long caseDefs = repositoryService.createCaseDefinitionQuery().count();
		long procDefs = repositoryService.createProcessDefinitionQuery().count();

		assertEquals(1, caseDefs);
		assertEquals(3, procDefs);
	  }

	  // when deployChangeOnly=true, new deployment should be created only for the changed resources
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testDeployChangeOnly() throws Exception
	  public virtual void testDeployChangeOnly()
	  {
		// given
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("model1").startEvent("oldId").endEvent().done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("model1").startEvent("newId").endEvent().done();
		BpmnModelInstance model3 = Bpmn.createExecutableProcess("model2").startEvent().endEvent().done();

		DynamicResourceProducer.addResource("a.bpmn", model1);
		DynamicResourceProducer.addResource("b.bpmn", model3);

		createAppContext(CTX_DEPLOY_CHANGE_ONLY_PATH);

		// assume
		assertEquals(1, repositoryService.createDeploymentQuery().count());

		// when
		((AbstractXmlApplicationContext) applicationContext).destroy();

		DynamicResourceProducer.clearResources();
		DynamicResourceProducer.addResource("a.bpmn", model2);
		DynamicResourceProducer.addResource("b.bpmn", model3);

		applicationContext = new ClassPathXmlApplicationContext(CTX_DEPLOY_CHANGE_ONLY_PATH);
		repositoryService = (RepositoryService) applicationContext.getBean("repositoryService");

		// then
		assertEquals(2, repositoryService.createDeploymentQuery().count());
		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());
	  }

	  // Updating the bpmn20 file should lead to a new deployment when restarting the Spring container
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testResourceRedeploymentAfterProcessDefinitionChange() throws Exception
	  public virtual void testResourceRedeploymentAfterProcessDefinitionChange()
	  {
		// given
		BpmnModelInstance model1 = Bpmn.createExecutableProcess("model1").startEvent("oldId").endEvent().done();
		BpmnModelInstance model2 = Bpmn.createExecutableProcess("model1").startEvent("newId").endEvent().done();
		BpmnModelInstance model3 = Bpmn.createExecutableProcess("model2").startEvent().endEvent().done();

		DynamicResourceProducer.addResource("a.bpmn", model1);
		DynamicResourceProducer.addResource("b.bpmn", model3);

		createAppContext(CTX_DYNAMIC_DEPLOY_PATH);
		assertEquals(1, repositoryService.createDeploymentQuery().count());
		((AbstractXmlApplicationContext)applicationContext).destroy();

		// when
		DynamicResourceProducer.clearResources();
		DynamicResourceProducer.addResource("a.bpmn", model2);
		DynamicResourceProducer.addResource("b.bpmn", model3);

		applicationContext = new ClassPathXmlApplicationContext(CTX_DYNAMIC_DEPLOY_PATH);
		repositoryService = (RepositoryService) applicationContext.getBean("repositoryService");

		// then
		// Assertions come AFTER the file write! Otherwise the process file is messed up if the assertions fail.
		assertEquals(2, repositoryService.createDeploymentQuery().count());
		assertEquals(4, repositoryService.createProcessDefinitionQuery().count());
	  }

	  public virtual void testAutoDeployWithCreateDropOnCleanDb()
	  {
		createAppContext(CTX_CREATE_DROP_CLEAN_DB);
		assertEquals(1, repositoryService.createDeploymentQuery().count());
		assertEquals(3, repositoryService.createProcessDefinitionQuery().count());
	  }

	  public virtual void testAutoDeployTenantId()
	  {
		createAppContext(CTX_TENANT_ID_PATH);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.tenantIdIn("tenant1").count());
	  }

	  public virtual void testAutoDeployWithoutTenantId()
	  {
		createAppContext(CTX_CMMN_BPMN_TOGETHER_PATH);

		DeploymentQuery deploymentQuery = repositoryService.createDeploymentQuery();

		assertEquals(1, deploymentQuery.withoutTenantId().count());
	  }

	  public virtual void testAutoDeployCustomName()
	  {
		createAppContext(CTX_CUSTOM_NAME_PATH);

		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());
	  }

	  // --Helper methods ----------------------------------------------------------

	  private void removeAllDeployments()
	  {
		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
	  }

	}
}