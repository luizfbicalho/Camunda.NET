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
namespace org.camunda.bpm.engine.rest.standalone
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.hal.cache.HalRelationCacheConfiguration.CONFIG_CACHES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.hal.cache.HalRelationCacheConfiguration.CONFIG_CACHE_IMPLEMENTATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Cache = org.camunda.bpm.engine.rest.cache.Cache;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using HalLinkResolver = org.camunda.bpm.engine.rest.hal.HalLinkResolver;
	using HalResource = org.camunda.bpm.engine.rest.hal.HalResource;
	using DefaultHalResourceCache = org.camunda.bpm.engine.rest.hal.cache.DefaultHalResourceCache;
	using HalRelationCacheBootstrap = org.camunda.bpm.engine.rest.hal.cache.HalRelationCacheBootstrap;
	using HalRelationCacheConfiguration = org.camunda.bpm.engine.rest.hal.cache.HalRelationCacheConfiguration;
	using HalRelationCacheConfigurationException = org.camunda.bpm.engine.rest.hal.cache.HalRelationCacheConfigurationException;
	using HalIdentityLink = org.camunda.bpm.engine.rest.hal.identitylink.HalIdentityLink;
	using HalUser = org.camunda.bpm.engine.rest.hal.user.HalUser;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using Matchers = org.mockito.Matchers;

	public class HalResourceCacheTest : AbstractRestServiceTest
	{

	  protected internal DefaultHalResourceCache cache;
	  protected internal HalRelationCacheBootstrap contextListener;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void createCache()
	  public virtual void createCache()
	  {
		cache = new DefaultHalResourceCache(100, 100);
		contextListener = new HalRelationCacheBootstrap();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void destroy()
	  public virtual void destroy()
	  {
		contextListener.contextDestroyed(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResourceRetrieval()
	  public virtual void testResourceRetrieval()
	  {
		cache.put("hello", "world");

		assertNull(cache.get(null));
		assertNull(cache.get("unknown"));
		assertEquals("world", cache.get("hello"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCacheCapacity()
	  public virtual void testCacheCapacity()
	  {
		assertEquals(0, cache.size());

		cache.put("a", "a");
		cache.put("b", "b");
		cache.put("c", "c");
		assertEquals(3, cache.size());

		forwardTime(100);

		for (int i = 0; i < 2 * cache.Capacity; i++)
		{
		  cache.put("id" + i, i);
		}
		assertTrue(cache.size() <= cache.Capacity);

		// old entries should be removed
		assertNull(cache.get("a"));
		assertNull(cache.get("b"));
		assertNull(cache.get("c"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEntryExpiration()
	  public virtual void testEntryExpiration()
	  {
		cache.put("hello", "world");

		assertEquals("world", cache.get("hello"));
		assertEquals(1, cache.size());

		forwardTime(cache.SecondsToLive + 1);

		assertNull(cache.get("hello"));
		assertEquals(0, cache.size());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidConfigurationFormat()
	  public virtual void testInvalidConfigurationFormat()
	  {
		try
		{
		  contextListener.configureCaches("<!xml>");
		  fail("Exception expected");
		}
		catch (HalRelationCacheConfigurationException e)
		{
		  assertTrue(e.InnerException is IOException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnknownCacheImplementationClass()
	  public virtual void testUnknownCacheImplementationClass()
	  {
		try
		{
		  contextListener.configureCaches("{\"" + CONFIG_CACHE_IMPLEMENTATION + "\": \"org.camunda.bpm.UnknownCache\" }");
		  fail("Exception expected");
		}
		catch (HalRelationCacheConfigurationException e)
		{
		  assertTrue(e.InnerException is ClassNotFoundException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCacheImplementationNotImplementingCache()
	  public virtual void testCacheImplementationNotImplementingCache()
	  {
		try
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  contextListener.configureCaches("{\"" + CONFIG_CACHE_IMPLEMENTATION + "\": \"" + this.GetType().FullName + "\" }");
		  fail("Exception expected");
		}
		catch (HalRelationCacheConfigurationException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  assertTrue(e.Message.contains(typeof(Cache).FullName));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCacheCreation()
	  public virtual void testCacheCreation()
	  {
		string contextParameter = "{" +
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			"\"" + CONFIG_CACHE_IMPLEMENTATION + "\": \"" + typeof(DefaultHalResourceCache).FullName + "\"," +
			"\"" + CONFIG_CACHES + "\": {" +
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  "\"" + typeof(HalUser).FullName + "\": {" +
				"\"capacity\": 123, \"secondsToLive\": 123" +
			  "}" +
			"}" +
		  "}";

		contextListener.configureCaches(contextParameter);

		Cache cache = Hal.Instance.getHalRelationCache(typeof(HalUser));
		assertNotNull(cache);
		assertEquals(123, ((DefaultHalResourceCache) cache).Capacity);
		assertEquals(123, ((DefaultHalResourceCache) cache).SecondsToLive);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCacheInvalidParameterName()
	  public virtual void testCacheInvalidParameterName()
	  {
		HalRelationCacheConfiguration configuration = new HalRelationCacheConfiguration();
		configuration.CacheImplementationClass = typeof(DefaultHalResourceCache);
		configuration.addCacheConfiguration(typeof(HalUser), Collections.singletonMap<string, object>("unknown", "property"));

		try
		{
		  contextListener.configureCaches(configuration);
		  fail("Exception expected");
		}
		catch (HalRelationCacheConfigurationException e)
		{
		  assertTrue(e.Message.contains("setter"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEntityCaching()
	  public virtual void testEntityCaching()
	  {
		string[] userIds = new string[]{"test"};
		// mock user and query
		User user = mock(typeof(User));
		when(user.Id).thenReturn(userIds[0]);
		when(user.FirstName).thenReturn("kermit");
		UserQuery userQuery = mock(typeof(UserQuery));
		when(userQuery.userIdIn(Matchers.anyVararg<string[]>())).thenReturn(userQuery);
		when(userQuery.listPage(anyInt(), anyInt())).thenReturn(Arrays.asList(user));
		when(processEngine.IdentityService.createUserQuery()).thenReturn(userQuery);

		// configure cache
		HalRelationCacheConfiguration configuration = new HalRelationCacheConfiguration();
		configuration.CacheImplementationClass = typeof(DefaultHalResourceCache);
		IDictionary<string, object> halUserConfig = new Dictionary<string, object>();
		halUserConfig["capacity"] = 100;
		halUserConfig["secondsToLive"] = 10000;
		configuration.addCacheConfiguration(typeof(HalUser), halUserConfig);

		contextListener.configureCaches(configuration);

		// cache exists and is empty
		DefaultHalResourceCache cache = (DefaultHalResourceCache) Hal.Instance.getHalRelationCache(typeof(HalUser));
		assertNotNull(cache);
		assertEquals(0, cache.size());

		// get link resolver and resolve user
		HalLinkResolver linkResolver = Hal.Instance.getLinkResolver(typeof(UserRestService));
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> halUsers = linkResolver.resolveLinks(userIds, processEngine);
		IList<HalResource<object>> halUsers = linkResolver.resolveLinks(userIds, processEngine);

		// mocked user was resolved
		assertNotNull(halUsers);
		assertEquals(1, halUsers.Count);
		HalUser halUser = (HalUser) halUsers[0];
		assertEquals("kermit", halUser.FirstName);

		// cache contains user
		assertEquals(1, cache.size());

		// change user mock
		when(user.FirstName).thenReturn("fritz");

		// resolve users again
		halUsers = linkResolver.resolveLinks(userIds, processEngine);

		// cached mocked user was resolved with old name
		assertNotNull(halUsers);
		assertEquals(1, halUsers.Count);
		halUser = (HalUser) halUsers[0];
		assertEquals("kermit", halUser.FirstName);

		forwardTime(cache.SecondsToLive * 3);

		// resolve users again
		halUsers = linkResolver.resolveLinks(userIds, processEngine);

		// new mocked user was resolved with old name
		assertNotNull(halUsers);
		assertEquals(1, halUsers.Count);
		halUser = (HalUser) halUsers[0];
		assertEquals("fritz", halUser.FirstName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIdentityLinkCaching()
	  public virtual void testIdentityLinkCaching()
	  {
		string[] taskIds = new string[]{"test"};
		// mock identityLinks and query
		IdentityLink link1 = mock(typeof(IdentityLink));
		when(link1.TaskId).thenReturn(taskIds[0]);
		IdentityLink link2 = mock(typeof(IdentityLink));
		when(link2.TaskId).thenReturn(taskIds[0]);
		when(processEngine.TaskService.getIdentityLinksForTask(anyString())).thenReturn(Arrays.asList(link1, link2));

		// configure cache
		HalRelationCacheConfiguration configuration = new HalRelationCacheConfiguration();
		configuration.CacheImplementationClass = typeof(DefaultHalResourceCache);
		IDictionary<string, object> halIdentityLinkConfig = new Dictionary<string, object>();
		halIdentityLinkConfig["capacity"] = 100;
		halIdentityLinkConfig["secondsToLive"] = 10000;
		configuration.addCacheConfiguration(typeof(HalIdentityLink), halIdentityLinkConfig);

		contextListener.configureCaches(configuration);

		// cache exists and is empty
		DefaultHalResourceCache cache = (DefaultHalResourceCache) Hal.Instance.getHalRelationCache(typeof(HalIdentityLink));
		assertNotNull(cache);
		assertEquals(0, cache.size());

		// get link resolver and resolve identity link
		HalLinkResolver linkResolver = Hal.Instance.getLinkResolver(typeof(IdentityRestService));
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.rest.hal.HalResource<?>> halIdentityLinks = linkResolver.resolveLinks(taskIds, processEngine);
		IList<HalResource<object>> halIdentityLinks = linkResolver.resolveLinks(taskIds, processEngine);

		assertEquals(2, halIdentityLinks.Count);
		assertEquals(1, cache.size());

		assertEquals(halIdentityLinks, cache.get(taskIds[0]));
	  }

	  protected internal virtual void forwardTime(long seconds)
	  {
		DateTime later = new DateTime(ClockUtil.CurrentTime.Ticks + seconds * 1000);
		ClockUtil.CurrentTime = later;
	  }

	}

}