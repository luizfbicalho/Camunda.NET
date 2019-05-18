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
namespace org.camunda.bpm.model.bpmn
{
	using org.camunda.bpm.model.bpmn.instance;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	public class ModelTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateEmptyModel()
	  public virtual void testCreateEmptyModel()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();

		Definitions definitions = bpmnModelInstance.Definitions;
		assertThat(definitions).Null;

		definitions = bpmnModelInstance.newInstance(typeof(Definitions));
		bpmnModelInstance.Definitions = definitions;

		definitions = bpmnModelInstance.Definitions;
		assertThat(definitions).NotNull;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBaseTypeCalculation()
	  public virtual void testBaseTypeCalculation()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();
		Model model = bpmnModelInstance.Model;
		ICollection<ModelElementType> allBaseTypes = ModelUtil.calculateAllBaseTypes(model.getType(typeof(StartEvent)));
		assertThat(allBaseTypes).hasSize(5);

		allBaseTypes = ModelUtil.calculateAllBaseTypes(model.getType(typeof(MessageEventDefinition)));
		assertThat(allBaseTypes).hasSize(3);

		allBaseTypes = ModelUtil.calculateAllBaseTypes(model.getType(typeof(BaseElement)));
		assertThat(allBaseTypes).hasSize(0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExtendingTypeCalculation()
	  public virtual void testExtendingTypeCalculation()
	  {
		BpmnModelInstance bpmnModelInstance = Bpmn.createEmptyModel();
		Model model = bpmnModelInstance.Model;
		IList<ModelElementType> baseInstanceTypes = new List<ModelElementType>();
		baseInstanceTypes.Add(model.getType(typeof(Event)));
		baseInstanceTypes.Add(model.getType(typeof(CatchEvent)));
		baseInstanceTypes.Add(model.getType(typeof(ExtensionElements)));
		baseInstanceTypes.Add(model.getType(typeof(EventDefinition)));
		ICollection<ModelElementType> allExtendingTypes = ModelUtil.calculateAllExtendingTypes(bpmnModelInstance.Model, baseInstanceTypes);
		assertThat(allExtendingTypes).hasSize(17);
	  }

	}

}