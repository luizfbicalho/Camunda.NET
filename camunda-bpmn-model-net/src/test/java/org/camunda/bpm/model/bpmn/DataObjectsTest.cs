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
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	/// <summary>
	/// @author Dario Campagna
	/// </summary>
	public class DataObjectsTest
	{

	  private static BpmnModelInstance modelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void parseModel()
	  public static void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(typeof(DataObjectsTest).getResourceAsStream("DataObjectTest.bpmn"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDataObject()
	  public virtual void testGetDataObject()
	  {
		DataObject dataObject = modelInstance.getModelElementById("_21");
		ItemDefinition itemDefinition = modelInstance.getModelElementById("_100");
		assertThat(dataObject).NotNull;
		assertThat(dataObject.Name).isEqualTo("DataObject _21");
		assertThat(dataObject.Collection).False;
		assertThat(dataObject.ItemSubject).isEqualTo(itemDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDataObjectReference()
	  public virtual void testGetDataObjectReference()
	  {
		DataObjectReference dataObjectReference = modelInstance.getModelElementById("_dataRef_7");
		DataObject dataObject = modelInstance.getModelElementById("_7");
		assertThat(dataObjectReference).NotNull;
		assertThat(dataObjectReference.Name).Null;
		assertThat(dataObjectReference.DataObject).isEqualTo(dataObject);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDataObjectReferenceAsDataAssociationSource()
	  public virtual void testDataObjectReferenceAsDataAssociationSource()
	  {
		ScriptTask scriptTask = modelInstance.getModelElementById("_3");
		DataObjectReference dataObjectReference = modelInstance.getModelElementById("_dataRef_11");
		DataInputAssociation dataInputAssociation = scriptTask.DataInputAssociations.GetEnumerator().next();
		ICollection<ItemAwareElement> sources = dataInputAssociation.Sources;
		assertThat(sources.Count).isEqualTo(1);
		assertThat(sources.GetEnumerator().next()).isEqualTo(dataObjectReference);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDataObjectReferenceAsDataAssociationTarget()
	  public virtual void testDataObjectReferenceAsDataAssociationTarget()
	  {
		ScriptTask scriptTask = modelInstance.getModelElementById("_3");
		DataObjectReference dataObjectReference = modelInstance.getModelElementById("_dataRef_7");
		DataOutputAssociation dataOutputAssociation = scriptTask.DataOutputAssociations.GetEnumerator().next();
		assertThat(dataOutputAssociation.Target).isEqualTo(dataObjectReference);
	  }
	}

}