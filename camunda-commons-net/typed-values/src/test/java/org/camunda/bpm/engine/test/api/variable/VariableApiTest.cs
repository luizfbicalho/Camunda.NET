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
namespace org.camunda.bpm.engine.test.api.variable
{
	using static org.camunda.bpm.engine.variable.Variables;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;


	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class VariableApiTest
	{

	  private const string DESERIALIZED_OBJECT_VAR_NAME = "deserializedObject";
	  private static readonly ExampleObject DESERIALIZED_OBJECT_VAR_VALUE = new ExampleObject();

	  private const string SERIALIZATION_DATA_FORMAT_NAME = "data-format-name";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateObjectVariables()
	  public virtual void testCreateObjectVariables()
	  {

		VariableMap variables = createVariables().putValue(DESERIALIZED_OBJECT_VAR_NAME, objectValue(DESERIALIZED_OBJECT_VAR_VALUE));

		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE, variables[DESERIALIZED_OBJECT_VAR_NAME]);
		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE, variables.getValue(DESERIALIZED_OBJECT_VAR_NAME, typeof(ExampleObject)));

		object untypedValue = variables.getValueTyped(DESERIALIZED_OBJECT_VAR_NAME).Value;
		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE, untypedValue);

		ExampleObject typedValue = variables.getValueTyped<ObjectValue>(DESERIALIZED_OBJECT_VAR_NAME).getValue(typeof(ExampleObject));
		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE, typedValue);

		// object type name is not yet available
		assertNull(variables.getValueTyped<ObjectValue>(DESERIALIZED_OBJECT_VAR_NAME).ObjectTypeName);
		// class is available
		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE.GetType(), variables.getValueTyped<ObjectValue>(DESERIALIZED_OBJECT_VAR_NAME).ObjectType);


		variables = createVariables().putValue(DESERIALIZED_OBJECT_VAR_NAME, objectValue(DESERIALIZED_OBJECT_VAR_VALUE).serializationDataFormat(SERIALIZATION_DATA_FORMAT_NAME));

		assertEquals(DESERIALIZED_OBJECT_VAR_VALUE, variables[DESERIALIZED_OBJECT_VAR_NAME]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableMapWithoutCreateVariables()
	  public virtual void testVariableMapWithoutCreateVariables()
	  {
		VariableMap map1 = putValue("foo", true).putValue("bar", 20);
		VariableMap map2 = putValueTyped("foo", booleanValue(true)).putValue("bar", integerValue(20));

		assertEquals(map1, map2);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		assertTrue(map1.Values.containsAll(map2.Values));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableMapCompatibility()
	  public virtual void testVariableMapCompatibility()
	  {

		// test compatibility with Map<String, Object>
		VariableMap map1 = createVariables().putValue("foo", 10).putValue("bar", 20);

		// assert the map is assignable to Map<String,Object>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") java.util.Map<String, Object> assignable = map1;
		IDictionary<string, object> assignable = map1;

		VariableMap map2 = createVariables().putValueTyped("foo", integerValue(10)).putValueTyped("bar", integerValue(20));

		IDictionary<string, object> map3 = new Dictionary<string, object>();
		map3["foo"] = 10;
		map3["bar"] = 20;

		// equals()
		assertEquals(map1, map2);
		assertEquals(map2, map3);
		assertEquals(map1, fromMap(map1));
		assertEquals(map1, fromMap(map3));

		// hashCode()
		assertEquals(map1.GetHashCode(), map2.GetHashCode());
		assertEquals(map2.GetHashCode(), map3.GetHashCode());

		// values()
		VariableMap.ValueCollection values1 = map1.Values;
		VariableMap.ValueCollection values2 = map2.Values;
		IDictionary<string, object>.ValueCollection values3 = map3.Values;
		assertTrue(values1.containsAll(values2));
		assertTrue(values2.containsAll(values1));
		assertTrue(values2.containsAll(values3));
		assertTrue(values3.containsAll(values2));

		// entry set
		assertEquals(map1.SetOfKeyValuePairs(), map2.SetOfKeyValuePairs());
		assertEquals(map2.SetOfKeyValuePairs(), map3.SetOfKeyValuePairs());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSerializationDataFormats()
	  public virtual void testSerializationDataFormats()
	  {
		ObjectValue objectValue = objectValue(DESERIALIZED_OBJECT_VAR_VALUE).serializationDataFormat(Variables.SerializationDataFormats.JAVA).create();
		assertEquals(Variables.SerializationDataFormats.JAVA.Name, objectValue.SerializationDataFormat);

		objectValue = objectValue(DESERIALIZED_OBJECT_VAR_VALUE).serializationDataFormat(Variables.SerializationDataFormats.JSON).create();
		assertEquals(Variables.SerializationDataFormats.JSON.Name, objectValue.SerializationDataFormat);

		objectValue = objectValue(DESERIALIZED_OBJECT_VAR_VALUE).serializationDataFormat(Variables.SerializationDataFormats.XML).create();
		assertEquals(Variables.SerializationDataFormats.XML.Name, objectValue.SerializationDataFormat);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyVariableMapAsVariableContext()
	  public virtual void testEmptyVariableMapAsVariableContext()
	  {
		VariableContext varContext = createVariables().asVariableContext();
		assertTrue(varContext.Keys.Count == 0);
		assertNull(varContext.resolve("nonExisting"));
		assertFalse(varContext.containsVariable("nonExisting"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyVariableContext()
	  public virtual void testEmptyVariableContext()
	  {
		VariableContext varContext = emptyVariableContext();
		assertTrue(varContext.Keys.Count == 0);
		assertNull(varContext.resolve("nonExisting"));
		assertFalse(varContext.containsVariable("nonExisting"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableMapAsVariableContext()
	  public virtual void testVariableMapAsVariableContext()
	  {
		VariableContext varContext = createVariables().putValueTyped("someValue", integerValue(1)).asVariableContext();

		assertTrue(varContext.Keys.Count == 1);

		assertNull(varContext.resolve("nonExisting"));
		assertFalse(varContext.containsVariable("nonExisting"));

		assertEquals(1, varContext.resolve("someValue").Value);
		assertTrue(varContext.containsVariable("someValue"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTransientVariables() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTransientVariables()
	  {
		VariableMap variableMap = createVariables().putValueTyped("foo", doubleValue(10.0, true)).putValueTyped("bar", integerValue(10, true)).putValueTyped("aa", booleanValue(true, true)).putValueTyped("bb", stringValue("bb", true)).putValueTyped("test", byteArrayValue("test".GetBytes(), true)).putValueTyped("blob", fileValue(new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/variables/simpleFile.txt").toURI()), true)).putValueTyped("val", dateValue(DateTime.Now, true)).putValueTyped("var", objectValue(new int?(10), true).create()).putValueTyped("short", shortValue((short)12, true)).putValueTyped("long", longValue((long)10, true)).putValueTyped("file", fileValue("org/camunda/bpm/engine/test/variables/simpleFile.txt").setTransient(true).create()).putValueTyped("hi", untypedValue("stringUntyped", true)).putValueTyped("null", untypedValue(null, true)).putValueTyped("ser", serializedObjectValue("{\"name\" : \"foo\"}", true).create());

		foreach (KeyValuePair<string, object> e in variableMap.SetOfKeyValuePairs())
		{
		  TypedValue value = (TypedValue) variableMap.getValueTyped(e.Key);
		  assertTrue(value.Transient);
		}
	  }
	}

}