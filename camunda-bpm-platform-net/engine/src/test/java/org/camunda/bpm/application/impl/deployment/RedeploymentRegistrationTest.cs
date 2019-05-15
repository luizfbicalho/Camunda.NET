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
namespace org.camunda.bpm.application.impl.deployment
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;


	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using DecisionDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionEntity;
	using DecisionRequirementsDefinitionEntity = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class RedeploymentRegistrationTest
	public class RedeploymentRegistrationTest
	{
		private bool InstanceFieldsInitialized = false;

		public RedeploymentRegistrationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string DEPLOYMENT_NAME = "my-deployment";

	  protected internal const string BPMN_RESOURCE_1 = "org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml";
	  protected internal const string BPMN_RESOURCE_2 = "org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml";

	  protected internal const string CMMN_RESOURCE_1 = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
	  protected internal const string CMMN_RESOURCE_2 = "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn";

	  protected internal const string DMN_RESOURCE_1 = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDmnDeployment.dmn11.xml";
	  protected internal const string DMN_RESOURCE_2 = "org/camunda/bpm/engine/test/dmn/deployment/dmnScore.dmn11.xml";

	  protected internal const string DRD_RESOURCE_1 = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string DRD_RESOURCE_2 = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

	  protected internal EmbeddedProcessApplication processApplication;

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal RepositoryService repositoryService;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String resource1;
	  public string resource1;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public String resource2;
	  public string resource2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(2) public String definitionKey1;
	  public string definitionKey1;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(3) public String definitionKey2;
	  public string definitionKey2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(4) public TestProvider testProvider;
	  public TestProvider testProvider;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "scenario {index}") public static java.util.Collection<Object[]> scenarios()
	  public static ICollection<object[]> scenarios()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {BPMN_RESOURCE_1, BPMN_RESOURCE_2, "processOne", "processTwo", processDefinitionTestProvider()},
			new object[] {CMMN_RESOURCE_1, CMMN_RESOURCE_2, "oneTaskCase", "twoTaskCase", caseDefinitionTestProvider()},
			new object[] {DMN_RESOURCE_1, DMN_RESOURCE_2, "decision", "score-decision", decisionDefinitionTestProvider()},
			new object[] {DRD_RESOURCE_1, DRD_RESOURCE_2, "score", "dish", decisionRequirementsDefinitionTestProvider()}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void init()
	  {
		repositoryService = engineRule.RepositoryService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		processApplication = new EmbeddedProcessApplication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationNotFoundByDeploymentId()
	  public virtual void registrationNotFoundByDeploymentId()
	  {
		// given
		ProcessApplicationReference reference = processApplication.Reference;

		Deployment deployment1 = repositoryService.createDeployment(reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		assertEquals(reference, getProcessApplicationForDeployment(deployment1.Id));

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		assertNull(getProcessApplicationForDeployment(deployment2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationNotFoundByDefinition()
	  public virtual void registrationNotFoundByDefinition()
	  {
		// given

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// when
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string definitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then
		assertNull(getProcessApplicationForDefinition(definitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundByDeploymentId()
	  public virtual void registrationFoundByDeploymentId()
	  {
		// given
		ProcessApplicationReference reference1 = processApplication.Reference;

		Deployment deployment1 = repositoryService.createDeployment(reference1).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		assertEquals(reference1, getProcessApplicationForDeployment(deployment1.Id));

		// when
		ProcessApplicationReference reference2 = processApplication.Reference;

		Deployment deployment2 = repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// then
		assertEquals(reference2, getProcessApplicationForDeployment(deployment2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundFromPreviousDefinition()
	  public virtual void registrationFoundFromPreviousDefinition()
	  {
		// given
		ProcessApplicationReference reference = processApplication.Reference;
		Deployment deployment1 = repositoryService.createDeployment(reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// when
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string definitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then
		assertEquals(reference, getProcessApplicationForDefinition(definitionId));

		// and the reference is not cached
		assertNull(getProcessApplicationForDeployment(deployment2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundFromLatestDeployment()
	  public virtual void registrationFoundFromLatestDeployment()
	  {
		// given
		ProcessApplicationReference reference1 = processApplication.Reference;
		Deployment deployment1 = repositoryService.createDeployment(reference1).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// when
		ProcessApplicationReference reference2 = processApplication.Reference;
		Deployment deployment2 = repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string definitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then
		assertEquals(reference2, getProcessApplicationForDefinition(definitionId));
		assertEquals(reference2, getProcessApplicationForDeployment(deployment2.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundOnlyForOneProcessDefinition()
	  public virtual void registrationFoundOnlyForOneProcessDefinition()
	  {
		// given

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).addClasspathResource(resource2).deploy();

		// second deployment
		ProcessApplicationReference reference2 = processApplication.Reference;
		repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// when
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);
		string secondDefinitionId = getLatestDefinitionIdByKey(definitionKey2);

		// then
		assertEquals(reference2, getProcessApplicationForDefinition(firstDefinitionId));
		assertNull(getProcessApplicationForDefinition(secondDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundFromDifferentDeployment()
	  public virtual void registrationFoundFromDifferentDeployment()
	  {
		// given

		// first deployment
		ProcessApplicationReference reference1 = processApplication.Reference;
		Deployment deployment1 = repositoryService.createDeployment(reference1).name(DEPLOYMENT_NAME).addClasspathResource(resource1).addClasspathResource(resource2).deploy();

		// second deployment
		ProcessApplicationReference reference2 = processApplication.Reference;
		repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// when
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);
		string secondDefinitionId = getLatestDefinitionIdByKey(definitionKey2);

		// then
		assertEquals(reference2, getProcessApplicationForDefinition(firstDefinitionId));
		assertEquals(reference1, getProcessApplicationForDefinition(secondDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundFromSameDeployment()
	  public virtual void registrationFoundFromSameDeployment()
	  {
		// given

		// first deployment
		ProcessApplicationReference reference1 = processApplication.Reference;
		Deployment deployment1 = repositoryService.createDeployment(reference1).name(DEPLOYMENT_NAME).addClasspathResource(resource1).addClasspathResource(resource2).deploy();

		// second deployment
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource2).deploy();

		// when
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);
		string secondDefinitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then
		assertEquals(reference1, getProcessApplicationForDefinition(firstDefinitionId));
		assertEquals(reference1, getProcessApplicationForDefinition(secondDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundFromDifferentDeployments()
	  public virtual void registrationFoundFromDifferentDeployments()
	  {
		// given

		// first deployment
		ProcessApplicationReference reference1 = processApplication.Reference;
		Deployment deployment1 = repositoryService.createDeployment(reference1).name(DEPLOYMENT_NAME + "-1").addClasspathResource(resource1).deploy();

		// second deployment
		ProcessApplicationReference reference2 = processApplication.Reference;
		repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME + "-2").addClasspathResource(resource2).deploy();

		// when
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);
		string secondDefinitionId = getLatestDefinitionIdByKey(definitionKey2);

		// then
		assertEquals(reference1, getProcessApplicationForDefinition(firstDefinitionId));
		assertEquals(reference2, getProcessApplicationForDefinition(secondDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationNotFoundWhenDeletingDeployment()
	  public virtual void registrationNotFoundWhenDeletingDeployment()
	  {
		// given

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		ProcessApplicationReference reference2 = processApplication.Reference;
		Deployment deployment2 = repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// when (1)
		// third deployment
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then (1)
		assertEquals(reference2, getProcessApplicationForDefinition(firstDefinitionId));

		// when (2)
		deleteDeployment(deployment2);

		// then (2)
		assertNull(getProcessApplicationForDefinition(firstDefinitionId));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void registrationFoundAfterDiscardingDeploymentCache()
	  public virtual void registrationFoundAfterDiscardingDeploymentCache()
	  {
		// given

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		ProcessApplicationReference reference2 = processApplication.Reference;
		repositoryService.createDeployment(reference2).name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		// when (1)
		// third deployment
		repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		string firstDefinitionId = getLatestDefinitionIdByKey(definitionKey1);

		// then (1)
		assertEquals(reference2, getProcessApplicationForDefinition(firstDefinitionId));

		// when (2)
		discardDefinitionCache();

		// then (2)
		assertEquals(reference2, getProcessApplicationForDefinition(firstDefinitionId));
	  }

	  // helper ///////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  deleteDeployment(deployment);
		}
	  }

	  protected internal virtual void deleteDeployment(Deployment deployment)
	  {
		repositoryService.deleteDeployment(deployment.Id, true);
		engineRule.ManagementService.unregisterProcessApplication(deployment.Id, false);
	  }

	  protected internal virtual ProcessApplicationReference getProcessApplicationForDeployment(string deploymentId)
	  {
		ProcessApplicationManager processApplicationManager = processEngineConfiguration.ProcessApplicationManager;
		return processApplicationManager.getProcessApplicationForDeployment(deploymentId);
	  }

	  protected internal virtual void discardDefinitionCache()
	  {
		processEngineConfiguration.DeploymentCache.discardProcessDefinitionCache();
		processEngineConfiguration.DeploymentCache.discardCaseDefinitionCache();
		processEngineConfiguration.DeploymentCache.discardDecisionDefinitionCache();
		processEngineConfiguration.DeploymentCache.discardDecisionRequirementsDefinitionCache();
	  }

	  protected internal virtual string getLatestDefinitionIdByKey(string key)
	  {
		return testProvider.getLatestDefinitionIdByKey(repositoryService, key);
	  }

	  protected internal virtual ProcessApplicationReference getProcessApplicationForDefinition(string definitionId)
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(testProvider.createGetProcessApplicationCommand(definitionId));
	  }

	  private interface TestProvider
	  {
		Command<ProcessApplicationReference> createGetProcessApplicationCommand(string definitionId);

		string getLatestDefinitionIdByKey(RepositoryService repositoryService, string key);
	  }

	  protected internal static TestProvider processDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass();
	  }

	  private class TestProviderAnonymousInnerClass : TestProvider
	  {

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.interceptor.Command<org.camunda.bpm.application.ProcessApplicationReference> createGetProcessApplicationCommand(final String definitionId)
		  public Command<ProcessApplicationReference> createGetProcessApplicationCommand(string definitionId)
		  {
			return new CommandAnonymousInnerClass(this, definitionId);
		  }

		  private class CommandAnonymousInnerClass : Command<ProcessApplicationReference>
		  {
			  private readonly TestProviderAnonymousInnerClass outerInstance;

			  private string definitionId;

			  public CommandAnonymousInnerClass(TestProviderAnonymousInnerClass outerInstance, string definitionId)
			  {
				  this.outerInstance = outerInstance;
				  this.definitionId = definitionId;
			  }


			  public ProcessApplicationReference execute(CommandContext commandContext)
			  {
				ProcessEngineConfigurationImpl configuration = commandContext.ProcessEngineConfiguration;
				DeploymentCache deploymentCache = configuration.DeploymentCache;
				ProcessDefinitionEntity definition = deploymentCache.findDeployedProcessDefinitionById(definitionId);
				return ProcessApplicationContextUtil.getTargetProcessApplication(definition);
			  }
		  }

		  public string getLatestDefinitionIdByKey(RepositoryService repositoryService, string key)
		  {
			return repositoryService.createProcessDefinitionQuery().processDefinitionKey(key).latestVersion().singleResult().Id;
		  }

	  }

	  protected internal static TestProvider caseDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass2();
	  }

	  private class TestProviderAnonymousInnerClass2 : TestProvider
	  {

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.interceptor.Command<org.camunda.bpm.application.ProcessApplicationReference> createGetProcessApplicationCommand(final String definitionId)
		  public Command<ProcessApplicationReference> createGetProcessApplicationCommand(string definitionId)
		  {
			return new CommandAnonymousInnerClass2(this, definitionId);
		  }

		  private class CommandAnonymousInnerClass2 : Command<ProcessApplicationReference>
		  {
			  private readonly TestProviderAnonymousInnerClass2 outerInstance;

			  private string definitionId;

			  public CommandAnonymousInnerClass2(TestProviderAnonymousInnerClass2 outerInstance, string definitionId)
			  {
				  this.outerInstance = outerInstance;
				  this.definitionId = definitionId;
			  }


			  public ProcessApplicationReference execute(CommandContext commandContext)
			  {
				ProcessEngineConfigurationImpl configuration = commandContext.ProcessEngineConfiguration;
				DeploymentCache deploymentCache = configuration.DeploymentCache;
				CaseDefinitionEntity definition = deploymentCache.findDeployedCaseDefinitionById(definitionId);
				return ProcessApplicationContextUtil.getTargetProcessApplication(definition);
			  }
		  }

		  public string getLatestDefinitionIdByKey(RepositoryService repositoryService, string key)
		  {
			return repositoryService.createCaseDefinitionQuery().caseDefinitionKey(key).latestVersion().singleResult().Id;
		  }

	  }

	  protected internal static TestProvider decisionDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass3();
	  }

	  private class TestProviderAnonymousInnerClass3 : TestProvider
	  {

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.interceptor.Command<org.camunda.bpm.application.ProcessApplicationReference> createGetProcessApplicationCommand(final String definitionId)
		  public Command<ProcessApplicationReference> createGetProcessApplicationCommand(string definitionId)
		  {
			return new CommandAnonymousInnerClass3(this, definitionId);
		  }

		  private class CommandAnonymousInnerClass3 : Command<ProcessApplicationReference>
		  {
			  private readonly TestProviderAnonymousInnerClass3 outerInstance;

			  private string definitionId;

			  public CommandAnonymousInnerClass3(TestProviderAnonymousInnerClass3 outerInstance, string definitionId)
			  {
				  this.outerInstance = outerInstance;
				  this.definitionId = definitionId;
			  }


			  public ProcessApplicationReference execute(CommandContext commandContext)
			  {
				ProcessEngineConfigurationImpl configuration = commandContext.ProcessEngineConfiguration;
				DeploymentCache deploymentCache = configuration.DeploymentCache;
				DecisionDefinitionEntity definition = deploymentCache.findDeployedDecisionDefinitionById(definitionId);
				return ProcessApplicationContextUtil.getTargetProcessApplication(definition);
			  }
		  }

		  public string getLatestDefinitionIdByKey(RepositoryService repositoryService, string key)
		  {
			return repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(key).latestVersion().singleResult().Id;
		  }

	  }

	  protected internal static TestProvider decisionRequirementsDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass4();
	  }

	  private class TestProviderAnonymousInnerClass4 : TestProvider
	  {

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.interceptor.Command<org.camunda.bpm.application.ProcessApplicationReference> createGetProcessApplicationCommand(final String definitionId)
		  public Command<ProcessApplicationReference> createGetProcessApplicationCommand(string definitionId)
		  {
			return new CommandAnonymousInnerClass4(this, definitionId);
		  }

		  private class CommandAnonymousInnerClass4 : Command<ProcessApplicationReference>
		  {
			  private readonly TestProviderAnonymousInnerClass4 outerInstance;

			  private string definitionId;

			  public CommandAnonymousInnerClass4(TestProviderAnonymousInnerClass4 outerInstance, string definitionId)
			  {
				  this.outerInstance = outerInstance;
				  this.definitionId = definitionId;
			  }


			  public ProcessApplicationReference execute(CommandContext commandContext)
			  {
				ProcessEngineConfigurationImpl configuration = commandContext.ProcessEngineConfiguration;
				DeploymentCache deploymentCache = configuration.DeploymentCache;
				DecisionRequirementsDefinitionEntity definition = deploymentCache.findDeployedDecisionRequirementsDefinitionById(definitionId);
				return ProcessApplicationContextUtil.getTargetProcessApplication(definition);
			  }
		  }

		  public string getLatestDefinitionIdByKey(RepositoryService repositoryService, string key)
		  {
			return repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(key).latestVersion().singleResult().Id;
		  }

	  }

	}

}