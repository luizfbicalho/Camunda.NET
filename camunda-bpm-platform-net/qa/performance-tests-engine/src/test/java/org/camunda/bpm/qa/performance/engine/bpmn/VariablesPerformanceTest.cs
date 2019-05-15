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
namespace org.camunda.bpm.qa.performance.engine.bpmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE1;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE10;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE2;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE3;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE4;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE5;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE6;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE7;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE8;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.VARIABLE9;

	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using ProcessEnginePerformanceTestCase = org.camunda.bpm.qa.performance.engine.junit.ProcessEnginePerformanceTestCase;
	using StartProcessInstanceStep = org.camunda.bpm.qa.performance.engine.steps.StartProcessInstanceStep;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class VariablesPerformanceTest : ProcessEnginePerformanceTestCase
	{


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEventStringVar()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEventStringVar()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = "someValue";

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEvent10StringVars()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEvent10StringVars()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = "someValue";
		variables[VARIABLE2] = "someValue";
		variables[VARIABLE3] = "someValue";
		variables[VARIABLE4] = "someValue";
		variables[VARIABLE5] = "someValue";
		variables[VARIABLE6] = "someValue";
		variables[VARIABLE7] = "someValue";
		variables[VARIABLE8] = "someValue";
		variables[VARIABLE9] = "someValue";
		variables[VARIABLE10] = "someValue";

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEventStringVar2()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEventStringVar2()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = "Some Text which is considerably longer than the first one.";

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEventDoubleVar()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEventDoubleVar()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = 2d;

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEventByteVar()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEventByteVar()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = "This string will be saved as a byte array.".GetBytes();

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEvent10ByteVars()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEvent10ByteVars()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		variables[VARIABLE1] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE2] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE3] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE4] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE5] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE6] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE7] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE8] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE9] = "This string will be saved as a byte array.".GetBytes();
		variables[VARIABLE10] = "This string will be saved as a byte array.".GetBytes();

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"}) public void noneStartEventLargeByteVar()
	  [Deployment(resources : {"org/camunda/bpm/qa/performance/engine/bpmn/StartEventPerformanceTest.noneStartEvent.bpmn"})]
	  public virtual void noneStartEventLargeByteVar()
	  {
		Dictionary<string, object> variables = new Dictionary<string, object>();
		sbyte[] bytes = new sbyte[5 * 1024];
		variables[VARIABLE1] = bytes;

		performanceTest().step(new StartProcessInstanceStep(engine, "process", variables)).run();
	  }

	}

}