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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	using DataStore = org.camunda.bpm.model.bpmn.instance.DataStore;
	using DataStoreReference = org.camunda.bpm.model.bpmn.instance.DataStoreReference;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Falko Menge
	/// </summary>
	public class DataStoreTest
	{

	  private static BpmnModelInstance modelInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void parseModel()
	  public static void parseModel()
	  {
		modelInstance = Bpmn.readModelFromStream(typeof(DataStoreTest).getResourceAsStream("DataStoreTest.bpmn"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDataStore()
	  public virtual void testGetDataStore()
	  {
		DataStore dataStore = modelInstance.getModelElementById("myDataStore");
		assertThat(dataStore).NotNull;
		assertThat(dataStore.Name).isEqualTo("My Data Store");
		assertThat(dataStore.Capacity).isEqualTo(23);
		assertThat(dataStore.Unlimited).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetDataStoreReference()
	  public virtual void testGetDataStoreReference()
	  {
		DataStoreReference dataStoreReference = modelInstance.getModelElementById("myDataStoreReference");
		DataStore dataStore = modelInstance.getModelElementById("myDataStore");
		assertThat(dataStoreReference).NotNull;
		assertThat(dataStoreReference.Name).isEqualTo("My Data Store Reference");
		assertThat(dataStoreReference.DataStore).isEqualTo(dataStore);
	  }
	}

}