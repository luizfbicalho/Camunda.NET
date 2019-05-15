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
namespace org.camunda.bpm.engine.test.history.dmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using JavaSerializable = org.camunda.bpm.engine.test.api.variables.JavaSerializable;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameter = org.junit.runners.Parameterized.Parameter;
	using Parameters = org.junit.runners.Parameterized.Parameters;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) @RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL) public class HistoricDecisionInstanceInputOutputValueTest
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDecisionInstanceInputOutputValueTest
	{

	  protected internal const string DECISION_PROCESS = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.processWithBusinessRuleTask.bpmn20.xml";
	  protected internal const string DECISION_SINGLE_OUTPUT_DMN = "org/camunda/bpm/engine/test/history/HistoricDecisionInstanceTest.decisionSingleOutput.dmn11.xml";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters(name = "{index}: input({0}) = {1}") public static java.util.Collection<Object[]> data()
	  public static ICollection<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"string", "a"},
			new object[] {"long", 1L},
			new object[] {"double", 2.5},
			new object[] {"bytes", "object".GetBytes()},
			new object[] {"object", new JavaSerializable("foo")},
			new object[] {"object", Collections.singletonList(new JavaSerializable("bar"))}
		});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(0) public String valueType;
	  public string valueType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameter(1) public Object inputValue;
	  public object inputValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) public void decisionInputInstanceValue() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void decisionInputInstanceValue()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");
		DateTime fixedDate = sdf.parse("01/01/2001 01:01:01.000");
		ClockUtil.CurrentTime = fixedDate;

		startProcessInstanceAndEvaluateDecision(inputValue);

		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().includeInputs().singleResult();
		IList<HistoricDecisionInputInstance> inputInstances = historicDecisionInstance.Inputs;
		assertThat(inputInstances.Count, @is(1));

		HistoricDecisionInputInstance inputInstance = inputInstances[0];
		assertThat(inputInstance.TypeName, @is(valueType));
		assertThat(inputInstance.Value, @is(inputValue));
		assertThat(inputInstance.CreateTime, @is(fixedDate));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN }) public void decisionOutputInstanceValue() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : { DECISION_PROCESS, DECISION_SINGLE_OUTPUT_DMN })]
	  public virtual void decisionOutputInstanceValue()
	  {
		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");
		DateTime fixedDate = sdf.parse("01/01/2001 01:01:01.000");
		ClockUtil.CurrentTime = fixedDate;

		startProcessInstanceAndEvaluateDecision(inputValue);

		HistoricDecisionInstance historicDecisionInstance = engineRule.HistoryService.createHistoricDecisionInstanceQuery().includeOutputs().singleResult();
		IList<HistoricDecisionOutputInstance> outputInstances = historicDecisionInstance.Outputs;
		assertThat(outputInstances.Count, @is(1));

		HistoricDecisionOutputInstance outputInstance = outputInstances[0];
		assertThat(outputInstance.TypeName, @is(valueType));
		assertThat(outputInstance.Value, @is(inputValue));
		assertThat(outputInstance.CreateTime, @is(fixedDate));
	  }

	  protected internal virtual ProcessInstance startProcessInstanceAndEvaluateDecision(object input)
	  {
		return engineRule.RuntimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("input1", input));
	  }

	}

}