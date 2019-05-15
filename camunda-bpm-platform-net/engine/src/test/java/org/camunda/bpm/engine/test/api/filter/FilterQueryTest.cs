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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.contains;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.beans.HasPropertyWithValue.hasProperty;


	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal IList<string> filterIds = new List<string>();

	  public virtual void setUp()
	  {
		saveFilter("b", "b");
		saveFilter("d", "d");
		saveFilter("a", "a");
		saveFilter("c_", "c");
	  }

	  protected internal virtual void saveFilter(string name, string owner)
	  {
		Filter filter = filterService.newTaskFilter().setName(name).setOwner(owner);
		filterService.saveFilter(filter);
		filterIds.Add(filter.Id);
	  }

	  public virtual void tearDown()
	  {
		// delete all filters
		foreach (Filter filter in filterService.createFilterQuery().list())
		{
		  filterService.deleteFilter(filter.Id);
		}
	  }

	  public virtual void testQueryNoCriteria()
	  {
		FilterQuery query = filterService.createFilterQuery();
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());
		try
		{
		  query.singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryByFilterId()
	  {
		FilterQuery query = filterService.createFilterQuery().filterId(filterIds[0]);
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidFilterId()
	  {
		FilterQuery query = filterService.createFilterQuery().filterId("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  filterService.createFilterQuery().filterId(null);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryByResourceType()
	  {
		FilterQuery query = filterService.createFilterQuery().filterResourceType(EntityTypes.TASK);
		try
		{
		  query.singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
		assertEquals(4, query.list().size());
		assertEquals(4, query.count());
	  }

	  public virtual void testQueryByInvalidResourceType()
	  {
		FilterQuery query = filterService.createFilterQuery().filterResourceType("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  filterService.createFilterQuery().filterResourceType(null);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryByName()
	  {
		FilterQuery query = filterService.createFilterQuery().filterName("a");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByNameLike()
	  {
		FilterQuery query = filterService.createFilterQuery().filterNameLike("%\\_");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidName()
	  {
		FilterQuery query = filterService.createFilterQuery().filterName("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  filterService.createFilterQuery().filterName(null);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryByOwner()
	  {
		FilterQuery query = filterService.createFilterQuery().filterOwner("a");
		assertNotNull(query.singleResult());
		assertEquals(1, query.list().size());
		assertEquals(1, query.count());
	  }

	  public virtual void testQueryByInvalidOwner()
	  {
		FilterQuery query = filterService.createFilterQuery().filterOwner("invalid");
		assertNull(query.singleResult());
		assertEquals(0, query.list().size());
		assertEquals(0, query.count());

		try
		{
		  filterService.createFilterQuery().filterOwner(null);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testQueryPaging()
	  {
		FilterQuery query = filterService.createFilterQuery();

		assertEquals(4, query.listPage(0, int.MaxValue).size());

		// Verifying the un-paged results
		assertEquals(4, query.count());
		assertEquals(4, query.list().size());

		// Verifying paged results
		assertEquals(2, query.listPage(0, 2).size());
		assertEquals(2, query.listPage(2, 2).size());
		assertEquals(1, query.listPage(3, 1).size());

		// Verifying odd usages
		assertEquals(0, query.listPage(-1, -1).size());
		assertEquals(0, query.listPage(4, 2).size()); // 4 is the last index with a result
		assertEquals(4, query.listPage(0, 15).size()); // there are only 4 tasks
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public void testQuerySorting()
	  public virtual void testQuerySorting()
	  {
		IList<string> sortedIds = new List<string>(filterIds);
		sortedIds.Sort();
		assertEquals(4, filterService.createFilterQuery().orderByFilterId().asc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterId().asc().list(), contains(hasProperty("id", equalTo(sortedIds[0])), hasProperty("id", equalTo(sortedIds[1])), hasProperty("id", equalTo(sortedIds[2])), hasProperty("id", equalTo(sortedIds[3]))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterResourceType().asc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterResourceType().asc().list(), contains(hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterName().asc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterName().asc().list(), contains(hasProperty("name", equalTo("a")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("d"))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterOwner().asc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterOwner().asc().list(), contains(hasProperty("owner", equalTo("a")), hasProperty("owner", equalTo("b")), hasProperty("owner", equalTo("c")), hasProperty("owner", equalTo("d"))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterId().desc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterId().desc().list(), contains(hasProperty("id", equalTo(sortedIds[3])), hasProperty("id", equalTo(sortedIds[2])), hasProperty("id", equalTo(sortedIds[1])), hasProperty("id", equalTo(sortedIds[0]))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterResourceType().desc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterResourceType().desc().list(), contains(hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK)), hasProperty("resourceType", equalTo(EntityTypes.TASK))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterName().desc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterName().desc().list(), contains(hasProperty("name", equalTo("d")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("a"))));

		assertEquals(4, filterService.createFilterQuery().orderByFilterOwner().desc().list().size());
		Assert.assertThat(filterService.createFilterQuery().orderByFilterOwner().desc().list(), contains(hasProperty("name", equalTo("d")), hasProperty("name", equalTo("c_")), hasProperty("name", equalTo("b")), hasProperty("name", equalTo("a"))));

		assertEquals(1, filterService.createFilterQuery().orderByFilterId().filterName("a").asc().list().size());
		assertEquals(1, filterService.createFilterQuery().orderByFilterId().filterName("a").desc().list().size());
	  }

	  public virtual void testNativeQuery()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		assertEquals(tablePrefix + "ACT_RU_FILTER", managementService.getTableName(typeof(Filter)));
		assertEquals(tablePrefix + "ACT_RU_FILTER", managementService.getTableName(typeof(FilterEntity)));
		assertEquals(4, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Filter))).list().size());
		assertEquals(4, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Filter))).count());

		assertEquals(16, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + tablePrefix + "ACT_RU_FILTER F1, " + tablePrefix + "ACT_RU_FILTER F2").count());

		// select with distinct
		assertEquals(4, taskService.createNativeTaskQuery().sql("SELECT F1.* FROM " + tablePrefix + "ACT_RU_FILTER F1").list().size());

		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Filter)) + " F WHERE F.NAME_ = 'a'").count());
		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Filter)) + " F WHERE F.NAME_ = 'a'").list().size());

		// use parameters
		assertEquals(1, taskService.createNativeTaskQuery().sql("SELECT count(*) FROM " + managementService.getTableName(typeof(Filter)) + " F WHERE F.NAME_ = #{filterName}").parameter("filterName", "a").count());
	  }

	  public virtual void testNativeQueryPaging()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		assertEquals(tablePrefix + "ACT_RU_FILTER", managementService.getTableName(typeof(Filter)));
		assertEquals(tablePrefix + "ACT_RU_FILTER", managementService.getTableName(typeof(FilterEntity)));
		assertEquals(3, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Filter))).listPage(0, 3).size());
		assertEquals(2, taskService.createNativeTaskQuery().sql("SELECT * FROM " + managementService.getTableName(typeof(Filter))).listPage(2, 2).size());
	  }

	}

}