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
namespace org.camunda.bpm.model.bpmn.instance.camunda
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.BpmnTestConstants.PROCESS_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using BpmnModelConstants = org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
	using ProcessImpl = org.camunda.bpm.model.bpmn.impl.instance.ProcessImpl;
	using Test = org.junit.Test;

	/// <summary>
	/// Test to check the interoperability when changing elements and attributes with
	/// the <seealso cref="BpmnModelConstants#ACTIVITI_NS"/>. In contrast to
	/// <seealso cref="CamundaExtensionsTest"/> this test uses directly the get*Ns() methods to
	/// check the expected value.
	/// 
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class CompatabilityTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingElementWithActivitiNsKeepsIt()
	  public virtual void modifyingElementWithActivitiNsKeepsIt()
	  {
		BpmnModelInstance modelInstance = Bpmn.readModelFromStream(typeof(CamundaExtensionsTest).getResourceAsStream("CamundaExtensionsCompatabilityTest.xml"));
		ProcessImpl process = modelInstance.getModelElementById(PROCESS_ID);
		ExtensionElements extensionElements = process.ExtensionElements;
		ICollection<CamundaExecutionListener> listeners = extensionElements.getChildElementsByType(typeof(CamundaExecutionListener));
		string listenerClass = "org.foo.Bar";
		foreach (CamundaExecutionListener listener in listeners)
		{
		  listener.CamundaClass = listenerClass;
		}
		foreach (CamundaExecutionListener listener in listeners)
		{
		  assertThat(listener.getAttributeValueNs(BpmnModelConstants.ACTIVITI_NS, "class"), @is(listenerClass));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void modifyingAttributeWithActivitiNsKeepsIt()
	  public virtual void modifyingAttributeWithActivitiNsKeepsIt()
	  {
		BpmnModelInstance modelInstance = Bpmn.readModelFromStream(typeof(CamundaExtensionsTest).getResourceAsStream("CamundaExtensionsCompatabilityTest.xml"));
		ProcessImpl process = modelInstance.getModelElementById(PROCESS_ID);
		string priority = "9000";
		process.CamundaJobPriority = priority;
		process.CamundaTaskPriority = priority;
		int? historyTimeToLive = 10;
		process.CamundaHistoryTimeToLive = historyTimeToLive;
		process.CamundaIsStartableInTasklist = false;
		process.CamundaVersionTag = "v1.0.0";
		assertThat(process.getAttributeValueNs(BpmnModelConstants.ACTIVITI_NS, "jobPriority"), @is(priority));
		assertThat(process.getAttributeValueNs(BpmnModelConstants.ACTIVITI_NS, "taskPriority"), @is(priority));
		assertThat(process.getAttributeValueNs(BpmnModelConstants.ACTIVITI_NS, "historyTimeToLive"), @is(historyTimeToLive.ToString()));
		assertThat(process.CamundaStartableInTasklist, @is(false));
		assertThat(process.CamundaVersionTag, @is("v1.0.0"));
	  }

	}

}