using System;

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
namespace org.camunda.bpm.model.bpmn.util
{
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelInstance = org.camunda.bpm.model.xml.ModelInstance;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using GetModelElementTypeRule = org.camunda.bpm.model.xml.test.GetModelElementTypeRule;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class GetBpmnModelElementTypeRule : TestWatcher, GetModelElementTypeRule
	{

	  private ModelInstance modelInstance;
	  private Model model;
	  private ModelElementType modelElementType;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings("unchecked") protected void starting(org.junit.runner.Description description)
	  protected internal override void starting(Description description)
	  {
		string className = description.ClassName;
		className = className.replaceAll("Test", "");
		Type instanceClass = null;
		try
		{
		  instanceClass = (Type) Type.GetType(className);
		}
		catch (ClassNotFoundException e)
		{
		  throw new Exception(e);
		}
		modelInstance = Bpmn.createEmptyModel();
		model = modelInstance.Model;
		modelElementType = model.getType(instanceClass);
	  }

	  public virtual ModelInstance ModelInstance
	  {
		  get
		  {
			return modelInstance;
		  }
	  }

	  public virtual Model Model
	  {
		  get
		  {
			return model;
		  }
	  }

	  public virtual ModelElementType ModelElementType
	  {
		  get
		  {
			return modelElementType;
		  }
	  }
	}

}