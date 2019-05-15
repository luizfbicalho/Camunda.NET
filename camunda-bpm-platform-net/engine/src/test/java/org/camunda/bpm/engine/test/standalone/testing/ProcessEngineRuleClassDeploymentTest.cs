﻿/*
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
namespace org.camunda.bpm.engine.test.standalone.testing
{

	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public class ProcessEngineRuleClassDeploymentTest
	public class ProcessEngineRuleClassDeploymentTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
		public readonly ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeploymentOnClassLevel()
	  public virtual void testDeploymentOnClassLevel()
	  {
		assertNotNull("process is not deployed",processEngineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("testHelperDeploymentTest").singleResult());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testDeploymentOnMethodOverridesClass()
	  public virtual void testDeploymentOnMethodOverridesClass()
	  {
		assertNotNull("process is not deployed",processEngineRule.RepositoryService.createProcessDefinitionQuery().processDefinitionKey("testHelperDeploymentTestOverride").singleResult());
	  }

	}

}