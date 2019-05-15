using System;
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

	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CmmnDisabledTest : ResourceProcessEngineTestCase
	{

	  protected internal EmbeddedProcessApplication processApplication;

	  public CmmnDisabledTest() : base("org/camunda/bpm/application/impl/deployment/cmmn.disabled.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal virtual void setUp()
	  {
		processApplication = new EmbeddedProcessApplication();
		base.setUp();
	  }

	  public virtual void testCmmnDisabled()
	  {
		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").deploy();

		// process is deployed:
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertNotNull(processDefinition);
		assertEquals(1, processDefinition.Version);

		try
		{
		  repositoryService.createCaseDefinitionQuery().singleResult();
		  fail("Cmmn Disabled: It should not be possible to query for a case definition.");
		}
		catch (Exception)
		{
		  // expected
		}

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testVariableInstanceQuery()
	  {
		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").deploy();

		VariableMap variables = Variables.createVariables().putValue("my-variable", "a-value");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);

		// variable instance query
		IList<VariableInstance> result = runtimeService.createVariableInstanceQuery().list();
		assertEquals(1, result.Count);

		VariableInstance variableInstance = result[0];
		assertEquals("my-variable", variableInstance.Name);

		// get variable
		assertNotNull(runtimeService.getVariable(processInstance.Id, "my-variable"));

		// get variable local
		assertNotNull(runtimeService.getVariableLocal(processInstance.Id, "my-variable"));

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	}

}