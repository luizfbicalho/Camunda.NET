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
namespace org.camunda.commons.utils.cache
{
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;

	public class ConcurrentLruCacheTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private ConcurrentLruCache<string, string> cache;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createCache()
	  public virtual void createCache()
	  {
		cache = new ConcurrentLruCache<string, string>(3);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getEntryWithNotExistingKey()
	  public virtual void getEntryWithNotExistingKey()
	  {
		assertThat(cache.get("not existing")).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getEntry()
	  public virtual void getEntry()
	  {
		cache.put("a", "1");

		assertThat(cache.size()).isEqualTo(1);
		assertThat(cache.get("a")).isEqualTo("1");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void overrideEntry()
	  public virtual void overrideEntry()
	  {
		cache.put("a", "1");
		cache.put("a", "2");

		assertThat(cache.size()).isEqualTo(1);
		assertThat(cache.get("a")).isEqualTo("2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeLeastRecentlyInsertedEntry()
	  public virtual void removeLeastRecentlyInsertedEntry()
	  {
		cache.put("a", "1");
		cache.put("b", "2");
		cache.put("c", "3");
		cache.put("d", "4");

		assertThat(cache.size()).isEqualTo(3);
		assertThat(cache.get("a")).Null;
		assertThat(cache.get("b")).isEqualTo("2");
		assertThat(cache.get("c")).isEqualTo("3");
		assertThat(cache.get("d")).isEqualTo("4");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeLeastRecentlyUsedEntry()
	  public virtual void removeLeastRecentlyUsedEntry()
	  {
		cache.put("a", "1");
		cache.put("b", "2");
		cache.put("c", "3");

		cache.get("a");
		cache.get("b");

		cache.put("d", "4");

		assertThat(cache.size()).isEqualTo(3);
		assertThat(cache.get("c")).Null;
		assertThat(cache.get("a")).isEqualTo("1");
		assertThat(cache.get("b")).isEqualTo("2");
		assertThat(cache.get("d")).isEqualTo("4");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void clearCache()
	  public virtual void clearCache()
	  {
		cache.put("a", "1");

		cache.clear();
		assertThat(cache.size()).isEqualTo(0);
		assertThat(cache.get("a")).Null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToInsertInvalidKey()
	  public virtual void failToInsertInvalidKey()
	  {
		thrown.expect(typeof(System.NullReferenceException));

		cache.put(null, "1");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToInsertInvalidValue()
	  public virtual void failToInsertInvalidValue()
	  {
		thrown.expect(typeof(System.NullReferenceException));

		cache.put("a", null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void failToCreateCacheWithInvalidCapacity()
	  public virtual void failToCreateCacheWithInvalidCapacity()
	  {
		thrown.expect(typeof(System.ArgumentException));

		new ConcurrentLruCache<string, string>(-1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeElementInEmptyCache()
	  public virtual void removeElementInEmptyCache()
	  {

		// given
		cache.clear();

		// when
		cache.remove("123");

		// then
		assertThat(cache.Empty).True;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeNoneExistingKeyInCache()
	  public virtual void removeNoneExistingKeyInCache()
	  {
		//given
		cache.put("a", "1");
		cache.put("b", "2");
		cache.put("c", "3");

		// when
		cache.remove("d");

		// then
		assertThat(cache.get("a")).isEqualTo("1");
		assertThat(cache.get("b")).isEqualTo("2");
		assertThat(cache.get("c")).isEqualTo("3");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removeAllElements()
	  public virtual void removeAllElements()
	  {
		// given
		cache.put("a", "1");
		cache.put("b", "2");
		cache.put("c", "3");

		// when
		cache.remove("a");
		cache.remove("b");
		cache.remove("c");

		// then
		assertThat(cache.Empty).True;
	  }


	}

}