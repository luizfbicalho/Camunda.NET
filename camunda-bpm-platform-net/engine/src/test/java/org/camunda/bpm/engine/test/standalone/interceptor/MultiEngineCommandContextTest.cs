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
namespace org.camunda.bpm.engine.test.standalone.interceptor
{
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MultiEngineCommandContextTest
	{

	  protected internal ProcessEngine engine1;
	  protected internal ProcessEngine engine2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void startEngines()
	  public virtual void startEngines()
	  {
		engine1 = createProcessEngine("engine1");
		engine2 = createProcessEngine("engine2");
		StartProcessInstanceOnEngineDelegate.ENGINES["engine1"] = engine1;
		StartProcessInstanceOnEngineDelegate.ENGINES["engine2"] = engine2;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeEngine1()
	  public virtual void closeEngine1()
	  {
		try
		{
		  engine1.close();
		}
		finally
		{
		  engine1 = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void closeEngine2()
	  public virtual void closeEngine2()
	  {
		try
		{
		  engine2.close();
		}
		finally
		{
		  engine2 = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void removeEngines()
	  public virtual void removeEngines()
	  {
		StartProcessInstanceOnEngineDelegate.ENGINES.Clear();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldOpenNewCommandContextWhenInteractingAccrossEngines()
	  public virtual void shouldOpenNewCommandContextWhenInteractingAccrossEngines()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance process1 = Bpmn.createExecutableProcess("process1").startEvent().serviceTask().camundaInputParameter("engineName", "engine2").camundaInputParameter("processKey", "process2").camundaClass(typeof(StartProcessInstanceOnEngineDelegate).FullName).endEvent().done();

		BpmnModelInstance process2 = Bpmn.createExecutableProcess("process2").startEvent().endEvent().done();

		// given
		engine1.RepositoryService.createDeployment().addModelInstance("process1.bpmn", process1).deploy();
		engine2.RepositoryService.createDeployment().addModelInstance("process2.bpmn", process2).deploy();

		// if
		engine1.RuntimeService.startProcessInstanceByKey("process1");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldOpenNewCommandContextWhenInteractingWithOtherEngineAndBack()
	  public virtual void shouldOpenNewCommandContextWhenInteractingWithOtherEngineAndBack()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance process1 = Bpmn.createExecutableProcess("process1").startEvent().serviceTask().camundaInputParameter("engineName", "engine2").camundaInputParameter("processKey", "process2").camundaClass(typeof(StartProcessInstanceOnEngineDelegate).FullName).endEvent().done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance process2 = Bpmn.createExecutableProcess("process2").startEvent().serviceTask().camundaInputParameter("engineName", "engine1").camundaInputParameter("processKey", "process3").camundaClass(typeof(StartProcessInstanceOnEngineDelegate).FullName).done();

		BpmnModelInstance process3 = Bpmn.createExecutableProcess("process3").startEvent().endEvent().done();

		// given
		engine1.RepositoryService.createDeployment().addModelInstance("process1.bpmn", process1).deploy();
		engine2.RepositoryService.createDeployment().addModelInstance("process2.bpmn", process2).deploy();
		engine1.RepositoryService.createDeployment().addModelInstance("process3.bpmn", process3).deploy();

		// if
		engine1.RuntimeService.startProcessInstanceByKey("process1");
	  }

	  private ProcessEngine createProcessEngine(string name)
	  {
		StandaloneInMemProcessEngineConfiguration processEngineConfiguration = new StandaloneInMemProcessEngineConfiguration();
		processEngineConfiguration.ProcessEngineName = name;
		processEngineConfiguration.JdbcUrl = string.Format("jdbc:h2:mem:{0}", name);
		return processEngineConfiguration.buildProcessEngine();
	  }

	}

}