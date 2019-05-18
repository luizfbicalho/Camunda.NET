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
	using Definitions = org.camunda.bpm.model.bpmn.instance.Definitions;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Filip Hrisafov
	/// </summary>
	public class BpmnModelInstanceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClone() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testClone()
	  {

		BpmnModelInstance modelInstance = Bpmn.createEmptyModel();

		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.Id = "TestId";
		modelInstance.Definitions = definitions;

		BpmnModelInstance cloneInstance = modelInstance.clone();
		cloneInstance.Definitions.Id = "TestId2";

		assertThat(modelInstance.Definitions.Id, @is(equalTo("TestId")));
		assertThat(cloneInstance.Definitions.Id, @is(equalTo("TestId2")));
	  }

	}

}