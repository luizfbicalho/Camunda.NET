using System;
using System.IO;

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
namespace org.camunda.bpm.engine.test.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TypedValueAssert
	{

	  public static void assertObjectValueDeserializedNull(ObjectValue typedValue)
	  {
		assertNotNull(typedValue);
		assertTrue(typedValue.Deserialized);
		assertNotNull(typedValue.SerializationDataFormat);
		assertNull(typedValue.Value);
		assertNull(typedValue.ValueSerialized);
		assertNull(typedValue.ObjectType);
		assertNull(typedValue.ObjectTypeName);
	  }

	  public static void assertObjectValueSerializedNull(ObjectValue typedValue)
	  {
		assertNotNull(typedValue);
		assertFalse(typedValue.Deserialized);
		assertNotNull(typedValue.SerializationDataFormat);
		assertNull(typedValue.ValueSerialized);
		assertNull(typedValue.ObjectTypeName);
	  }

	  public static void assertObjectValueDeserialized(ObjectValue typedValue, object value)
	  {
		Type expectedObjectType = value.GetType();
		assertTrue(typedValue.Deserialized);

		assertEquals(ValueType.OBJECT, typedValue.Type);

		assertEquals(value, typedValue.Value);
		assertEquals(value, typedValue.getValue(expectedObjectType));

		assertEquals(expectedObjectType, typedValue.ObjectType);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertEquals(expectedObjectType.FullName, typedValue.ObjectTypeName);
	  }

	  public static void assertObjectValueSerializedJava(ObjectValue typedValue, object value)
	  {
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, typedValue.SerializationDataFormat);

		try
		{
		  // validate this is the base 64 encoded string representation of the serialized value of the java object
		  string valueSerialized = typedValue.ValueSerialized;
		  sbyte[] decodedObject = Base64.decodeBase64(valueSerialized.GetBytes(Charset.forName("UTF-8")));
		  ObjectInputStream objectInputStream = new ObjectInputStream(new MemoryStream(decodedObject));
		  assertEquals(value, objectInputStream.readObject());
		}
		catch (IOException e)
		{
		  throw new Exception(e);
		}
		catch (ClassNotFoundException e)
		{
		  throw new Exception(e);
		}
	  }

	  public static void assertUntypedNullValue(TypedValue nullValue)
	  {
		assertNotNull(nullValue);
		assertNull(nullValue.Value);
		assertEquals(ValueType.NULL, nullValue.Type);
	  }


	}

}