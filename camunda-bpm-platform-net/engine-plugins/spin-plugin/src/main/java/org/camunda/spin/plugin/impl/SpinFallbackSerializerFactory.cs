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
namespace org.camunda.spin.plugin.impl
{

	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SpinFallbackSerializerFactory : VariableSerializerFactory
	{

	  public static readonly Pattern SPIN_SERIALIZER_NAME_PATTERN = Pattern.compile("spin://(.*)");

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer(String serializerName)
	  public virtual TypedValueSerializer<object> getSerializer(string serializerName)
	  {
		Matcher matcher = SPIN_SERIALIZER_NAME_PATTERN.matcher(serializerName);
		if (matcher.matches())
		{
		  string serializationFormat = matcher.group(1);
		  return new FallbackSpinObjectValueSerializer(serializationFormat);
		}
		else
		{
		  return null;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer(org.camunda.bpm.engine.variable.value.TypedValue value)
	  public virtual TypedValueSerializer<object> getSerializer(TypedValue value)
	  {
		if (value is ObjectValue)
		{
		  ObjectValue objectValue = (ObjectValue) value;
		  if (objectValue.SerializationDataFormat != null && !objectValue.Deserialized)
		  {
			return new FallbackSpinObjectValueSerializer(objectValue.SerializationDataFormat);
		  }
		}
		return null;
	  }
	}

}