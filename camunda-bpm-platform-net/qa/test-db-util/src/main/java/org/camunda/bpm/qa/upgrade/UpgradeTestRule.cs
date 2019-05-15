using System;

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
namespace org.camunda.bpm.qa.upgrade
{
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using CaseExecutionQuery = org.camunda.bpm.engine.runtime.CaseExecutionQuery;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using MessageCorrelationBuilder = org.camunda.bpm.engine.runtime.MessageCorrelationBuilder;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Assert = org.junit.Assert;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class UpgradeTestRule : ProcessEngineRule
	{

	  protected internal string scenarioTestedByClass = null;
	  protected internal string scenarioName;
	  protected internal string tag;

	  public UpgradeTestRule() : base("camunda.cfg.xml")
	  {
	  }

	  public UpgradeTestRule(string configurationResource) : base(configurationResource)
	  {
	  }

	  public override void starting(Description description)
	  {
		Type testClass = description.TestClass;
		if (string.ReferenceEquals(scenarioTestedByClass, null))
		{
		  ScenarioUnderTest testScenarioClassAnnotation = testClass.getAnnotation(typeof(ScenarioUnderTest));
		  if (testScenarioClassAnnotation != null)
		  {
			scenarioTestedByClass = testScenarioClassAnnotation.value();
		  }
		}

		ScenarioUnderTest testScenarioAnnotation = description.getAnnotation(typeof(ScenarioUnderTest));
		if (testScenarioAnnotation != null)
		{
		  if (!string.ReferenceEquals(scenarioTestedByClass, null))
		  {
			scenarioName = scenarioTestedByClass + "." + testScenarioAnnotation.value();
		  }
		  else
		  {
			scenarioName = testScenarioAnnotation.value();
		  }
		}

		// method annotation overrides class annotation
		Origin originAnnotation = description.getAnnotation(typeof(Origin));
		if (originAnnotation == null)
		{
		  originAnnotation = testClass.getAnnotation(typeof(Origin));
		}

		if (originAnnotation != null)
		{
		  tag = originAnnotation.value();
		}

		if (string.ReferenceEquals(scenarioName, null))
		{
		  throw new Exception("Could not determine scenario under test for test " + description.DisplayName);
		}

		base.starting(description);
	  }

	  public virtual TaskQuery taskQuery()
	  {
		return taskService.createTaskQuery().processInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual ExecutionQuery executionQuery()
	  {
		return runtimeService.createExecutionQuery().processInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual JobQuery jobQuery()
	  {
		ProcessInstance instance = processInstance();
		return managementService.createJobQuery().processInstanceId(instance.Id);
	  }

	  public virtual JobDefinitionQuery jobDefinitionQuery()
	  {
		ProcessInstance instance = processInstance();
		return managementService.createJobDefinitionQuery().processDefinitionId(instance.ProcessDefinitionId);
	  }

	  public virtual ProcessInstanceQuery processInstanceQuery()
	  {
		return runtimeService.createProcessInstanceQuery().processInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual ProcessInstance processInstance()
	  {
		ProcessInstance instance = processInstanceQuery().singleResult();

		if (instance == null)
		{
		  throw new Exception("There is no process instance for scenario " + BuisnessKey);
		}

		return instance;
	  }

	  public virtual HistoricProcessInstance historicProcessInstance()
	  {
		HistoricProcessInstance historicProcessInstance = historyService.createHistoricProcessInstanceQuery().processInstanceBusinessKey(BuisnessKey).singleResult();

		if (historicProcessInstance == null)
		{
		  throw new Exception("There is no historic process instance for scenario " + BuisnessKey);
		}

		return historicProcessInstance;
	  }

	  public virtual MessageCorrelationBuilder messageCorrelation(string messageName)
	  {
		return runtimeService.createMessageCorrelation(messageName).processInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual void assertScenarioEnded()
	  {
		Assert.assertTrue("Process instance for scenario " + BuisnessKey + " should have ended", processInstanceQuery().singleResult() == null);
	  }

	  // case //////////////////////////////////////////////////
	  public virtual CaseInstanceQuery caseInstanceQuery()
	  {
		return caseService.createCaseInstanceQuery().caseInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual CaseExecutionQuery caseExecutionQuery()
	  {
		return caseService.createCaseExecutionQuery().caseInstanceBusinessKey(BuisnessKey);
	  }

	  public virtual CaseInstance caseInstance()
	  {
		CaseInstance instance = caseInstanceQuery().singleResult();

		if (instance == null)
		{
		  throw new Exception("There is no case instance for scenario " + BuisnessKey);
		}

		return instance;
	  }

	  public virtual string ScenarioName
	  {
		  get
		  {
			return scenarioName;
		  }
	  }

	  public virtual string BuisnessKey
	  {
		  get
		  {
			if (!string.ReferenceEquals(tag, null))
			{
			  return tag + '.' + scenarioName;
			}
			return scenarioName;
		  }
	  }

	  public virtual string Tag
	  {
		  get
		  {
			return tag;
		  }
		  set
		  {
			this.tag = value;
		  }
	  }


	}

}