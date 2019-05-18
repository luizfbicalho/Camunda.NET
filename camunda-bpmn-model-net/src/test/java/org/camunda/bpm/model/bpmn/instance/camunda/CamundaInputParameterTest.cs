using System;
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
namespace org.camunda.bpm.model.bpmn.instance.camunda
{
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaInputParameterTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(CAMUNDA_NS, false);
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
			return Arrays.asList(new AttributeAssumption(CAMUNDA_NS, "name", false, true)
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("Test ignored. CAM-9441: Bug fix needed") @Test public void testIntputParameterScriptChildAssignment()
	  public virtual void testIntputParameterScriptChildAssignment()
	  {
		try
		{
		  CamundaInputParameter inputParamElement = modelInstance.newInstance(typeof(CamundaInputParameter));
		  inputParamElement.CamundaName = "aVariable";

		  CamundaScript scriptElement = modelInstance.newInstance(typeof(CamundaScript));
		  scriptElement.CamundaScriptFormat = "juel";
		  scriptElement.TextContent = "${'a script'}";

		  inputParamElement.addChildElement(scriptElement);
		}
		catch (Exception e)
		{
		  fail("CamundaScript should be accepted as a child element of CamundaInputParameter. Error: " + e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("Test ignored. CAM-9441: Bug fix needed") @Test public void testInputParameterListChildAssignment()
	  public virtual void testInputParameterListChildAssignment()
	  {
		try
		{
		  CamundaInputParameter inputParamElement = modelInstance.newInstance(typeof(CamundaInputParameter));
		  inputParamElement.CamundaName = "aVariable";

		  CamundaList listElement = modelInstance.newInstance(typeof(CamundaList));

		  inputParamElement.addChildElement(listElement);
		}
		catch (Exception e)
		{
		  fail("CamundaList should be accepted as a child element of CamundaInputParameter. Error: " + e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore("Test ignored. CAM-9441: Bug fix needed") @Test public void testInputParameterMapChildAssignment()
	  public virtual void testInputParameterMapChildAssignment()
	  {
		try
		{
		  CamundaInputParameter inputParamElement = modelInstance.newInstance(typeof(CamundaInputParameter));
		  inputParamElement.CamundaName = "aVariable";

		  CamundaMap listElement = modelInstance.newInstance(typeof(CamundaMap));

		  inputParamElement.addChildElement(listElement);
		}
		catch (Exception e)
		{
		  fail("CamundaMap should be accepted as a child element of CamundaInputParameter. Error: " + e.Message);
		}
	  }
	}

}