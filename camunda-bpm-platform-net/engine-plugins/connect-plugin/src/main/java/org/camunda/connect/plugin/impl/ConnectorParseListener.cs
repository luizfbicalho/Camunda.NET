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
namespace org.camunda.connect.plugin.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseUtil.findCamundaExtensionElement;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseUtil.parseInputOutput;

	using BpmnParseException = org.camunda.bpm.engine.BpmnParseException;
	using AbstractBpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.AbstractBpmnParseListener;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;

	public class ConnectorParseListener : AbstractBpmnParseListener
	{

	  public override void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		Element connectorDefinition = findCamundaExtensionElement(serviceTaskElement, "connector");
		if (connectorDefinition != null)
		{
		  Element connectorIdElement = connectorDefinition.element("connectorId");

		  string connectorId = null;
		  if (connectorIdElement != null)
		  {
			connectorId = connectorIdElement.Text.Trim();
		  }
		  if (connectorIdElement == null || connectorId.Length == 0)
		  {
			throw new BpmnParseException("No 'id' defined for connector.", connectorDefinition);
		  }

		  IoMapping ioMapping = parseInputOutput(connectorDefinition);
		  activity.ActivityBehavior = new ServiceTaskConnectorActivityBehavior(connectorId, ioMapping);
		}
	  }

	}

}