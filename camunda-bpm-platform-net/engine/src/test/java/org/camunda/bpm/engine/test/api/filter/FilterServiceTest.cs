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
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Query = org.camunda.bpm.engine.query.Query;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterServiceTest : PluggableProcessEngineTestCase
	{

	  protected internal Filter filter;

	  public virtual void setUp()
	  {
		filter = filterService.newTaskFilter().setName("name").setOwner("owner").setQuery(taskService.createTaskQuery()).setProperties(new Dictionary<string, object>());
		assertNull(filter.Id);
		filterService.saveFilter(filter);
		assertNotNull(filter.Id);
	  }

	  public virtual void tearDown()
	  {
		// delete all existing filters
		foreach (Filter filter in filterService.createTaskFilterQuery().list())
		{
		  filterService.deleteFilter(filter.Id);
		}
	  }

	  public virtual void testCreateFilter()
	  {
		assertNotNull(filter);

		Filter filter2 = filterService.getFilter(filter.Id);
		assertNotNull(filter2);

		compareFilter(filter, filter2);
	  }

	  public virtual void testCreateInvalidFilter()
	  {
		try
		{
		  filter.Name = null;
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  filter.Name = "";
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: filter.setQuery((org.camunda.bpm.engine.query.Query<?, ?>) null);
		  filter.Query = (Query<object, ?>) null;
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testUpdateFilter()
	  {
		filter.Name = "newName";
		filter.Owner = "newOwner";
		filter.Query = taskService.createTaskQuery();
		filter.Properties = new Dictionary<string, object>();

		filterService.saveFilter(filter);

		Filter filter2 = filterService.getFilter(filter.Id);

		compareFilter(filter, filter2);
	  }

	  public virtual void testExtendFilter()
	  {
		TaskQuery extendingQuery = taskService.createTaskQuery().taskName("newName").taskOwner("newOwner");
		Filter newFilter = filter.extend(extendingQuery);
		assertNull(newFilter.Id);

		TaskQueryImpl filterQuery = newFilter.Query;
		assertEquals("newName", filterQuery.Name);
		assertEquals("newOwner", filterQuery.Owner);
	  }

	  public virtual void testQueryFilter()
	  {

		Filter filter2 = filterService.createTaskFilterQuery().filterId(filter.Id).filterName("name").filterOwner("owner").singleResult();

		compareFilter(filter, filter2);

		filter2 = filterService.createTaskFilterQuery().filterNameLike("%m%").singleResult();

		compareFilter(filter, filter2);
	  }

	  public virtual void testQueryUnknownFilter()
	  {
		Filter unknownFilter = filterService.createTaskFilterQuery().filterId("unknown").singleResult();

		assertNull(unknownFilter);

		unknownFilter = filterService.createTaskFilterQuery().filterId(filter.Id).filterName("invalid").singleResult();

		assertNull(unknownFilter);
	  }

	  public virtual void testDeleteFilter()
	  {
		filterService.deleteFilter(filter.Id);

		filter = filterService.getFilter(filter.Id);
		assertNull(filter);
	  }

	  public virtual void testDeleteUnknownFilter()
	  {
		filterService.deleteFilter(filter.Id);
		long count = filterService.createFilterQuery().count();
		assertEquals(0, count);

		try
		{
		  filterService.deleteFilter(filter.Id);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public static void compareFilter(Filter filter1, Filter filter2)
	  {
		assertNotNull(filter1);
		assertNotNull(filter2);
		assertEquals(filter1.Id, filter2.Id);
		assertEquals(filter1.ResourceType, filter2.ResourceType);
		assertEquals(filter1.Name, filter2.Name);
		assertEquals(filter1.Owner, filter2.Owner);
		assertEquals(((FilterEntity) filter1).QueryInternal, ((FilterEntity) filter2).QueryInternal);
		assertEquals(filter1.Properties, filter2.Properties);
	  }

	}

}