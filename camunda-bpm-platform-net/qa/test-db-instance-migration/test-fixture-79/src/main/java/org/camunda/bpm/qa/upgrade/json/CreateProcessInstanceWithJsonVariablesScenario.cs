using System.Collections;
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
namespace org.camunda.bpm.qa.upgrade.json
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using SerializedObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.SerializedObjectValueBuilder;
	using ObjectList = org.camunda.bpm.qa.upgrade.json.beans.ObjectList;
	using Order = org.camunda.bpm.qa.upgrade.json.beans.Order;
	using OrderDetails = org.camunda.bpm.qa.upgrade.json.beans.OrderDetails;
	using RegularCustomer = org.camunda.bpm.qa.upgrade.json.beans.RegularCustomer;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	public class CreateProcessInstanceWithJsonVariablesScenario
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static String deployProcess()
	  public static string deployProcess()
	  {
		return "org/camunda/bpm/qa/upgrade/json/simpleProcess.bpmn20.xml";
	  }

	  [DescribesScenario("initProcessInstanceWithDifferentVariables")]
	  public static ScenarioSetup initProcessInstance()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			// given
			ProcessInstance processInstance = engine.RuntimeService.startProcessInstanceByKey("Process", "processWithJsonVariables79");
			// when
			Execution execution = engine.RuntimeService.createExecutionQuery().processInstanceId(processInstance.Id).singleResult();
			engine.RuntimeService.setVariable(execution.Id, "objectVariable", createObjectVariable());
			engine.RuntimeService.setVariable(execution.Id, "plainTypeArrayVariable", createPlainTypeArray());
			engine.RuntimeService.setVariable(execution.Id, "notGenericObjectListVariable", createNotGenericObjectList());
			engine.RuntimeService.setVariable(execution.Id, "serializedMapVariable", createSerializedMap());

		  }
	  }

	  public static object createObjectVariable()
	  {
		Order order = new Order();
		order.Id = 1234567890987654321L;
		order.setOrder("order1");
		order.DueUntil = 20150112;
		order.Active = true;

		OrderDetails orderDetails = new OrderDetails();
		orderDetails.Article = "camundaBPM";
		orderDetails.Price = 32000.45;
		orderDetails.RoundedPrice = 32000;

		IList<string> currencies = new List<string>();
		currencies.Add("euro");
		currencies.Add("dollar");
		orderDetails.Currencies = currencies;

		order.OrderDetails = orderDetails;

		IList<RegularCustomer> customers = new List<RegularCustomer>();

		customers.Add(new RegularCustomer("Kermit", 1354539722));
		customers.Add(new RegularCustomer("Waldo", 1320325322));
		customers.Add(new RegularCustomer("Johnny", 1286110922));

		order.Customers = customers;

		return order;
	  }

	  public static int[] createPlainTypeArray()
	  {
		return new int[]{5, 10};
	  }

	  public static ObjectList createNotGenericObjectList()
	  {
		ObjectList customers = new ObjectList();
		customers.Add(new RegularCustomer("someCustomer", 5));
		customers.Add(new RegularCustomer("secondCustomer", 666));
		return customers;
	  }

	  public static SerializedObjectValueBuilder createSerializedMap()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return serializedObjectValue("{\"foo\": \"bar\"}").serializationDataFormat("application/json").objectTypeName(typeof(Hashtable).FullName);
	  }
	}

}