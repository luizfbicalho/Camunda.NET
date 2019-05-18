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
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class BaseElementTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(true);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Documentation)), new ChildElementAssumption(typeof(ExtensionElements), 0, 1)
		   );
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("id", true)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testId()
	  public virtual void testId()
	  {
		Task task = modelInstance.newInstance(typeof(Task));
		assertThat(task.Id).NotNull.StartsWith("task");
		task.Id = "test";
		assertThat(task.Id).isEqualTo("test");
		StartEvent startEvent = modelInstance.newInstance(typeof(StartEvent));
		assertThat(startEvent.Id).NotNull.StartsWith("startEvent");
		startEvent.Id = "test";
		assertThat(startEvent.Id).isEqualTo("test");
	  }

	}

}