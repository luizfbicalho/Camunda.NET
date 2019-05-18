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
	using ConditionExpression = org.camunda.bpm.model.bpmn.instance.ConditionExpression;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ConditionalSequenceFlowTest
	{

	  protected internal BpmnModelInstance modelInstance;
	  protected internal SequenceFlow flow1;
	  protected internal SequenceFlow flow2;
	  protected internal SequenceFlow flow3;
	  protected internal ConditionExpression conditionExpression1;
	  protected internal ConditionExpression conditionExpression2;
	  protected internal ConditionExpression conditionExpression3;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void parseModel()
	  public virtual void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(this.GetType().getResourceAsStream(this.GetType().Name + ".xml"));
		flow1 = modelInstance.getModelElementById("flow1");
		flow2 = modelInstance.getModelElementById("flow2");
		flow3 = modelInstance.getModelElementById("flow3");
		conditionExpression1 = flow1.ConditionExpression;
		conditionExpression2 = flow2.ConditionExpression;
		conditionExpression3 = flow3.ConditionExpression;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveTypeTFormalExpression()
	  public virtual void shouldHaveTypeTFormalExpression()
	  {
		assertThat(conditionExpression1.Type).isEqualTo("tFormalExpression");
		assertThat(conditionExpression2.Type).isEqualTo("tFormalExpression");
		assertThat(conditionExpression3.Type).isEqualTo("tFormalExpression");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveLanguage()
	  public virtual void shouldHaveLanguage()
	  {
		assertThat(conditionExpression1.Language).Null;
		assertThat(conditionExpression2.Language).Null;
		assertThat(conditionExpression3.Language).isEqualTo("groovy");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveSourceCode()
	  public virtual void shouldHaveSourceCode()
	  {
		assertThat(conditionExpression1.TextContent).isEqualTo("test");
		assertThat(conditionExpression2.TextContent).isEqualTo("${test}");
		assertThat(conditionExpression3.TextContent).Empty;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldHaveResource()
	  public virtual void shouldHaveResource()
	  {
		assertThat(conditionExpression1.CamundaResource).Null;
		assertThat(conditionExpression2.CamundaResource).Null;
		assertThat(conditionExpression3.CamundaResource).isEqualTo("test.groovy");
	  }

	}

}