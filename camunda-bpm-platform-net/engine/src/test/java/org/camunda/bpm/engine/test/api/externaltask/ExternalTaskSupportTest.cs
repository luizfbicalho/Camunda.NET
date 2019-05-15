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
namespace org.camunda.bpm.engine.test.api.externaltask
{

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ExternalTaskSupportTest
	public class ExternalTaskSupportTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
		public ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> processResources()
	  public static ICollection<object[]> processResources()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskSupportTest.businessRuleTask.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskSupportTest.messageEndEvent.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskSupportTest.messageIntermediateEvent.bpmn20.xml"},
			new object[] {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskSupportTest.sendTask.bpmn20.xml"}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter public String processDefinitionResource;
	  public string processDefinitionResource;

	  protected internal string deploymentId;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		deploymentId = rule.RepositoryService.createDeployment().addClasspathResource(processDefinitionResource).deploy().Id;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  rule.RepositoryService.deleteDeployment(deploymentId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExternalTaskSupport()
	  public virtual void testExternalTaskSupport()
	  {
		// given
		ProcessDefinition processDefinition = rule.RepositoryService.createProcessDefinitionQuery().singleResult();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(processDefinition.Id);

		// then
		IList<LockedExternalTask> externalTasks = rule.ExternalTaskService.fetchAndLock(1, "aWorker").topic("externalTaskTopic", 5000L).execute();

		Assert.assertEquals(1, externalTasks.Count);
		Assert.assertEquals(processInstance.Id, externalTasks[0].ProcessInstanceId);

		// and it is possible to complete the external task successfully and end the process instance
		rule.ExternalTaskService.complete(externalTasks[0].Id, "aWorker");

		Assert.assertEquals(0L, rule.RuntimeService.createProcessInstanceQuery().count());
	  }
	}

}