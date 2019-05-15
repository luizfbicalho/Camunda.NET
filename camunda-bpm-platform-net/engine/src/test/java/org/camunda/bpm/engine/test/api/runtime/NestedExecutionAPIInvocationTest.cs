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
namespace org.camunda.bpm.engine.test.api.runtime
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class NestedExecutionAPIInvocationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule1 = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule1 = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule engine2BootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule("camunda.cfg.prefix_extended.xml");
	  public static ProcessEngineBootstrapRule engine2BootstrapRule = new ProcessEngineBootstrapRule("camunda.cfg.prefix_extended.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule2 = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule(engine2BootstrapRule);
	  public ProcessEngineRule engineRule2 = new ProvidedProcessEngineRule(engine2BootstrapRule);

	  public const string PROCESS_KEY_1 = "process";

	  public const string PROCESS_KEY_2 = "multiEngineProcess";

	  public const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_MODEL = Bpmn.createExecutableProcess(PROCESS_KEY_1).startEvent().userTask("waitState").serviceTask("startProcess").camundaClass(typeof(NestedProcessStartDelegate).FullName).endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  public static readonly BpmnModelInstance PROCESS_MODEL_2 = Bpmn.createExecutableProcess(PROCESS_KEY_2).startEvent().userTask("waitState").serviceTask("startProcess").camundaClass(typeof(StartProcessOnAnotherEngineDelegate).FullName).endEvent().done();

	  public static readonly BpmnModelInstance ONE_TASK_PROCESS_MODEL = Bpmn.createExecutableProcess(ONE_TASK_PROCESS_KEY).startEvent().userTask("waitState").endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {

		StartProcessOnAnotherEngineDelegate.engine = engine2BootstrapRule.ProcessEngine;
		NestedProcessStartDelegate.engine = engineRule1.ProcessEngine;

		// given
		Deployment deployment1 = engineRule1.RepositoryService.createDeployment().addModelInstance("foo.bpmn", PROCESS_MODEL).deploy();

		Deployment deployment2 = engineRule1.RepositoryService.createDeployment().addModelInstance("boo.bpmn", PROCESS_MODEL_2).deploy();

		engineRule1.manageDeployment(deployment1);
		engineRule1.manageDeployment(deployment2);

		Deployment deployment3 = engineRule2.ProcessEngine.RepositoryService.createDeployment().addModelInstance("joo.bpmn", ONE_TASK_PROCESS_MODEL).deploy();

		engineRule2.manageDeployment(deployment3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void clearEngineReference()
	  public virtual void clearEngineReference()
	  {
		StartProcessOnAnotherEngineDelegate.engine = null;
		NestedProcessStartDelegate.engine = null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWaitStateIsReachedOnNestedInstantiation()
	  public virtual void testWaitStateIsReachedOnNestedInstantiation()
	  {

		engineRule1.RuntimeService.startProcessInstanceByKey(PROCESS_KEY_1);
		string taskId = engineRule1.TaskService.createTaskQuery().singleResult().Id;

		// when
		engineRule1.TaskService.complete(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWaitStateIsReachedOnMultiEngine()
	  public virtual void testWaitStateIsReachedOnMultiEngine()
	  {

		engineRule1.RuntimeService.startProcessInstanceByKey(PROCESS_KEY_2);
		string taskId = engineRule1.TaskService.createTaskQuery().singleResult().Id;

		// when
		engineRule1.TaskService.complete(taskId);
	  }

	  public class StartProcessOnAnotherEngineDelegate : JavaDelegate
	  {

		public static ProcessEngine engine;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{

		  RuntimeService runtimeService = engine.RuntimeService;

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

		  // then the wait state is reached immediately after instantiation
		  ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		  ActivityInstance[] activityInstances = activityInstance.getActivityInstances("waitState");
		  Assert.assertEquals(1, activityInstances.Length);

		}
	  }

	  public class NestedProcessStartDelegate : JavaDelegate
	  {

		public static ProcessEngine engine;
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{

		  RuntimeService runtimeService = engine.RuntimeService;

		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");

		  // then the wait state is reached immediately after instantiation
		  ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);
		  ActivityInstance[] activityInstances = activityInstance.getActivityInstances("waitState");
		  Assert.assertEquals(1, activityInstances.Length);

		}
	  }
	}

}