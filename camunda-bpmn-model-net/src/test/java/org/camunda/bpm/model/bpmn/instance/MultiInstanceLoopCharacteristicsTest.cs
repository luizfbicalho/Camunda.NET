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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using LoopDataInputRef = org.camunda.bpm.model.bpmn.impl.instance.LoopDataInputRef;
	using LoopDataOutputRef = org.camunda.bpm.model.bpmn.impl.instance.LoopDataOutputRef;

	/// <summary>
	/// @author Filip Hrisafov
	/// </summary>
	public class MultiInstanceLoopCharacteristicsTest : BpmnModelElementInstanceTest
	{

	  public override TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(LoopCharacteristics), false);
		  }
	  }

	  public override ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(LoopCardinality), 0, 1), new ChildElementAssumption(typeof(LoopDataInputRef), 0, 1), new ChildElementAssumption(typeof(LoopDataOutputRef), 0, 1), new ChildElementAssumption(typeof(OutputDataItem), 0, 1), new ChildElementAssumption(typeof(InputDataItem), 0, 1), new ChildElementAssumption(typeof(ComplexBehaviorDefinition)), new ChildElementAssumption(typeof(CompletionCondition), 0, 1)
		   );
		  }
	  }

	  public override ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("isSequential", false, false, false), new AttributeAssumption("behavior", false, false, MultiInstanceFlowCondition.All), new AttributeAssumption("oneBehaviorEventRef"), new AttributeAssumption("noneBehaviorEventRef"), new AttributeAssumption(CAMUNDA_NS, "asyncBefore", false, false, false), new AttributeAssumption(CAMUNDA_NS, "asyncAfter", false, false, false), new AttributeAssumption(CAMUNDA_NS, "exclusive", false, false, true), new AttributeAssumption(CAMUNDA_NS, "collection"), new AttributeAssumption(CAMUNDA_NS, "elementVariable")
		   );
		  }
	  }
	}

}