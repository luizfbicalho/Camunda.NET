using System.Collections.Generic;
using System.IO;

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
	using ReflectUtil = org.camunda.bpm.model.xml.impl.util.ReflectUtil;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using Before = org.junit.Before;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractGatewayTest<G> : BpmnModelElementInstanceTest where G : Gateway
	{

	  protected internal G gateway;

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(Gateway), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption(CAMUNDA_NS, "asyncBefore", false, false, false), new AttributeAssumption(CAMUNDA_NS, "asyncAfter", false, false, false)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before @SuppressWarnings("unchecked") public void getGateway()
	  public virtual void getGateway()
	  {
		Stream inputStream = ReflectUtil.getResourceAsStream("org/camunda/bpm/model/bpmn/GatewaysTest.xml");
		ICollection<ModelElementInstance> elementInstances = Bpmn.readModelFromStream(inputStream).getModelElementsByType(modelElementType);
		assertThat(elementInstances).hasSize(1);
		gateway = (G) elementInstances.GetEnumerator().next();
		assertThat(gateway.GatewayDirection).isEqualTo(GatewayDirection.Mixed);
	  }

	}

}