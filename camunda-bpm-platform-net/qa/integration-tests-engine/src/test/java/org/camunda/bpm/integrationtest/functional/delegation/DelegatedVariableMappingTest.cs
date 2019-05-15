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
namespace org.camunda.bpm.integrationtest.functional.delegation
{
	using DelegateVarMapping = org.camunda.bpm.integrationtest.functional.delegation.beans.DelegateVarMapping;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest.initWebArchiveDeployment;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class DelegatedVariableMappingTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class DelegatedVariableMappingTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name : "mainDeployment")]
		public static WebArchive createProcessArchiveDeplyoment()
		{
		return initWebArchiveDeployment("mainDeployment.war").addClass(typeof(DelegateVarMapping)).addAsResource("org/camunda/bpm/integrationtest/functional/delegation/DelegatedVariableMappingTest.testCallSubProcessWithDelegatedVariableMapping.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/delegation/DelegatedVariableMappingTest.testCallSubProcessWithDelegatedVariableMappingExpression.bpmn20.xml");
		}

	  [Deployment(name : "calledDeployment")]
	  public static WebArchive createSecondProcessArchiveDeployment()
	  {
		return initWebArchiveDeployment("calledDeployment.war").addAsResource("org/camunda/bpm/integrationtest/functional/delegation/simpleSubProcess.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private javax.enterprise.inject.spi.BeanManager beanManager;
	  private BeanManager beanManager;

	  protected internal virtual void testDelegation()
	  {
		TaskQuery taskQuery = taskService.createTaskQuery();

		//when
		Task taskInSubProcess = taskQuery.singleResult();
		assertEquals("Task in subprocess", taskInSubProcess.Name);

		//then check value from input variable
		object inputVar = runtimeService.getVariable(taskInSubProcess.ProcessInstanceId, "TestInputVar");
		assertEquals("inValue", inputVar);

		//when completing the task in the subprocess, finishes the subprocess
		taskService.complete(taskInSubProcess.Id);
		Task taskAfterSubProcess = taskQuery.singleResult();
		assertEquals("Task after subprocess", taskAfterSubProcess.Name);

		//then check value from output variable
		ProcessInstance processInstance = runtimeService.createProcessInstanceQuery().singleResult();
		object outputVar = runtimeService.getVariable(processInstance.Id, "TestOutputVar");
		assertEquals("outValue", outputVar);

		//complete task after sub process
		taskService.complete(taskAfterSubProcess.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testDelegateVariableMapping()
	  public virtual void testDelegateVariableMapping()
	  {
		//given
		runtimeService.startProcessInstanceByKey("callSimpleSubProcess");
		testDelegation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("mainDeployment") public void testDelegateVariableMappingExpression()
	  public virtual void testDelegateVariableMappingExpression()
	  {
		runtimeService.startProcessInstanceByKey("callSubProcessExpr");
		testDelegation();
	  }

	}

}