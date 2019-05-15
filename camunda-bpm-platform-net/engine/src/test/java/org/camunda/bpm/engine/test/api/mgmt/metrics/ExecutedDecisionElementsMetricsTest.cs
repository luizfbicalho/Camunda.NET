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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using BusinessRuleTask = org.camunda.bpm.model.bpmn.instance.BusinessRuleTask;

	public class ExecutedDecisionElementsMetricsTest : AbstractMetricsTest
	{

	  public const string DMN_FILE = "org/camunda/bpm/engine/test/api/mgmt/metrics/ExecutedDecisionElementsTest.dmn11.xml";
	  public static VariableMap VARIABLES = Variables.createVariables().putValue("status", "").putValue("sum", 100);

	  protected internal override void clearMetrics()
	  {
		base.clearMetrics();
		processEngineConfiguration.DmnEngineConfiguration.EngineMetricCollector.clearExecutedDecisionElements();
	  }

	  public virtual void testBusinessRuleTask()
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("testProcess").startEvent().businessRuleTask("task").endEvent().done();

		BusinessRuleTask task = modelInstance.getModelElementById("task");
		task.CamundaDecisionRef = "decision";

		deploymentId = repositoryService.createDeployment().addModelInstance("process.bpmn", modelInstance).addClasspathResource(DMN_FILE).deploy().Id;

		assertEquals(0l, ExecutedDecisionElements);
		assertEquals(0l, ExecutedDecisionElementsFromDmnEngine);

		runtimeService.startProcessInstanceByKey("testProcess", VARIABLES);

		assertEquals(16l, ExecutedDecisionElements);
		assertEquals(16l, ExecutedDecisionElementsFromDmnEngine);

		processEngineConfiguration.DbMetricsReporter.reportNow();

		assertEquals(16l, ExecutedDecisionElements);
		assertEquals(16l, ExecutedDecisionElementsFromDmnEngine);
	  }

	  protected internal virtual long ExecutedDecisionElements
	  {
		  get
		  {
			return managementService.createMetricsQuery().name(Metrics.EXECUTED_DECISION_ELEMENTS).sum();
		  }
	  }

	  protected internal virtual long ExecutedDecisionElementsFromDmnEngine
	  {
		  get
		  {
			return processEngineConfiguration.DmnEngineConfiguration.EngineMetricCollector.ExecutedDecisionElements;
		  }
	  }

	}

}