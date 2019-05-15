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
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using CaseService = org.camunda.bpm.engine.CaseService;
	using DecisionService = org.camunda.bpm.engine.DecisionService;
	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class RedeploymentProcessApplicationTest
	public class RedeploymentProcessApplicationTest
	{

	  protected internal const string DEPLOYMENT_NAME = "my-deployment";

	  protected internal const string BPMN_RESOURCE_1 = "org/camunda/bpm/engine/test/api/repository/processOne.bpmn20.xml";
	  protected internal const string BPMN_RESOURCE_2 = "org/camunda/bpm/engine/test/api/repository/processTwo.bpmn20.xml";

	  protected internal const string CMMN_RESOURCE_1 = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
	  protected internal const string CMMN_RESOURCE_2 = "org/camunda/bpm/engine/test/api/cmmn/twoTaskCase.cmmn";

	  protected internal const string DMN_RESOURCE_1 = "org/camunda/bpm/engine/test/dmn/deployment/DecisionDefinitionDeployerTest.testDmnDeployment.dmn11.xml";
	  protected internal const string DMN_RESOURCE_2 = "org/camunda/bpm/engine/test/dmn/deployment/dmnScore.dmn11.xml";

	  protected internal const string DRD_RESOURCE_1 = "org/camunda/bpm/engine/test/dmn/deployment/drdScore.dmn11.xml";
	  protected internal const string DRD_RESOURCE_2 = "org/camunda/bpm/engine/test/dmn/deployment/drdDish.dmn11.xml";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal static RepositoryService repositoryService;
	  protected internal static RuntimeService runtimeService;
	  protected internal static CaseService caseService;
	  protected internal static DecisionService decisionService;
	  protected internal static ManagementService managementService;

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
		runtimeService = engineRule.RuntimeService;
		caseService = engineRule.CaseService;
		decisionService = engineRule.DecisionService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionOnePreviousDeploymentWithPA()
	  public virtual void definitionOnePreviousDeploymentWithPA()
	  {
		// given

		MyEmbeddedProcessApplication application = new MyEmbeddedProcessApplication(this);

		// first deployment
		Deployment deployment1 = repositoryService.createDeployment(application.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(2, testProvider.countDefinitionsByKey(definitionKey1));

		// when
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then
		assertTrue(application.Called);

		deleteDeployments(deployment1, deployment2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoPreviousDeploymentWithPA()
	  public virtual void definitionTwoPreviousDeploymentWithPA()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		MyEmbeddedProcessApplication application2 = new MyEmbeddedProcessApplication(this);
		Deployment deployment2 = repositoryService.createDeployment(application2.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(3, testProvider.countDefinitionsByKey(definitionKey1));

		// when
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then
		assertFalse(application1.Called);
		assertTrue(application2.Called);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoPreviousDeploymentFirstDeploymentWithPA()
	  public virtual void definitionTwoPreviousDeploymentFirstDeploymentWithPA()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment2 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(3, testProvider.countDefinitionsByKey(definitionKey1));

		// when
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then
		assertTrue(application1.Called);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoPreviousDeploymentDeleteSecondDeployment()
	  public virtual void definitionTwoPreviousDeploymentDeleteSecondDeployment()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		MyEmbeddedProcessApplication application2 = new MyEmbeddedProcessApplication(this);
		Deployment deployment2 = repositoryService.createDeployment(application2.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(3, testProvider.countDefinitionsByKey(definitionKey1));

		// when
		deleteDeployments(deployment2);
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then
		assertTrue(application1.Called);
		assertFalse(application2.Called);

		deleteDeployments(deployment1, deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoPreviousDeploymentUnregisterSecondPA()
	  public virtual void definitionTwoPreviousDeploymentUnregisterSecondPA()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		MyEmbeddedProcessApplication application2 = new MyEmbeddedProcessApplication(this);
		Deployment deployment2 = repositoryService.createDeployment(application2.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// second deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(3, testProvider.countDefinitionsByKey(definitionKey1));

		// when
		managementService.unregisterProcessApplication(deployment2.Id, true);
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then
		assertTrue(application1.Called);
		assertFalse(application2.Called);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoDifferentPreviousDeploymentsWithDifferentPA()
	  public virtual void definitionTwoDifferentPreviousDeploymentsWithDifferentPA()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME + "-1").addClasspathResource(resource1).deploy();

		// second deployment
		MyEmbeddedProcessApplication application2 = new MyEmbeddedProcessApplication(this);
		Deployment deployment2 = repositoryService.createDeployment(application2.Reference).name(DEPLOYMENT_NAME + "-2").addClasspathResource(resource2).deploy();

		// second deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME + "-3").addDeploymentResources(deployment1.Id).addDeploymentResources(deployment2.Id).deploy();

		assertEquals(2, testProvider.countDefinitionsByKey(definitionKey1));
		assertEquals(2, testProvider.countDefinitionsByKey(definitionKey2));

		// when (1)
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then (1)
		assertTrue(application1.Called);
		assertFalse(application2.Called);

		// reset flag
		application1.Called = false;

		// when (2)
		testProvider.createInstanceByDefinitionKey(definitionKey2);

		// then (2)
		assertFalse(application1.Called);
		assertTrue(application2.Called);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void definitionTwoPreviousDeploymentsWithDifferentPA()
	  public virtual void definitionTwoPreviousDeploymentsWithDifferentPA()
	  {
		// given

		// first deployment
		MyEmbeddedProcessApplication application1 = new MyEmbeddedProcessApplication(this);
		Deployment deployment1 = repositoryService.createDeployment(application1.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).addClasspathResource(resource2).deploy();

		// second deployment
		MyEmbeddedProcessApplication application2 = new MyEmbeddedProcessApplication(this);
		Deployment deployment2 = repositoryService.createDeployment(application2.Reference).name(DEPLOYMENT_NAME).addClasspathResource(resource1).deploy();

		// third deployment
		Deployment deployment3 = repositoryService.createDeployment().name(DEPLOYMENT_NAME).addDeploymentResources(deployment1.Id).deploy();

		assertEquals(3, testProvider.countDefinitionsByKey(definitionKey1));
		assertEquals(2, testProvider.countDefinitionsByKey(definitionKey2));

		// when (1)
		testProvider.createInstanceByDefinitionKey(definitionKey1);

		// then (1)
		assertFalse(application1.Called);
		assertTrue(application2.Called);

		// reset flag
		application2.Called = false;

		// when (2)
		testProvider.createInstanceByDefinitionKey(definitionKey2);

		// then (2)
		assertTrue(application1.Called);
		assertFalse(application2.Called);

		deleteDeployments(deployment1, deployment2, deployment3);
	  }

	  protected internal virtual void deleteDeployments(params Deployment[] deployments)
	  {
		foreach (Deployment deployment in deployments)
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		  managementService.unregisterProcessApplication(deployment.Id, false);
		}
	  }

	  protected internal interface TestProvider
	  {
		long countDefinitionsByKey(string definitionKey);

		void createInstanceByDefinitionKey(string definitionKey);
	  }

	  protected internal static TestProvider processDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass();
	  }

	  private class TestProviderAnonymousInnerClass : TestProvider
	  {

		  public long countDefinitionsByKey(string definitionKey)
		  {
			return repositoryService.createProcessDefinitionQuery().processDefinitionKey(definitionKey).count();
		  }

		  public void createInstanceByDefinitionKey(string definitionKey)
		  {
			runtimeService.startProcessInstanceByKey(definitionKey, Variables.createVariables().putValue("a", 1).putValue("b", 1));
		  }

	  }

	  protected internal static TestProvider caseDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass2();
	  }

	  private class TestProviderAnonymousInnerClass2 : TestProvider
	  {

		  public long countDefinitionsByKey(string definitionKey)
		  {
			return repositoryService.createCaseDefinitionQuery().caseDefinitionKey(definitionKey).count();
		  }

		  public void createInstanceByDefinitionKey(string definitionKey)
		  {
			caseService.createCaseInstanceByKey(definitionKey);
		  }

	  }

	  protected internal static TestProvider decisionDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass3();
	  }

	  private class TestProviderAnonymousInnerClass3 : TestProvider
	  {

		  public long countDefinitionsByKey(string definitionKey)
		  {
			return repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(definitionKey).count();
		  }

		  public void createInstanceByDefinitionKey(string definitionKey)
		  {
			decisionService.evaluateDecisionTableByKey(definitionKey).variables(Variables.createVariables().putValue("input", "john")).evaluate();
		  }

	  }

	  protected internal static TestProvider decisionRequirementsDefinitionTestProvider()
	  {
		return new TestProviderAnonymousInnerClass4();
	  }

	  private class TestProviderAnonymousInnerClass4 : TestProvider
	  {

		  public long countDefinitionsByKey(string definitionKey)
		  {
			return repositoryService.createDecisionRequirementsDefinitionQuery().decisionRequirementsDefinitionKey(definitionKey).count();
		  }

		  public void createInstanceByDefinitionKey(string definitionKey)
		  {
			decisionService.evaluateDecisionTableByKey(definitionKey + "-decision").variables(Variables.createVariables().putValue("temperature", 21).putValue("dayType", "Weekend").putValue("input", "John")).evaluate();
		  }

	  }

	  public class MyEmbeddedProcessApplication : EmbeddedProcessApplication
	  {
		  private readonly RedeploymentProcessApplicationTest outerInstance;

		  public MyEmbeddedProcessApplication(RedeploymentProcessApplicationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		protected internal ProcessApplicationReference reference;
		protected internal bool called;

		public override ProcessApplicationReference Reference
		{
			get
			{
			  if (reference == null)
			  {
				reference = base.Reference;
			  }
			  return reference;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public <T> T execute(java.util.concurrent.Callable<T> callable) throws org.camunda.bpm.application.ProcessApplicationExecutionException
		public override T execute<T>(Callable<T> callable)
		{
		  called = true;
		  return base.execute(callable);
		}

		public virtual bool Called
		{
			get
			{
			  return called;
			}
			set
			{
			  this.called = value;
			}
		}


	  }

	}

}