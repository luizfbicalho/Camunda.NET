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
	using BpmnModelConstants = org.camunda.bpm.model.bpmn.impl.BpmnModelConstants;
	using GetBpmnModelElementTypeRule = org.camunda.bpm.model.bpmn.util.GetBpmnModelElementTypeRule;
	using AbstractModelElementInstanceTest = org.camunda.bpm.model.xml.test.AbstractModelElementInstanceTest;
	using BeforeClass = org.junit.BeforeClass;
	using ClassRule = org.junit.ClassRule;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class BpmnModelElementInstanceTest : AbstractModelElementInstanceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static final org.camunda.bpm.model.bpmn.util.GetBpmnModelElementTypeRule modelElementTypeRule = new org.camunda.bpm.model.bpmn.util.GetBpmnModelElementTypeRule();
	  public static readonly GetBpmnModelElementTypeRule modelElementTypeRule = new GetBpmnModelElementTypeRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void initModelElementType()
	  public static void initModelElementType()
	  {
		initModelElementType(modelElementTypeRule);
	  }

	  public virtual string DefaultNamespace
	  {
		  get
		  {
			return BpmnModelConstants.BPMN20_NS;
		  }
	  }

	}

}