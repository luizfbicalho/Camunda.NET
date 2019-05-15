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
namespace org.camunda.bpm.engine.rest.helper
{
	using ObjectValueImpl = org.camunda.bpm.engine.variable.impl.value.ObjectValueImpl;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;

	/// <summary>
	/// Subclass of <seealso cref="ObjectValueImpl"/> that allows to manipulate all of its properties
	/// from the outside such that it can be easily used when "mocking" (i.e. crafting them by hand) these values
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MockObjectValue : ObjectValueImpl
	{

	  private const long serialVersionUID = 1L;

	  public MockObjectValue(object value) : base(value)
	  {
	  }

	  public virtual MockObjectValue objectTypeName(string objectTypeName)
	  {
		this.objectTypeName = objectTypeName;
		return this;
	  }

	  public virtual MockObjectValue serializedValue(string serializedValue)
	  {
		this.serializedValue = serializedValue;
		return this;
	  }

	  public static MockObjectValue fromObjectValue(ObjectValue objectValue)
	  {
		MockObjectValue result = new MockObjectValue(objectValue.Value);

		result.isDeserialized = objectValue.Deserialized;
		result.objectTypeName = objectValue.ObjectTypeName;
		result.serializationDataFormat = objectValue.SerializationDataFormat;
		result.serializedValue = objectValue.ValueSerialized;
		result.type = objectValue.Type;

		return result;
	  }




	}

}