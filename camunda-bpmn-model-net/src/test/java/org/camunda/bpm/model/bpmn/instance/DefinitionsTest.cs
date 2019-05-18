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
namespace org.camunda.bpm.model.bpmn.instance
{
	using BpmnDiagram = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnDiagram;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMNDI_NS;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class DefinitionsTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Import)), new ChildElementAssumption(typeof(Extension)), new ChildElementAssumption(typeof(RootElement)), new ChildElementAssumption(BPMNDI_NS, typeof(BpmnDiagram)), new ChildElementAssumption(typeof(Relationship))
		   );
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("id", true), new AttributeAssumption("name"), new AttributeAssumption("targetNamespace", false, true), new AttributeAssumption("expressionLanguage", false, false, "http://www.w3.org/1999/XPath"), new AttributeAssumption("typeLanguage", false, false, "http://www.w3.org/2001/XMLSchema"), new AttributeAssumption("exporter"), new AttributeAssumption("exporterVersion")
		   );
		  }
	  }
	}

}