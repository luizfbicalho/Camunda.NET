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
namespace org.camunda.bpm.engine.experimental
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using BusinessProcess = org.camunda.bpm.engine.cdi.BusinessProcess;
	using CdiProcessEngineTestCase = org.camunda.bpm.engine.cdi.test.CdiProcessEngineTestCase;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;

	public class ProcessVariablesTest : CdiProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test @Deployment(resources = "org/activiti/cdi/BusinessProcessBeanTest.test.bpmn20.xml") public void testResolveString()
	  [Deployment(resources : "org/activiti/cdi/BusinessProcessBeanTest.test.bpmn20.xml")]
	  public virtual void testResolveString()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		IDictionary<string, object> processVariables = new Dictionary<string, object>();
		businessProcess.setVariable("testKeyString", "testValue");
		businessProcess.startProcessByKey("businessProcessBeanTest", processVariables);
		businessProcess.startTask(taskService.createTaskQuery().singleResult().Id);

		InjectProcessVariable injectProcessVariables = getBeanInstance(typeof(InjectProcessVariable));
		assertEquals("testValue", injectProcessVariables.testKeyString);

		businessProcess.completeTask();
	  }

	}

}