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
	using ParseBpmnModelRule = org.camunda.bpm.model.bpmn.util.ParseBpmnModelRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmnModelTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.camunda.bpm.model.bpmn.util.ParseBpmnModelRule parseBpmnModelRule = new org.camunda.bpm.model.bpmn.util.ParseBpmnModelRule();
	  public readonly ParseBpmnModelRule parseBpmnModelRule = new ParseBpmnModelRule();

	  protected internal BpmnModelInstance bpmnModelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setup()
	  public virtual void setup()
	  {
		bpmnModelInstance = parseBpmnModelRule.BpmnModel;
	  }

	}

}