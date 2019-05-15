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
namespace org.camunda.bpm.engine.spring.test.transaction.modification
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using LogFactory = org.apache.ibatis.logging.LogFactory;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using SpringJUnit4ClassRunner = org.springframework.test.context.junit4.SpringJUnit4ClassRunner;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(SpringJUnit4ClassRunner.class) @ContextConfiguration(locations = {"classpath:org/camunda/bpm/engine/spring/test/transaction/ProcessInstanceModificationInTransactionTest-applicationContext.xml"}) public class ProcessInstanceModificationInTransactionTest
	public class ProcessInstanceModificationInTransactionTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule;
		public ProcessEngineRule rule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired public org.camunda.bpm.engine.ProcessEngine processEngine;
	  public ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired RuntimeService runtimeService;
	  internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired RepositoryService repositoryService;
	  internal RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired UserBean userBean;
	  internal UserBean userBean;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		LogFactory.useSlf4jLogging();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBeAbleToPerformModification()
	  public virtual void shouldBeAbleToPerformModification()
	  {

		// given
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("TestProcess").startEvent().intermediateCatchEvent("TimerEvent").timerWithDate("${calculateTimerDate.execute(execution)}").camundaExecutionListenerDelegateExpression("end", "${deleteVariableListener}").endEvent().done();

		deployModelInstance(modelInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance procInst = runtimeService.startProcessInstanceByKey("TestProcess");
		ProcessInstance procInst = runtimeService.startProcessInstanceByKey("TestProcess");

		// when
		userBean.completeUserTaskAndModifyInstanceInOneTransaction(procInst);

		// then
		VariableInstance variable = rule.RuntimeService.createVariableInstanceQuery().processInstanceIdIn(procInst.Id).variableName("createDate").singleResult();
		assertNotNull(variable);
		HistoricVariableInstance historicVariable = rule.HistoryService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(variable.Name, historicVariable.Name);
		assertEquals(org.camunda.bpm.engine.history.HistoricVariableInstance_Fields.STATE_CREATED, historicVariable.State);
	  }

	  private void deployModelInstance(BpmnModelInstance modelInstance)
	  {
		DeploymentBuilder deploymentbuilder = repositoryService.createDeployment();
		deploymentbuilder.addModelInstance("process0.bpmn", modelInstance);
		Deployment deployment = deploymentbuilder.deploy();
		rule.manageDeployment(deployment);
	  }
	}

}