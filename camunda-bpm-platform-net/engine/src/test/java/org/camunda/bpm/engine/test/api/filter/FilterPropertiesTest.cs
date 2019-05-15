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
namespace org.camunda.bpm.engine.test.api.filter
{
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterPropertiesTest : PluggableProcessEngineTestCase
	{

	  protected internal Filter filter;
	  protected internal string nestedJsonObject = "{\"id\":\"nested\"}";
	  protected internal string nestedJsonArray = "[\"a\",\"b\"]";


	  public virtual void setUp()
	  {
		filter = filterService.newTaskFilter("name").setOwner("owner").setProperties(new Dictionary<string, object>());
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		if (!string.ReferenceEquals(filter.Id, null))
		{
		  filterService.deleteFilter(filter.Id);
		}
	  }


	  public virtual void testPropertiesFromNull()
	  {
		filter.Properties = null;
		assertNull(filter.Properties);

		filter.Properties = (IDictionary<string, object>) null;
		assertNull(filter.Properties);
	  }

	  public virtual void testPropertiesFromMap()
	  {
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["color"] = "#123456";
		properties["priority"] = 42;
		properties["userDefined"] = true;
		properties["object"] = nestedJsonObject;
		properties["array"] = nestedJsonArray;
		filter.Properties = properties;

		assertTestProperties();
	  }

	  protected internal virtual void assertTestProperties()
	  {
		filterService.saveFilter(filter);
		filter = filterService.getFilter(filter.Id);

		IDictionary<string, object> properties = filter.Properties;
		assertEquals(5, properties.Count);
		assertEquals("#123456", properties["color"]);
		assertEquals(42, properties["priority"]);
		assertEquals(true, properties["userDefined"]);
		assertEquals(nestedJsonObject, properties["object"]);
		assertEquals(nestedJsonArray, properties["array"]);
	  }

	  public virtual void testNullProperty()
	  {
		// given
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["null"] = null;
		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		// then
		IDictionary<string, object> persistentProperties = filter.Properties;
		assertEquals(1, persistentProperties.Count);
		assertTrue(persistentProperties.ContainsKey("null"));
		assertNull(persistentProperties["null"]);

	  }

	  public virtual void testMapContainingListProperty()
	  {
		// given
		System.Collections.IDictionary properties = Collections.singletonMap("foo", Collections.singletonList("bar"));

		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		System.Collections.IDictionary deserialisedProperties = filter.Properties;
		System.Collections.IList list = (System.Collections.IList) deserialisedProperties["foo"];
		object @string = list[0];

		// then
		assertThat(deserialisedProperties.Count, @is(1));
		assertThat(@string, instanceOf(typeof(string)));
		assertThat(@string.ToString(), @is("bar"));
	  }

	  public virtual void testMapContainingMapProperty()
	  {
		// given
		System.Collections.IDictionary properties = Collections.singletonMap("foo", Collections.singletonMap("bar", "foo"));

		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		System.Collections.IDictionary deserialisedProperties = filter.Properties;

		System.Collections.IDictionary map = (System.Collections.IDictionary) deserialisedProperties["foo"];
		object @string = map["bar"];

		// then
		assertThat(deserialisedProperties.Count, @is(1));
		assertThat(@string.ToString(), @is("foo"));
	  }

	  public virtual void testMapContainingMapContainingListProperty()
	  {
		// given
		System.Collections.IDictionary properties = Collections.singletonMap("foo", Collections.singletonMap("bar", Collections.singletonList("foo")));

		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		System.Collections.IDictionary deserialisedProperties = filter.Properties;

		System.Collections.IDictionary map = (System.Collections.IDictionary) deserialisedProperties["foo"];
		System.Collections.IList list = (System.Collections.IList) map["bar"];
		object @string = list[0];

		// then
		assertThat(deserialisedProperties.Count, @is(1));
		assertThat(@string.ToString(), @is("foo"));
	  }

	  public virtual void testMapContainingListContainingMapProperty_DeserializePrimitives()
	  {
		// given
		IDictionary<string, object> primitives = new Dictionary<string, object>();
		primitives["string"] = "aStringValue";
		primitives["int"] = 47;
		primitives["intOutOfRange"] = int.MaxValue + 1L;
		primitives["long"] = long.MaxValue;
		primitives["double"] = 3.14159265359D;
		primitives["boolean"] = true;
		primitives["null"] = null;

		System.Collections.IDictionary properties = Collections.singletonMap("foo", Collections.singletonList(primitives));

		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		System.Collections.IDictionary deserialisedProperties = filter.Properties;

		System.Collections.IList list = (System.Collections.IList) deserialisedProperties["foo"];
		System.Collections.IDictionary map = (System.Collections.IDictionary) list[0];

		// then
		assertThat(deserialisedProperties.Count, @is(1));
		assertThat((string) map["string"], @is("aStringValue"));
		assertThat((int) map["int"], @is(47));
		assertThat((long) map["intOutOfRange"], @is(int.MaxValue + 1L));
		assertThat((long) map["long"], @is(long.MaxValue));
		assertThat((double) map["double"], @is(3.14159265359D));
		assertThat((bool) map["boolean"], @is(true));
		assertThat(map["null"], nullValue());
	  }

	  public virtual void testMapContainingMapContainingListProperty_DeserializePrimitives()
	  {
		// given
		IList<object> primitives = new List<object>();
		primitives.Add("aStringValue");
		primitives.Add(47);
		primitives.Add(int.MaxValue + 1L);
		primitives.Add(long.MaxValue);
		primitives.Add(3.14159265359D);
		primitives.Add(true);
		primitives.Add(null);

		System.Collections.IDictionary properties = Collections.singletonMap("foo", Collections.singletonMap("bar", primitives));

		filter.Properties = properties;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		System.Collections.IDictionary deserialisedProperties = filter.Properties;

		System.Collections.IList list = (System.Collections.IList)((System.Collections.IDictionary) deserialisedProperties["foo"])["bar"];

		// then
		assertThat(deserialisedProperties.Count, @is(1));

		assertThat((string) list[0], @is("aStringValue"));
		assertThat((int) list[1], @is(47));
		assertThat((long) list[2], @is(int.MaxValue + 1L));
		assertThat((long) list[3], @is(long.MaxValue));
		assertThat((double) list[4], @is(3.14159265359D));
		assertThat((bool) list[5], @is(true));
		assertThat(list[6], nullValue());
	  }

	}

}