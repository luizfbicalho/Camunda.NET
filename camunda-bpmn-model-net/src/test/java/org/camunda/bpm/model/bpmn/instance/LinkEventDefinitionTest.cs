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
	using Source = org.camunda.bpm.model.bpmn.impl.instance.Source;
	using Target = org.camunda.bpm.model.bpmn.impl.instance.Target;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	public class LinkEventDefinitionTest : AbstractEventDefinitionTest
	{

	  public override ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Source)), new ChildElementAssumption(typeof(Target), 0, 1)
		   );
		  }
	  }

	  public override ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("name", false, true)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getEventDefinition()
	  public virtual void getEventDefinition()
	  {
		LinkEventDefinition eventDefinition = eventDefinitionQuery.filterByType(typeof(LinkEventDefinition)).singleResult();
		assertThat(eventDefinition).NotNull;
		assertThat(eventDefinition.Name).isEqualTo("link");
		assertThat(eventDefinition.Sources.GetEnumerator().next().Name).isEqualTo("link");
		assertThat(eventDefinition.Target.Name).isEqualTo("link");
	  }

	}

}